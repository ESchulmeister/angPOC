using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class AppUser
    {
        public AppUser()
        {
            AppUserPermissions = new HashSet<AppUserPermission>();
            AppUserSettings = new HashSet<AppUserSetting>();
        }

        public int ApuId { get; set; }
        public int AppId { get; set; }
        public int UsrId { get; set; }
        public bool? ApuActive { get; set; }
        public byte[] ApuAccessKey { get; set; }
        public string ApuCreatedBy { get; set; }
        public DateTime ApuCreatedDate { get; set; }
        public string ApuModifiedBy { get; set; }
        public DateTime ApuModifiedDate { get; set; }

        public virtual Application App { get; set; }
        public virtual User Usr { get; set; }
        public virtual ICollection<AppUserPermission> AppUserPermissions { get; set; }
        public virtual ICollection<AppUserSetting> AppUserSettings { get; set; }
    }
}
