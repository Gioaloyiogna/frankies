//using AhercodeWebAPI.HR.Data;
using AhercodeWebAPI.HR.UserModels;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Text;

namespace AhercodeWebAPI.HR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //////// this method gets called by the runtime. use this method to add services to the container.


        public void ConfigureServices(IServiceCollection services)
        {
            var userConnection = Configuration.GetConnectionString("UserDbConnection");
            var appConnection = Configuration.GetConnectionString("DefaultConnection");


            services.AddDbContext<FrankiesHotelContext>(options =>
                options.UseSqlServer(appConnection)
                );
            services.AddDbContext<UserDBContext>(options => options.UseSqlServer(userConnection));

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<UserDBContext>();
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
                
                )
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddControllers();
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AhercodeWebAPI-HR", Version = "v1" });
            });

            //services.AddMultiTenant<TenantInfo>()
            //    .WithHeaderStrategy("tenant")
            //    .WithEFCoreStore<ENP_HRPR_DBContext, TenantInfo>(OptionsBuilderConfigurationExtensions )
            //    );

            services.AddMultiTenant<TenantInfo>()
                .WithHeaderStrategy("tenant")
                .WithInMemoryStore(options =>
                {
                    options.Tenants.Add(new TenantInfo { Id = "1", Identifier = "Apple" });
                    options.Tenants.Add(new TenantInfo { Id = "2", Identifier = "Samsung" });
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AhercodeWebAPI.HR v1"));
            //}

            //app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "Ahercode.Api v1"));

            app.UseHttpsRedirection();


            app.UseCors(options=> {

                string devUrl = "http://localhost:3000";
                string devUrlBack = "http://localhost:3001";
                string productionEndUrl = "http://173.248.129.77,1439/";
                string devUrlBack_Frankies = "http://localhost";
                string productionEndUrl_Frankies = "http://3.74.54.13/,1439/";
                string url_Frankies = "http://3.74.54.13";

                options.WithOrigins(devUrl, productionEndUrl, devUrlBack, devUrlBack_Frankies,productionEndUrl_Frankies,url_Frankies)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
            

            app.UseRouting();

            app.UseMultiTenant();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
