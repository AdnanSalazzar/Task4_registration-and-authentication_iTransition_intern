using System.ComponentModel.DataAnnotations;

namespace Task4_registration_and_authentication.ViewModels
{
    public class LoginVM
    {
        [Required (ErrorMessage = "Email Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
