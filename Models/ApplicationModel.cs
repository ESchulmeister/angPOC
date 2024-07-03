using System;
using System.ComponentModel.DataAnnotations;

namespace angPOC.Models
{
    public class ApplicationModel
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(255), MinLength(3)]
        public string? Name { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; } 

        public DateTime ModifiedDate { get; set; }


    }
}
