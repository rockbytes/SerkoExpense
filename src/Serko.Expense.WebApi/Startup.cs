using System;
using System.IO;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serko.Expense.ApplicationCore.Interfaces;
using Serko.Expense.ApplicationCore.Services;
using Serko.Expense.ApplicationCore.Validators;
using Serko.Expense.WebApi.Filters;
using Serko.Expense.WebApi.Formatters;
using Swashbuckle.AspNetCore.Swagger;

namespace Serko.Expense.WebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IExpenseService, ExpenseService>();

            services.AddMvc(options =>
                {
                    // To support text/plain or on media type input
                    options.InputFormatters.Insert(0, new RawRequestBodyFormatter());

                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ExpenseClaimInputValidator>());

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info
				{
					Version = "v1",
					Title = "Serko API"
				});

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serko API V1");
			});

			app.UseMvc();
		}
	}
}
