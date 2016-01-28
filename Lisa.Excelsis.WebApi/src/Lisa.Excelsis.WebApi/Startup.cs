using Microsoft.AspNet.Authentication.JwtBearer;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using WebApiSample.Properties;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System;

namespace Lisa.Excelsis.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public static UserProfile Profile { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment app)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(app.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                   "CorsExcelsis",
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            app.UseIISPlatformHandler();
            app.UseStaticFiles();
            app.UseCors("CorsExcelsis");

            loggerfactory.AddConsole();

            var logger = loggerfactory.CreateLogger("Auth0");
            var settings = app.ApplicationServices.GetService<IOptions<Auth0Settings>>();

            app.UseJwtBearerAuthentication(options =>
            {
                options.Audience = settings.Value.ClientId;
                options.Authority = $"https://{settings.Value.Domain}";
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError("Authentication failed.", context.Exception);
                        return Task.FromResult(0);
                    },
                    OnValidatedToken = context =>
                    {
                        var claimsIdentity = context.AuthenticationTicket.Principal.Identity as ClaimsIdentity;
                        claimsIdentity.AddClaim(new Claim("id_token",
                            context.Request.Headers["Authorization"][0].Substring(context.AuthenticationTicket.AuthenticationScheme.Length + 1)));

                        // OPTIONAL: you can read/modify the claims that are populated based on the JWT
                        claimsIdentity.AddClaim(new Claim("Email", claimsIdentity.FindFirst("name").Value));

                        ToUserProfile(claimsIdentity);
                        return Task.FromResult(0);
                    }
                };
            });

            app.UseMvcWithDefaultRoute();
        }

        private void ToUserProfile(ClaimsIdentity claimsIdentity)
        {
            Type type = typeof(UserProfile);
            var obj = Activator.CreateInstance(type);
                       
            foreach (var claim in claimsIdentity.Claims)
            {
                if (type.GetProperty(FirstCharToUpper(claim.Type)) != null)
                {
                    type.GetProperty(FirstCharToUpper(claim.Type)).SetValue(obj, claim.Value.ToString());
                }
                else if (claim.Type == "user_metadata" || claim.Type == "app_metadata")
                {
                    JObject json = JObject.Parse("{" + claim.Value + "}");
                    foreach (var pair in json)
                    {
                        if (type.GetProperty(FirstCharToUpper(pair.Key)) != null)
                        {
                            Type valueType = pair.Value.GetType();
                            if (pair.Value.Type.ToString() == "Array")
                            {
                                string[] value = pair.Value.OfType<object>().Select(o => o.ToString()).ToArray();
                                type.GetProperty(FirstCharToUpper(pair.Key)).SetValue(obj, value);
                            }
                            else {
                                type.GetProperty(FirstCharToUpper(pair.Key)).SetValue(obj, pair.Value.ToObject<string>());
                            }
                        }
                    }
                }
            }
            Profile = (UserProfile)obj;
        }

        

        private static string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}