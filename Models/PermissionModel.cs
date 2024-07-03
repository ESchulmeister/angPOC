using System.ComponentModel.DataAnnotations;

namespace angPOC.Models
{
    public class PermissionModel
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }


    }
}
