#region license
/*
Copyright 2005 - 2020 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Origam.Security.Common;
using Origam.Security.Identity;
using Origam.ServerCore.Authorization;
using Origam.ServerCore.Configuration;

namespace Origam.ServerCore
{
    public class Startup
    {
        private readonly StartUpConfiguration startUpConfiguration;
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            startUpConfiguration = new StartUpConfiguration(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = startUpConfiguration.PathToClientApp ?? ".";
            });
            services.AddScoped<IManager, CoreManagerAdapter>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<SessionObjects, SessionObjects>();
            services.AddTransient<IUserStore<IOrigamUser>, UserStore>();
            services.AddSingleton<IPasswordHasher<IOrigamUser>, CorePasswordHasher>();
            services.AddScoped<SignInManager<IOrigamUser>>();
            services.AddScoped<IUserClaimsPrincipalFactory<IOrigamUser>, UserClaimsPrincipalFactory<IOrigamUser>>();
            services.AddScoped<CoreUserManager>();
            services.AddScoped<UserManager<IOrigamUser>>(x =>
                x.GetRequiredService<CoreUserManager>());
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddIdentity<IOrigamUser, Role>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Jwt";
                options.DefaultChallengeScheme = "Jwt";
            }).AddJwtBearer("Jwt", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,  //ValidAudience = "the audience you want to validate",
                    ValidateIssuer = false,  //ValidIssuer = "the user you want to validate",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(startUpConfiguration.SecurityKey)),
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5) // 5 minute tolerance for the expiration date
                };
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(
                provider => provider.GetService<IHttpContextAccessor>().HttpContext?.User);
            services.Configure<UserConfig>(options => Configuration.GetSection("UserConfig").Bind(options));
        }

        public void Configure(
            IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
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
            app.MapWhen(IsPublicUserApiRoute, apiBranch => {
                apiBranch.UseResponseBuffering();
                apiBranch.UseMiddleware<UserApiMiddleWare>();
            });
            app.MapWhen(IsRestrictedUserApiRoute, apiBranch =>
            {
                apiBranch.UseAuthentication();
                apiBranch.Use(async (context, next) =>
                {
                    // Authentication middleware doesn't short-circuit the request itself
                    // we must do that here.
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    await next.Invoke();
                });
                apiBranch.UseResponseBuffering();
                apiBranch.UseMiddleware<UserApiMiddleWare>();
            });
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRequestLocalization();
            app.UseMvc();
            app.UseSpa(spa => {});
            // add DI to origam, in order to be able to resolve IPrincipal from
            // https://davidpine.net/blog/principal-architecture-changes/
            // https://docs.microsoft.com/cs-cz/aspnet/core/migration/claimsprincipal-current?view=aspnetcore-3.0
            SecurityManager.SetDIServiceProvider(app.ApplicationServices);
            OrigamEngine.OrigamEngine.ConnectRuntime();
        }
        private bool IsRestrictedUserApiRoute(HttpContext context)
        {
            return startUpConfiguration
                .UserApiRestrictedRoutes
                .Any(route => context.Request.Path.ToString().StartsWith(route));
        }
        private bool IsPublicUserApiRoute(HttpContext context)
        {
            return startUpConfiguration
                .UserApiPublicRoutes
                .Any(route => context.Request.Path.ToString().StartsWith(route));
        }
        private bool IsReportRoute(HttpContext context)
        {
            return context.Request.Path.ToString()
                .StartsWith("/internalApi/Report");
        }
    }
}
