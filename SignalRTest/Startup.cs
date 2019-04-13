using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SignalRTest
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddSignalR();
        }
         
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            app.UseDefaultFiles().UseStaticFiles(); 
            app.UseSignalR(router => {
                router.MapHub<HubHelper>("/game");
            });
        }
    }
}
