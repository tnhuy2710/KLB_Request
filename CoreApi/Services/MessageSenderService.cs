using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Extensions;
using KlbService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreApi.Services
{
    public interface IMessageSender
    {
        Task<bool> SendSmsAsync(string message, string phoneNumber);
        Task<bool> SendEmailAsync(string email, string subject, string message);

        /// <summary>
        /// Get file email template.
        /// </summary>
        /// <param name="fileName">File name without extension</param>
        /// <returns></returns>
        string GetEmailTemplate(string fileName);
    }

    public class MessageSenderService : IMessageSender
    {
        private readonly IConfiguration _configuration;
        private readonly MercuryService _mercuryService;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _log;

        public MessageSenderService(IConfiguration configuration, ILogger<MessageSenderService> log, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _log = log;
            _environment = environment;
            _mercuryService = new MercuryService(configuration.GetValue<string>("WebServiceUrls:MercuryWebService"));
        }

        public async Task<bool> SendSmsAsync(string message, string phoneNumber)
        {
            try
            {
                if (StringExtensions.IsEmptyOrNull(message, phoneNumber))
                    return false;

                return await _mercuryService.SendSmsAsync(message.Trim(), phoneNumber.Trim());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error cant send new sms: " + e.Message);
                _log.LogError("Error cant send new sms: " + e.Message);
            }

            return false;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            if (StringExtensions.IsEmptyOrNull(email, subject, message))
                return false;

            return await _mercuryService.SendEmailAsync(subject, message, email);
        }

        public string GetEmailTemplate(string fileName)
        {
            var templateFolder = Path.Combine(_environment.ContentRootPath, "UserData/EmailTemplates");
            if (Directory.Exists(templateFolder))
            {
                var fileTemplate = Path.Combine(templateFolder, $"{fileName}.html");
                if (File.Exists(fileTemplate))
                    return File.ReadAllText(fileTemplate);
            }

            return "";
        }
    }
}
