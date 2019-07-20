using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }    

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password cannot longer than 8 and shorter than 4")]
        public string Password { get; set; }
    }
}