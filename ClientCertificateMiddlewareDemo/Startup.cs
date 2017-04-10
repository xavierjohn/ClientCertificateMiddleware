using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ClientCertificateMiddleware;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ClientCertificateMiddlewareDemo
{
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
            var certificateAndRoles = new List<AuthorizedCertficatesAndRoles.CertificateAndRoles>();
            Configuration.GetSection("AuthorizedCertficatesAndRoles:CertificateAndRoles").Bind(certificateAndRoles);

            services.Configure<AuthorizedCertficatesAndRoles>(x =>
            {
                x.CertificatesAndRoles = certificateAndRoles.ToArray();
            });
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseClientCertificateMiddleware();
            app.UseMvc();
        }
    }
}
