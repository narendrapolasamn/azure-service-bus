﻿using Microsoft.AspNetCore.Builder;

namespace Car.API.Core.DependencyInjection
{
    public static class AppBuilderExtensions
    {
        public static void UseSwaggerServices(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cars Catalog API v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
