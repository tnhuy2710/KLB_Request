using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Security.Attributes
{
    [Flags]
    public enum RequirePermissionType
    {
        View,
        Create,
        Edit,
        Delete
    }
}
