using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using myDemo.Fliters;
using myDemo.Models;
using myDemo.Services.EmailMessageService;
using ORMEFCoreDA.Models;

namespace myDemo
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(Options => Options.UseSqlServer(_config.GetConnectionString("MyConnStr"), b => b.MigrationsAssembly("myDemo")));
          
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequiredUniqueChars = 3;
                opt.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
           
            services.AddMvc(Config =>
            {
                var Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                Config.Filters.Add(new AuthorizeFilter(Policy));
            });
           
            services.AddScoped<IEmpRepo, SqlEmpRepo>();
            services.AddScoped<IEmpRepo, EmpRepo>(); 


            services.AddAuthorization(options =>
            {
                 
                 options.AddPolicy("EditRolePolicy", policy =>
                   policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                 //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
                 //context.User.IsInRole("Administration") &&
                 //context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                 //context.User.IsInRole("Super Admin")));

                 //options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "true").RequireRole("Administration"));

                 options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true").RequireClaim("Edit Role"));

                 options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Administration"));

                 options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Administration", "User", "Manager"));
             });

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "1056649214476-hkad0g8rqiuq8dasr2pbgtp225auksqs.apps.googleusercontent.com";
                options.ClientSecret = "ZXNRipgNtAOUxFJkm_ezctQg";
            }).AddFacebook(options =>
            {
                options.ClientId = "2493402844210843";
                options.ClientSecret = "1288afe275863d704a133e40b1f435b3";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
           
            services.AddSingleton<IAuthorizationHandler,CanEditOnlyOtherAdminRolesAndClaimsHandler>();
          
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            services.AddSingleton<IEmailConfiguration>(_config.GetSection("EmailConfiguration").Get<EmailConfiguration>());

            services.AddSingleton<IEmailService, EmailService>();

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
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePages();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }

            app.UseStaticFiles();
            //app.UseMvc();
            app.UseAuthentication();

            app.UseMvc(mapRoute =>
            {
                mapRoute.MapRoute(" defualt ", "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(env.EnvironmentName);
            //});
        }
    }
}
