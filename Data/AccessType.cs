using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class AccessType
    {
        public AccessType()
        {
            AppUserPermissions = new HashSet<AppUserPermission>();
        }

        public int ActId { get; set; }
        public string ActName { get; set; }

        public virtual ICollection<AppUserPermission> AppUserPermissions { get; set; }
    }
}
