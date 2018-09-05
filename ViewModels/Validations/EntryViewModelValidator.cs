using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace robstagram.ViewModels.Validations
{
    public class EntryViewModelValidator : AbstractValidator<EntryViewModel>
    {
        public EntryViewModelValidator()
        {
            RuleFor(vm => vm.Description).NotEmpty().WithMessage("Description cannot be empty");
        }
    }
}
