using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API
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
      // Add JWT from 'JwtConfig' section of appsettings.json (make secrets available to the application)
      services.Configure<JwtConfiguration>(Configuration.GetSection("JwtConfig"));

      // Add SQL server with Entity Framework using DefaultConnection from appsettings.json
      services.AddDbContext<PhenomenologicalStudyContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      // Add Identity
      services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<PhenomenologicalStudyContext>()
                .AddDefaultTokenProviders();

      // Add Authentication (with JWT Bearer) 
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
        // Add JWT Bearer using AuthenticationBuilder returned from AddAuthentication()
        .AddJwtBearer(options =>
        {
          options.SaveToken = true;
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidateIssuerSigningKey = true,  // Validates 3rd part of JWT token (encrypted part) generated from secret in JwtConfig 
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = false,
            ValidAudience = Configuration["JwtConfig:ValidAudience"],
            ValidIssuer = Configuration["JwtConfig:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtConfig:Secret"]))  // Encrypts JWT tokens
          };
        });

      // Add JWT refresh service
      services.AddSingleton(new TokenValidationParameters()
      {
        ValidateIssuerSigningKey = true,  // Validates 3rd part of JWT token (encrypted part) generated from secret in JwtConfig 
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = false,
        ValidAudience = Configuration["JwtConfig:ValidAudience"],
        ValidIssuer = Configuration["JwtConfig:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtConfig:Secret"]))  // Encrypts JWT tokens
      });

      // Included with project scaffolding
      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhenomenologicalStudy.API", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhenomenologicalStudy.API v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
