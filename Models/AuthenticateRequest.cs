    using System.ComponentModel.DataAnnotations;


namespace angPOC.Models
{
  
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "8 characters")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", 
            ErrorMessage = "8 alpha-numeric characters with at least 1 Upper case letter and 1 special character - [@, #, $, %, ^, *, (, ), !, <,>,?] ") ]
        public string Password { get; set; }

        [Required]
        public int appID { get; set; }
    }
}
