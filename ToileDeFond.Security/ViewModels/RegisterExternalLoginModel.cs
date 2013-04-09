using System.ComponentModel.DataAnnotations;

namespace ToileDeFond.Security.ViewModels
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "IUser name")]
        public string UserName { get; set; }

        //TODO: Validation avec confirmation email - see: http://stackoverflow.com/questions/201323/using-a-regular-expression-to-validate-an-email-address
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string ExternalLoginData { get; set; }
    }
}