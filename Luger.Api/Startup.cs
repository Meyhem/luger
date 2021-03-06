using Luger.Api.Features.Configuration;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Luger.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();

            services.AddTransient<ILugerConfigurationProvider, LugerConfigurationProvider>();
            services.AddTransient<ILogService, LogService>();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var jwtSection = Configuration.GetSection("Jwt");
                jwtSection.Bind(jwt);

                var key = jwtSection.GetValue<string>("TokenValidationParameters:IssuerSigningKey");
                jwt.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                jwt.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });

            services.Configure<LoggingOptions>(Configuration.GetSection("Luger"));
            services.Configure<MongoOptions>(Configuration.GetSection("Luger:Mongo"));

            services.AddScoped(di =>
            {
                var opts = di.GetOptions<MongoOptions>();

                return new MongoClient(opts.Url);
            });

            services.AddScoped(di =>
            {
                var mc = di.GetRequiredService<MongoClient>();
                var opts = di.GetOptions<MongoOptions>();
                
                return mc.GetDatabase(opts.Database);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitDb(app.ApplicationServices).Wait();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DocumentTitle = "Luger API";
            });
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.Map("api/{**path}", delegate (HttpContext ctx)
                {
                    ctx.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });

                endpoints.MapFallbackToFile("index.html");
            });
        }

        private async Task InitDb(IServiceProvider di)
        {
            using var scope = di.CreateScope();
            di = scope.ServiceProvider;

            var service = di.GetRequiredService<ILogService>();
            await service.PrepareDatabase();
        }
    }
}