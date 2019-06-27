using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Services;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Linq;


namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<WebService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Gestion CORS
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();

                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var client = new MongoClient( (string) LoadJson().ConnectionStrings.MurVegetalDb);
            var database = client.GetDatabase("MurVegetalDb");

            app.Use(async (context, next) =>
            {
                for(int i = 0; i < context.Request.Headers.Count; i++) {
                    if(context.Request.Headers.ElementAt(i).Key == "Authorization")
                    {
                        string base64auth = (string) context.Request.Headers.ElementAt(i).Value;
                        var base64EncodedBytes = System.Convert.FromBase64String(base64auth.Split(' ')[1]);
                        string[] credentials = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':');

                        var users = database.GetCollection<WebAPI.Models.UsersAPI>("UsersAPI").Find(x => x.Username == credentials[0]).ToList();
                        foreach(var user in users) {
                            // if the hash match, we can pass the request to the rest of middleware and end-ware
                            if(GenerateSaltedHash(credentials[1], user.Salt) == user.PasswordHash){
                                await next.Invoke();
                                return;
                            }   
                        }
                        break;
                    }
                }
                //if anything is wrong (no )
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Credentials \"Http Authentification Header\"");
            });

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseCors(MyAllowSpecificOrigins);
        }

        /* ///////////////////////////// JE STOCKE ICI PROVISOIREMENT LES MDP D'ACCES A L'API /////////////////////////////////
         * admin : iotdata2019!
         * interfaceWeb : Th3L0u15V1v13r
         * interfaceMobile : mdpInterfaceM0bile
         */

        public dynamic LoadJson()
        {
            using (StreamReader r = new StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject(json);
            }
        }

        static string GenerateSaltedHash(string plainText, string salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] plainTextWithSaltBytes = System.Text.Encoding.UTF8.GetBytes(plainText + salt);

            byte[] hash = algorithm.ComputeHash(plainTextWithSaltBytes);

            string hashString = string.Empty;
            foreach (byte x in hash)
            {
            hashString += String.Format("{0:x2}", x);
            }
            return hashString;

        }
    }
}
