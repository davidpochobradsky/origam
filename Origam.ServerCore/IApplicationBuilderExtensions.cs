﻿using System.Linq;
using System.ServiceModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Origam.ServerCore.Configuration;
using Origam.ServerCore.Middleware;
using SoapCore;

namespace Origam.ServerCore
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseCustomSpa(this IApplicationBuilder app, string pathToClientApp)
        {
   
            app.Use((context, next) =>
            {
                if (context.GetEndpoint() != null)
                {
                    return next();
                }
                if (context.Request.Path == "/")
                {
                    context.Request.Path = "/index.html";
                }
                return next();
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(pathToClientApp)
            });
        }  
        
        public static void UseUserApi(this IApplicationBuilder app, StartUpConfiguration startUpConfiguration)
        {
            app.MapWhen(
                context => IsPublicUserApiRoute(startUpConfiguration, context),
                apiBranch => {
                    apiBranch.UseResponseBuffering();
                    apiBranch.UseMiddleware<UserApiMiddleWare>();
                });
            app.MapWhen(
                context => IsRestrictedUserApiRoute(startUpConfiguration, context), 
                apiBranch =>
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
        } 
        
        private static bool IsRestrictedUserApiRoute(
            StartUpConfiguration startUpConfiguration, HttpContext context)
        {
            return startUpConfiguration
                .UserApiRestrictedRoutes
                .Any(route => context.Request.Path.ToString().StartsWith(route));
        }
        private static bool IsPublicUserApiRoute(
            StartUpConfiguration startUpConfiguration, HttpContext context)
        {
            return startUpConfiguration
                .UserApiPublicRoutes
                .Any(route => context.Request.Path.ToString().StartsWith(route));
        }
        
        public static void UseSoapApi(this IApplicationBuilder app,
            bool authenticationRequired)
        {
            app.MapWhen(IsSoapApiRoute, apiBranch =>
            {
                apiBranch.Use(async (context, next) =>
                {
                    // Authentication middleware doesn't short-circuit the request itself
                    // we must do that here.
                    if (authenticationRequired && !context.User.Identity.IsAuthenticated)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    await next.Invoke();
                });
                apiBranch.UseMiddleware<SoapNamespaceReplacerMiddleware>();
                apiBranch.UseSoapEndpoint<DataServiceSoap>("/soap/DataService", new BasicHttpBinding());
                apiBranch.UseSoapEndpoint<WorkflowServiceSoap>("/soap/WorkflowService", new BasicHttpBinding());
            });
        }
        
        private static bool IsSoapApiRoute(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith("/soap");
        }
    }
}