using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeskAlerts
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // если решу использовать  entity framework core
            //string connection = "Server=(localdb)\\mssqllocaldb;Database=rolestoredb;Trusted_Connection=True;";
            //services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
            var dataBase = new DBManager(@"DESKTOP-N1GQUIJ", "sa", "1234", "Desk_Alerts");
            //var  dataBase = new DBManager(@"DESKTOP-9S4HQ7H\SQLEXPRESS", "Desk_Alerts");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => { options.LoginPath = new PathString("/Account/Login"); });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware();  //1 for bild React.js - redux
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseWebSockets();
            app.UseFileServer();
            //app.UseMvc(routes => {
            //    routes.MapRoute(
            //        "default",
            //        "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapSpaFallbackRoute("spa-fallbak", new { controller = "Home", action = "Index" });
            //});

            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Home", action = "Index" });
                });
            });
        }
    }
}