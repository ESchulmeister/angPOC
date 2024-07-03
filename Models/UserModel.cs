using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace angPOC.Models
{

   //[Table("User")]
    public class UserModel
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50), MinLength(5)]
        public string? UserName { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string? FirstName { get; set; }

        [MaxLength(4), MinLength(4)]
        public string? Clock { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255), MinLength(5)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int? stateID { get; set; } 

        [Required]
        public DateTime CreateDate { get; set; }

        public string? CreatedBy { get; set; } 

        public DateTime UpdateDate { get; set; }


        [MaxLength(50), MinLength(1)]
        public string? ModifiedBy { get; set; }

    }
}
