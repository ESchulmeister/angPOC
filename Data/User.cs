using System;
using System.Collections.Generic;

#nullable disable

namespace angPOC.Data
{
    public partial class User
    {
        public int UsrId { get; set; }
        public string UsrLogin { get; set; }
        public string UsrLastName { get; set; }
        public string UsrFirstName { get; set; }
        public string UsrClock { get; set; }
        public string UsrEmail { get; set; }
        public bool? UsrActive { get; set; } = true;
        public int? UsrStateId { get; set; }
        public string UsrCreatedBy { get; set; }
        public DateTime UsrCreatedDate { get; set; }
        public string UsrModifiedBy { get; set; }
        public DateTime UsrModifiedDate { get; set; }
    }
}
