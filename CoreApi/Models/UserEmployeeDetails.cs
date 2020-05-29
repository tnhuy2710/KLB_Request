using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserEmployeeDetails
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutStart { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }

        //
        public string EMPID { get; set; }
        public string EmpCode { get; set; }
        public string FULLNAME { get; set; }
        public string TITLE { get; set; }
        public string EMAIL { get; set; }
        public string LEVEL1ID { get; set; }
        public string LEVEL1NAME { get; set; }
        public string LEVEL2ID { get; set; }
        public string LEVEL2NAME { get; set; }
        public string LEVEL3ID { get; set; }
        public string LEVEL3NAME { get; set; }
        public decimal ISMANAGER { get; set; }
        public string LSJOBGROUPID { get; set; }
        public string POSITIONID { get; set; }
        public string LSPOSITIONCODE { get; set; }
        public string WORKINGDATE { get; set; }
        public string DIRECTEMPID { get; set; }
        public string DIRECTEMPMANAGER { get; set; }
        public string THUONGTRU { get; set; }
        public string TAMTRU { get; set; }
        public string BIRTHDAY { get; set; }
        public string GENDER { get; set; }
        public string PIN { get; set; }
        public string PINDATE { get; set; }
        public string PINPLACE { get; set; }
        public string LSPOSGROUPID { get; set; }
        public string LSPOSGROUPCODEID { get; set; }
        public DateTime POSITION_DATE { get; set; }

        public string GetLevelName()
        {
            if (!string.IsNullOrEmpty(LEVEL1NAME))
            {
                var levelName = LEVEL1NAME;
                if (!string.IsNullOrEmpty(LEVEL2NAME))
                    levelName = LEVEL2NAME;

                return levelName.Trim();
            }

            return "";
        }
    }
}
