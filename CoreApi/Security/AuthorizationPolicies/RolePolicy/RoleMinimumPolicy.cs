using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Security.AuthorizationPolicies.RolePolicy
{
    public class RoleMinimumPolicy
    {
        /// <summary>
        /// Allow Registered User above can access to this feature.
        /// </summary>
        public const string Register = "MinRegisterLevel";

        /// <summary>
        /// Allow Moderator User above can access to this feature.
        /// </summary>
        public const string Moderator = "MinModeratorLevel";

        /// <summary>
        /// Allow Developer User above can access to this feature.
        /// </summary>
        public const string Developer = "MinDeveloperLevel";


        /// <summary>
        /// Allow Supporter User above can access to this feature.
        /// </summary>
        public const string Supporter = "MinSupporterLevel";

        /// <summary>
        /// Allow Only Administrator can access to this feature.
        /// </summary>
        public const string Administrator = "MinAdminLevel";
    }
}
