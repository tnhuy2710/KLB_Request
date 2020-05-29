using Microsoft.Extensions.Configuration;

namespace CoreApi.Extensions
{
    public static class ConfigurationExtensions
    {
        public static int GetMaxFailedAccessAttemptsSetting(this IConfiguration config)
        {
            return int.Parse(config.GetSection("AppSettings")["MaxFailedAccessAttempts"]);
        }

        public static long GetCookieLifetimeSetting(this IConfiguration config)
        {
            return long.Parse(config.GetSection("AppSettings")["CookieLifeTimeInMinutes"]);
        }

        public static long GetTokenLifetimeSetting(this IConfiguration config)
        {
            return long.Parse(config.GetSection("AppSettings")["TokenLifeTimeInMinutes"]);
        }
    }
}
