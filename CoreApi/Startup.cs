using System;
using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoreApi.Data;
using CoreApi.Extensions;
using CoreApi.Middlewares;
using CoreApi.Models;
using CoreApi.Security;
using CoreApi.Security.AuthenticationHandlers;
using CoreApi.Security.AuthorizationPolicies.RolePolicy;
using CoreApi.Services;
using CoreApi.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), builder =>
                {
                    builder.EnableRetryOnFailure();
                })
            );

            // Setup Identity Framework
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                options.Lockout.MaxFailedAccessAttempts = Configuration.GetMaxFailedAccessAttemptsSetting();
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Setup Antiforgery
            services.AddAntiforgery(options =>
            {
                options.Cookie = new CookieBuilder()
                {
                    Name = ".AppAntiForgery"
                };
                options.HeaderName = "APP-XSRF-TOKEN";
            });

            // Add MVC
            services.AddMvc(options =>
            {
                // Add exception filter
                options.Filters.Add(typeof(ExceptionFilter));

                // Support Return Json Equals Property Name or JsonProperty Define
                var jsonOutputFormat = new JsonOutputFormatter(new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver(),
                    DateFormatString = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"
                }, ArrayPool<char>.Shared);

                options.OutputFormatters.Insert(0, jsonOutputFormat);
            });

            // Setup custom Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = SchemeConstants.Api;
            })
            .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(SchemeConstants.Api, options =>
            {

            });

            // Config Cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AppCookie";
                options.Cookie.Expiration = TimeSpan.FromMinutes(Configuration.GetCookieLifetimeSetting());
                options.Events.OnValidatePrincipal = CookiePrincipalValidator.ValidateAsync;
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(1);
            });
            

            // DataProtect
            services.AddDataProtection()
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
            });

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IMessageSender, MessageSenderService>();
            services.AddTransient<IKlbService, Services.KlbService>();

            services.AddSingleton<GlReportContext>();

            // Node Service
            services.AddNodeServices();

            // Build all Repository Service
            // Comment it if in design mode
            services.AddRepositories();

            // Authorization Setups
            services.SetupMinimunRolePolicy();

            // Build all service
            var serviceProvider = services.BuildServiceProvider();

            // Setup RSA Crypto Algorithm
            //CryptoUtils.RSA.Init(Configuration.GetSection("Security")["CertificateThumbprint"]);
            CryptoUtils.AES.InitDataProtect(serviceProvider.GetService<IDataProtectionProvider>());

            // Build the intermediate service provider
            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            //Comment it if in design mode
            app.DataSeed();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "admin_default",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }
                    );
            });
        }
    }
}
