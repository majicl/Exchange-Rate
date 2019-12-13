using System.Reflection;
using ExchangeRate.Application.Currency.Queries;
using ExchangeRate.Domain.Currency;
using ExchangeRate.Infrastructure.Currency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using System.IO;
using System;

namespace ExchangeRate.API
{
    public class Startup
    {
        private const string ExchangeRateAPIUrl = "ExchangeRateAPIUrl";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<ICurrencyExchangeRepository, CurrencyExchangeRepository>();
            services.AddSingleton<ICurrencyExchangeRepository, CurrencyExchangeRepository>(serviceProvider =>
            {
                return new CurrencyExchangeRepository(Configuration[ExchangeRateAPIUrl]);
            });
            services.AddMediatR(typeof(CurrencyExchangeRateQuery).GetTypeInfo().Assembly);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v0.1",
                    Title = "Currency Exchange Rate API",
                    Description = "A Foreign exchange rates API with currency conversion"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Currency Exchange Rate API Documentation";
                c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Currency Exchange Rate API v0.1");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
