using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

using angPOC.Data;
using angPOC.Services;
using angPOC.Utilities;

namespace angPOC
{
    public class Startup
    {

        public Startup(IConfiguration configuration) => Configuration = configuration;


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //set up  anti-forgery header
            services.AddAntiforgery(options => options.HeaderName = Constants.AntiForgery.Header);

            services.AddMvc();
            services.AddSpaStaticFiles(c =>
            {
                c.RootPath = "ClientApp/dist";
            });
  

            services.AddControllersWithViews()
                 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddDbContext<UserDBContext>(options =>
                                options.UseSqlServer(Configuration.GetConnectionString("UserDB")));

            services.AddControllers().AddNewtonsoftJson();   //@  patch request(s)

            //set up mappinggs for all endponts - ref. classes @ Profiles folder
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed settings object jwt secret
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure scopes for application services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IPermissionRepository, PermisionRepository>();
            services.AddScoped<IStateRepository, StateRepository>();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddCors();
            services.AddControllers();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            // global exeption handler
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));


            app.UseHttpsRedirection();
            app.UseCors(builder =>
                 builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());


            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

       
            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.UseAngularCliServer(npmScript: "start");
                }
                else
                {
                    spa.Options.SourcePath = "ClientApp/dist";
                }
            });
        }
    }
}
