using Car.API.Infrastructure.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using carModel = Car.Domain.Model;

namespace Car.API.Core.DependencyInjection
{
    public static class ModelValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddModelValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<carModel.Car>, CarValidator>();
            return services;
        }
    }
}
