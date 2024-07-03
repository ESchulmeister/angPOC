using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class AppUserSetting
    {
        public int SetId { get; set; }
        public int? ApuId { get; set; }
        public string SetName { get; set; }
        public string SetValue { get; set; }
        public int? SetContentTypeId { get; set; }
        public bool? SetActive { get; set; }
        public string SetCreatedBy { get; set; }
        public DateTime? SetCreatedDate { get; set; }
        public string SetModifiedBy { get; set; }
        public DateTime SetModifiedDate { get; set; }

        public virtual AppUser Apu { get; set; }
        public virtual ContentType SetContentType { get; set; }
    }
}
