using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepoDb;
using Serilog;
using Wasteless.Data;
using Wasteless.Infrastructure;
using Wasteless.Models;

namespace Wasteless
{
    public class Startup
    {
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;

            SqlServerBootstrap.Initialize();
            MapperDefinitions.Setup();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            if (_env.IsProduction())
            {
                var bytes = File.ReadAllBytes($"/var/ssl/private/{Configuration["WEBSITE_LOAD_CERTIFICATES"]}.p12");
                var certificate = new X509Certificate2(bytes);

                services.AddIdentityServer()
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddSigningCredential(certificate)
                    .AddOperationalStore<ApplicationDbContext>()
                    .AddIdentityResources()
                    .AddApiResources()
                    .AddClients()
                    .AddProfileService<ProfileService>();
            }
            else
            {
                services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                    .AddProfileService<ProfileService>();
            }


            services.AddAuthentication()
                .AddIdentityServerJwt();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (_env.IsDevelopment())
                services.AddSingleton<ICorsPolicyService>(container =>
                {
                    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
                    return new DefaultCorsPolicyService(logger)
                    {
                        AllowAll = true
                    };
                });

            services.AddHttpContextAccessor();

            services.AddSingleton<WasteService>();
            services.AddScoped<UserService>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();


            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseReactDevelopmentServer(npmScript: "start");
            });

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var _ = roleManager.CreateAsync(new IdentityRole("Admin")).Result;
            }

            var adminUser = userManager.FindByNameAsync("Admin").Result;
            if (adminUser == null)
            {
                adminUser = new ApplicationUser {UserName = "Admin", Email = "Admin"};
                var _ = userManager.CreateAsync(adminUser, Configuration["AdminPassword"]).Result;
            }

            if (!userManager.IsInRoleAsync(adminUser, "Admin").Result)
            {
                var _ = userManager.AddToRoleAsync(adminUser, "Admin").Result;
            }
        }
    }

    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = user != null;
        }
    }
}