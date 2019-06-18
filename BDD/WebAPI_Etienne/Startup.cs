using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CapteursApi.Services;
using MongoDB.Driver;
using System.Collections.Generic;
using CapteursApi.Models;

namespace WebAPI
{
    public class Startup
    {
        private readonly CapteurService _capteurService;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<CapteurService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var client = new MongoClient("mongodb://localhost:27017");
            var _database = client.GetDatabase("MurVegetalDb");

            //app.Use(async (context, next) =>
        //{
            //Console.WriteLine(_database.GetCollection<ICollectionModel>("Users").Find("{ \"username\" : \"" + "toto" + "\"}").ToList()[0] );     
            //await next.Invoke();
            // Do logging or other work that doesn't write to the Response.
        //});

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
