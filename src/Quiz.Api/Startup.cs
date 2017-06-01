﻿using EasyEventSourcing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quiz.Domain;
using Quiz.Domain.Commands;
using Swashbuckle.AspNetCore.Swagger;

namespace Quiz.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public ILogger<Startup> Logger { get; }

        public Startup(ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            Configuration = builder.Build();
            Logger = loggerFactory.CreateLogger<Startup>();
            loggerFactory.AddConsole();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddMvcCore().AddApiExplorer().AddJsonFormatters();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new Info { Title = "Quiz Voting API", Version = "v1" })
            );

            services.AddEasyEventSourcing<QuizAggregate>(
                EventStoreOptions.Create(
                    Configuration["EVENT_STORE"],
                    Configuration["EVENT_STORE_MANAGER_HOST"]));

            services.AddTransient<QuizAppService>();
        }

        public void Configure(IApplicationBuilder app, QuizAppService quizAppService)
        {
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
            );
        }
    }
}
