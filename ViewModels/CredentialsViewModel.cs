//using FluentValidation.Attributes;
using robstagram.ViewModels.Validations;

namespace robstagram.ViewModels
{
    // TODO [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}