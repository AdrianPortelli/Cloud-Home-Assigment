using DataAccess.Interfaces;
using DataAccess.Repositories;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdrianCloudAssigment
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
            // requires
            // using Microsoft.AspNetCore.Authentication.Cookies;
            // using Microsoft.AspNetCore.Authentication.Google;
            // NuGet package Microsoft.AspNetCore.Authentication.Google

            string projectName = Configuration["project"];
            services.AddLogging(builder => builder.AddGoogle(new LoggingServiceOptions
            {
                // Replace ProjectId with your Google Cloud Project ID.
                ProjectId = projectName,
                // Replace Service with a name or identifier for the service.
                ServiceName = "AdrianCloudAssigment",
                // Replace Version with a version for the service.
                Version = "1"
            }));

            SecretManagerServiceClient client = SecretManagerServiceClient.Create();
            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(projectName, "secretinfo", "1");
            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);
            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();
            dynamic jsonData = JsonConvert.DeserializeObject(payload);
            string clientId = Convert.ToString(jsonData["Authentication:Google:ClientId"]);
            string clientSecret = Convert.ToString(jsonData["Authentication:Google:ClientSecret"]);
            string redis_connection = Convert.ToString(jsonData["redis_connectionstring"]);

            //switch them with the others and added the method to cloud function

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                });

            services.AddRazorPages();
            services.AddControllersWithViews();

            

            services.AddScoped<IFireStoreDataAccess, FireStoreDataAccess>(x =>
            {
                return new FireStoreDataAccess(projectName);
            });

            
            services.AddScoped<ICacheRepository, CacheRepository>(x =>
            {
                return new CacheRepository(redis_connection);
            });

            string topicName = Configuration["topic"];

            services.AddScoped<IPubSubRepository, PubSubRepository>(x => 
            {
                return new PubSubRepository(projectName, topicName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
