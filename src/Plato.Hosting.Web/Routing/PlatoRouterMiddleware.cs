﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Plato.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Settings;

namespace Plato.Hosting.Web.Routing
{
    public class PlatoRouterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly Dictionary<string, RequestDelegate> _pipelines = new Dictionary<string, RequestDelegate>();

        public PlatoRouterMiddleware(
            RequestDelegate next,
            ILogger<PlatoRouterMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Begin Routing Request");
            }

            var shellSettings = (ShellSettings)httpContext.Features[typeof(ShellSettings)];

            RequestDelegate pipeline;

            if (!_pipelines.TryGetValue(shellSettings.Name, out pipeline))
            {
                // Building a pipeline can't be done by two requests
                lock (_pipelines)
                {
                    if (!_pipelines.TryGetValue(shellSettings.Name, out pipeline))
                    {
                        pipeline = BuildTenantPipeline(shellSettings, httpContext.RequestServices);

                        if (shellSettings.State == Shell.Models.TenantState.Running)
                        {
                            // TODO: Invalidate the pipeline automatically when the shell context is changed
                            // such that we can reload the middlewares and the routes. Implement something similar
                            // to IRunningShellTable but for the pipelines.

                            _pipelines.Add(shellSettings.Name, pipeline);
                        }
                    }
                }
            }

            await pipeline.Invoke(httpContext);
        }

        // Build the middleware pipeline for the current tenant
        public RequestDelegate BuildTenantPipeline(ShellSettings shellSettings, IServiceProvider serviceProvider)
        {
            var startups = serviceProvider.GetServices<IStartup>();
            var inlineConstraintResolver = serviceProvider.GetService<IInlineConstraintResolver>();

            IApplicationBuilder appBuilder = new ApplicationBuilder(serviceProvider);

            string routePrefix = "";
            if (!string.IsNullOrWhiteSpace(shellSettings.RequestUrlPrefix))
            {
                routePrefix = shellSettings.RequestUrlPrefix + "/";
            }

            var routeBuilder = new RouteBuilder(appBuilder)
            {
                DefaultHandler = serviceProvider.GetRequiredService<MvcRouteHandler>()
            };

            var prefixedRouteBuilder = new PrefixedRouteBuilder(routePrefix, routeBuilder, inlineConstraintResolver);

            // Register one top level TenantRoute per tenant. Each instance contains all the routes
            // for this tenant.

            // In the case of several tenants, they will all be checked by ShellSettings. To optimize
            // the TenantRoute resolution we can create a single Router type that would index the
            // TenantRoute object by their ShellSetting. This way there would just be one lookup.
            // And the ShellSettings test in TenantRoute would also be useless.

            foreach (var startup in startups)
            {
                startup.Configure(appBuilder, prefixedRouteBuilder, serviceProvider);
            }


            // The default route is added to each tenant as a template route, with a prefix
            prefixedRouteBuilder.Routes.Add(new Route(
                prefixedRouteBuilder.DefaultHandler,
                "Default",
                "{area:exists}/{controller}/{action}/{id?}",
                null,
                null,
                null,
                inlineConstraintResolver)
            );

            var siteService = routeBuilder.ServiceProvider.GetService<ISiteService>();

            // ISiteService might not be registered during Setup
            if (siteService != null)
            {
                // Add home page route
                routeBuilder.Routes.Add(new HomePageRoute(shellSettings.RequestUrlPrefix, siteService, routeBuilder, inlineConstraintResolver));
            }

            var router = prefixedRouteBuilder.Build();

            appBuilder.UseRouter(router);

            var pipeline = appBuilder.Build();

            return pipeline;
        }
    }
}
