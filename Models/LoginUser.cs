using System;
using System.ComponentModel.DataAnnotations;

namespace LoginRegWDb.Models
{
    public class LoginUser
    {
        // No other fields!
        [Required]
        public string Email {get; set;}
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
