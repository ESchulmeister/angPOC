using System.ComponentModel.DataAnnotations;

namespace angPOC.Models
{
    public class StateModel
    {


        public int ID { get; set; }

        [Required]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
