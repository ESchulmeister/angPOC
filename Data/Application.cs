using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class Application
    {
        public Application() => AppPermissions = new HashSet<AppPermission>();

        public int AppId { get; set; }
        public string AppName { get; set; }
        public int? AppFlags { get; set; }
        public bool? AppActive { get; set; }
        public string AppCreatedBy { get; set; }
        public DateTime AppCreatedDate { get; set; }
        public string AppModifiedBy { get; set; }
        public DateTime AppModifiedDate { get; set; }

        public virtual ICollection<AppPermission> AppPermissions { get; set; }
    }
}
