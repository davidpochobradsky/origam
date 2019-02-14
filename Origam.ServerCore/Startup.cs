﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Origam.Workbench.Services;

namespace Origam.ServerCore
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<SessionObjects, SessionObjects>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Jwt";
                options.DefaultChallengeScheme = "Jwt";
            }).AddJwtBearer("Jwt", options =>
            {
                string securityKey = Configuration["SecurityKey"];
                if (string.IsNullOrWhiteSpace(securityKey))
                {
                    throw new ArgumentException("SecurityKey was not found in configuration. Please add it to appsettings.json");
                }

                if (securityKey.Length < 16)
                {
                    throw new ArgumentException("SecurityKey found in appsettings.json has to be at least 16 characters long!");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,  //ValidAudience = "the audience you want to validate",
                    ValidateIssuer = false,  //ValidIssuer = "the user you want to validate",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5) // 5 minute tolerance for the expiration date
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddLog4Net();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseMiddleware<CustomApiRedirect>();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.Use((context, next) =>
            {
                Thread.CurrentPrincipal =  context.User;               
                return next();
            });
            app.UseMvc();

            OrigamEngine.OrigamEngine.ConnectRuntime();
        }
    }
}
