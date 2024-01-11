using carModel = Car.Domain.Model;
using FluentValidation;

namespace Car.API.Infrastructure.Validators
{
    public class CarValidator : AbstractValidator<carModel.Car>
    {
        public CarValidator()
        {
            RuleFor(x => x.Brand).NotNull().NotEmpty();
            RuleFor(x => x.Model).NotNull().NotEmpty();
            RuleFor(x => x.PricePerDay).InclusiveBetween(50, 25000);
        }
    }
}
