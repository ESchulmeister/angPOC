using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class AppUserPermission
    {
        public int PerId { get; set; }
        public int? ApuId { get; set; }
        public int ApId { get; set; }
        public int? ActId { get; set; }
        public string PerMetadata { get; set; }
        public bool? PerActive { get; set; }
        public string PerCreatedBy { get; set; }
        public DateTime? PerCreatedDate { get; set; }
        public string PerModifiedBy { get; set; }
        public DateTime PerModifiedDate { get; set; }

        public virtual AccessType Act { get; set; }
        public virtual AppPermission Ap { get; set; }
        public virtual AppUser Apu { get; set; }
    }
}
