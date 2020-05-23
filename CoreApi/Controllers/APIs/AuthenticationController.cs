using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Data.Repositories;
using CoreApi.Enums;
using CoreApi.Extensions;
using CoreApi.Models;
using CoreApi.Security;
using CoreApi.Utilities;
using CoreApi.ViewModels.AuthenticationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreApi.Controllers.APIs
{
    public class AuthenticationController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthenticationController(
            IUserRepository userRepository, 
            SignInManager<User> signInManager, 
            UserManager<User> userManager, 
            ILogger<AuthenticationController> logger, 
            ITokenRepository tokenRepository, 
            IDeviceRepository deviceRepository, 
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _tokenRepository = tokenRepository;
            _deviceRepository = deviceRepository;
            _configuration = configuration;
        }


        /// <summary>
        /// Send Request to Login
        /// </summary>
        /// <param name="vm"></param>
        /// <status>
        /// 400.1 
        /// </status>
        [AllowAnonymous]
        [Route("~/api/auth/request")]
        [HttpPost]
        public async Task<DataResponse> AuthenticationRequest([FromBody]AuthenticationRequestViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Find user
                var user = await _userRepository.FindByUserNameAsync(vm.UserName);

                // If user exist then check permission
                if (user != null)
                {
                    // Next Step
                    // Ensure the user is allowed to sign in.
                    if (user.LockoutStart.HasValue && user.LockoutStart.Value <= DateTimeOffset.UtcNow)
                    {
                        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                        {
                            _logger.LogInformation($"User {user.UserName} login but this account have been locked to {user.LockoutEnd.Value}");
                            return BadRequest(
                                "Tài khoản của bạn hiện đang bị tạm khóa truy cập.",
                                400.7);
                        }
                    }

                    // Next step
                    // Ensure the user is not already locked out.
                    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                    {
                        _logger.LogInformation($"Temporary locked user {user.Email} because maximum login failed.");
                        return BadRequest(
                            "Tài khoản của bạn hiện đang bị tạm khóa do đăng nhập sai nhiều lần liên tiếp.", 
                            400.7);
                    }


                    // Next step
                    var deviceCheck = user.UserDevices?.FirstOrDefault(x => x.Device.Uuid.Equals(vm.DeviceUuid));
                    if (deviceCheck != null)
                    {
                        // Child step
                        // Check device is locked or not
                        if (deviceCheck.Device.LockoutEnd.HasValue &&
                            deviceCheck.Device.LockoutEnd.Value > DateTimeOffset.UtcNow)
                        {
                            _logger.LogInformation($"User {user.UserName} login with device {deviceCheck.DeviceId} but it has been locked to {deviceCheck.Device.LockoutEnd.Value}");
                            return BadRequest(
                                "Thiết bị này bị khóa truy cập ứng dụng. Vui lòng gọi Bộ Phận Hỗ Trợ để được giải đáp.",
                                400.8);
                        }
                    }
                }


                // Send SMS with Active Code
                var activeCode = RandomUtils.GenerateRandomNumber(6);
                var hashedActiveCode = CryptoUtils.SHA.EncryptToBase64(activeCode, "d_7#sCec2k@&RJ-f", CryptoUtils.SHA.Size.SHA512);

                var token = new Token()
                {
                    Value = hashedActiveCode,
                    UserTarget = vm.UserName.Trim().ToLower(),
                    DeviceTarget = vm.DeviceUuid.Trim().ToLower(),
                    Type = TokenType.Password,
                    ExpireIn = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10)
                };

                // Create new Temporary Token
                if (await _tokenRepository.InsertAsync(token) != null)
                {
                    var resp = new Dictionary<string, object>()
                    {
                        {"Token", hashedActiveCode},
                        {"ActiveCode", activeCode},
                    };
                    return Success(resp);
                }

                return BadRequest("Lỗi không thể tạo thông tin đăng nhập.",400.10);
            }

            return BadRequest(ModelState);
        }


        [AllowAnonymous]
        [Route("~/api/auth")]
        [HttpPost]
        public async Task<DataResponse> Authentication([FromBody]AuthenticationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Check Token First
                var activeCodeHashed = CryptoUtils.SHA.EncryptToBase64(vm.Password.Trim(), "d_7#sCec2k@&RJ-f", CryptoUtils.SHA.Size.SHA512);
                if (!activeCodeHashed.Equals(vm.Token.Trim())) return BadRequest("Sai mã kích hoạt", 400.1);

                // Check token from db
                var token = await _tokenRepository.FindByValueAsync(vm.Token);
                if (token == null) return BadRequest("Sai mã kích hoạt", 400.2);

                // Check username and device uuid match
                if (!token.UserTarget.Equals(vm.UserName.Trim().ToLower())) return BadRequest("Sai mã kích hoạt", 400.3);
                if (!token.DeviceTarget.Equals(vm.DeviceUuid.Trim().ToLower())) return BadRequest("Sai mã kích hoạt", 400.4);

                // Check token expire
                if (token.ExpireIn < DateTimeOffset.UtcNow) return BadRequest("Mã kích hoạt đã hết hạn", 400.3);

                // Find user
                var user = await _userRepository.FindByUserNameAsync(vm.UserName.Trim());
                if (user == null) 
                {
                    // Create New User
                    return BadRequest("Tạo user trước");
                }

                // Next Step
                // Ensure the user is allowed to sign in.
                if (user.LockoutStart.HasValue && user.LockoutStart.Value <= DateTimeOffset.UtcNow)
                {
                    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                    {
                        _logger.LogInformation($"User {user.UserName} login but this account have been locked to {user.LockoutEnd.Value}");
                        return BadRequest(
                            "Tài khoản của bạn hiện đang bị tạm khóa truy cập.",
                            400.7);
                    }
                }

                // Next step
                // Ensure the user is not already locked out.
                if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogInformation($"Temporary locked user {user.Email} because maximum login failed.");
                    return BadRequest(
                        "Tài khoản của bạn hiện đang bị tạm khóa do đăng nhập sai nhiều lần liên tiếp.",
                        400.7);
                }

                // Next step
                var deviceCheck = user.UserDevices?.FirstOrDefault(x => x.Device != null && x.Device.Uuid.Equals(vm.DeviceUuid));
                if (deviceCheck != null)
                {
                    // Child step
                    // Check device is locked or not
                    if (deviceCheck.Device.LockoutEnd.HasValue &&
                        deviceCheck.Device.LockoutEnd.Value > DateTimeOffset.UtcNow)
                    {
                        _logger.LogInformation($"User {user.UserName} login with device {deviceCheck.DeviceId} but it has been locked to {deviceCheck.Device.LockoutEnd.Value}");
                        return BadRequest(
                            "Thiết bị này bị khóa truy cập ứng dụng. Vui lòng gọi Bộ Phận Hỗ Trợ để được giải đáp.",
                            400.8);
                    }
                }

                // Create new device info
                var deviceInfo = await _deviceRepository.FindOneAsync(x => x.Uuid.Equals(vm.DeviceUuid.MakeLowerCase()));
                if (deviceInfo == null)
                {
                    deviceInfo = new Device()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = vm.DeviceName.Trim(),
                        Uuid = vm.DeviceUuid.Trim().ToLower(),
                        OsName = vm.DeviceOsName.Trim(),
                        OsVersion = vm.DeviceOsVersion.Trim()
                    };

                    // Insert device info to db
                    if (await _deviceRepository.InsertAsync(deviceInfo) == null)
                    {
                        _logger.LogError($"Error cant register new Device info with UUID {deviceInfo.Uuid}");
                        return BadRequest("Lỗi không thể đăng ký thông tin thiết bị.", 400.5);
                    }
                }

                
                // Register or update device to user
                var sessionToken = "";

                // Nếu Token != empty và Token chưa bị Expire so với giờ hệ thống
                // thì set sessionToken từ Db
                if (!string.IsNullOrEmpty(deviceCheck?.Token) &&
                    deviceCheck.ExpireIn > DateTimeOffset.UtcNow)
                {
                    sessionToken = deviceCheck.Token;
                }

                // Nếu session token = empty thì register mới or update
                if (string.IsNullOrEmpty(sessionToken))
                {
                    var newSession = deviceCheck ?? new UserDevice()
                    {
                        UserId = user.Id,
                        DeviceId = deviceInfo.Id,
                    };

                    // Update token and Expire Time
                    newSession.Token = CryptoUtils.SHA.Encrypt(RandomUtils.GenerateRandomNumber(9), CryptoUtils.SHA.Size.SHA512);
                    newSession.ExpireIn = DateTimeOffset.UtcNow +
                                       TimeSpan.FromMinutes(_configuration.GetTokenLifetimeSetting());

                    // Set Session Token
                    sessionToken = newSession.Token;

                    if (deviceCheck == null)
                    {
                        // Insert new Session
                        if (!await _deviceRepository.RegisterNewUserDeviceAsync(newSession))
                        {
                            _logger.LogError(
                                $"Error cant register new UserDevice of UserId {user.Id} and DeviceId {deviceInfo.Id}");
                            return BadRequest("Lỗi không thể khởi tạo phiên đăng nhập mới", 400.3);
                        }
                    }
                    else
                    {
                        // Update new Session
                        if (!await _deviceRepository.UpdateUserDeviceAsync(newSession))
                        {
                            _logger.LogError(
                                $"Error cant update new UserDevice of UserId {user.Id} and DeviceId {deviceInfo.Id}");
                            return BadRequest("Lỗi không thể khởi tạo phiên đăng nhập mới", 400.3);
                        }
                    }

                }

                // Success and reset access failed count
                await _userManager.ResetAccessFailedCountAsync(user);

                // Create Claims
                var claims = new Dictionary<string, string>()
                {
                    {ClaimTypes.NameIdentifier, user.Id},
                    {ClaimTypes.Name, user.UserName},
                    {ClaimTypes.Email, user.Email},
                    {ClaimContants.DeviceId, deviceInfo.Id},
                    {ClaimContants.SessionToken, sessionToken},
                };

                // Encrypt Claims to Token
                var contentEncryptLevel1 = CryptoUtils.AES.EncryptTwoLevel(JsonConvert.SerializeObject(claims));

                var resp = new Dictionary<string, object>()
                {
                    {"AccessToken", contentEncryptLevel1 },
                };

                return Success(resp);
            }

            return BadRequest(ModelState);
        }

    }
}
