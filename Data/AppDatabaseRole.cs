using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class AppDatabaseRole
    {
        public int AdbId { get; set; }
        public int? AppId { get; set; }
        public string AdbName { get; set; }
        public string AdbRoleName { get; set; }
        public string AdbAccessKey { get; set; }
        public bool? AdbActive { get; set; }
        public string AdbCreatedBy { get; set; }
        public DateTime AdbCreatedDate { get; set; }
        public string AdbModifiedBy { get; set; }
        public DateTime AdbModifiedDate { get; set; }
    }
}
