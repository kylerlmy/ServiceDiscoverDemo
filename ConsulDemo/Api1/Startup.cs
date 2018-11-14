using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api1
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            lifetime.ApplicationStarted.Register(OnStart);
            lifetime.ApplicationStopped.Register(OnStopped);

            app.UseMvc();
        }


        private void OnStart()
        {
            var client = new ConsulClient();//users default host:port which is localhsot:8500
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = $"http://127.0.0.1:64850/HealthCheck"
            };

            var tcpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                TCP = $"127.0.0.1:{64850}"
            };

            var agentRegister = new AgentServiceRegistration()
            {
                ID = "servicename:64850",//Guid.NewGuid().ToString(),
                Check = httpCheck,
                //Checks = new[] { httpCheck, tcpCheck },
                Address = "127.0.0.1",
                Name = "servicename",
                Port = 64850
            };

            client.Agent.ServiceRegister(agentRegister).ConfigureAwait(false);
        }

        private void OnStopped()
        {
            var client = new ConsulClient();//users default host:port which is localhsot:8500
            client.Agent.ServiceDeregister("servicename: 64850");
        }

    }
}
