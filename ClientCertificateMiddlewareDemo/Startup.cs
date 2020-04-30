// <copyright file="Startup.cs" company="CWiz Software">
// Copyright (c) CWiz Software. All rights reserved.
// </copyright>

namespace ClientCertificateMiddlewareDemo
{
    using System;
    using System.Collections.Generic;
    using CWiz.ClientCertificateRoleBasedAccessControlMiddlewarej;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CertificateAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CertificateAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCertificateAuthentication(certOptions =>
            {
                var certificateAndRoles = new List<CertficateAuthenticationOptions.CertificateAndRoles>();
                Configuration.GetSection("AuthorizedCertficatesAndRoles:CertificateAndRoles").Bind(certificateAndRoles);
                certOptions.CertificatesAndRoles = certificateAndRoles.ToArray();
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAccessAdminMethods", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CanAccessUserMethods", policy => policy.RequireRole("User"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
