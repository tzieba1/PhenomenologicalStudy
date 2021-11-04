using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhenomenologicalStudy.API.Authorization.Handlers;
using PhenomenologicalStudy.API.Authorization.Requirements;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Middleware;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Services;
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
      // Needed to set up email sender service
      services.AddTransient<IEmailSender, EmailSender>();
      services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSenderOptions"));

      // Needed to access secrets from Azure Key Vault
      services.AddSingleton(Configuration);

      // Authorization policies added using custom requirements from PhenomenologicalStudy.API.Authorization.Requirements namespace
      services.AddAuthorization(OptionsBuilderConfigurationExtensions =>
      {
        OptionsBuilderConfigurationExtensions.AddPolicy("ExamplePermissionPolicy", p => 
          { 
            p.RequireRole("Admin");
            p.Requirements.Add(new ExamplePermissionRequirement("grant"));
          });
      });

      // Needed to handle authorization requirements added to authorization policies above
      services.AddSingleton<IAuthorizationHandler, ExamplePermissionRequirementHandler>();

      // Needed to create DbContext interface as dependency injectable service for adding/saving to database within middleware invokation.
      services.AddScoped<IPhenomenologicalStudyContext, PhenomenologicalStudyContext>();

      // Disable the default behaviour where invalid model state is automatically thrown because it is handled with middleware
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.SuppressModelStateInvalidFilter = true;
      });

      // Add JWT from 'JwtConfig' section of configuration (secret)
      services.Configure<JwtConfiguration>(Configuration.GetSection("JwtConfig"));

      // Add SQL server with Entity Framework using DefaultConnection from appsettings.json
      services.AddDbContext<PhenomenologicalStudyContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      // Add Identity
      services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<PhenomenologicalStudyContext>()
                .AddDefaultTokenProviders();

      // Instantiate JWT validation parameters here to ensure single issuer signing key for token validation
      var jwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfiguration>();
      TokenValidationParameters tokenValidationParams = new()
      {
        ValidateIssuerSigningKey = true,  // Validates 3rd part of JWT token (encrypted part) generated from secret in JwtConfig 
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = false,
        ValidAudience = jwtConfig.ValidAudience,
        ValidIssuer = jwtConfig.ValidIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.PrivateKey))  // Encrypts JWT tokens
      };

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
          options.Events = new JwtBearerEvents
          {
            OnTokenValidated = context =>
            {
              //TODO: Add claims here
              return Task.CompletedTask;
            }
          };
          options.SaveToken = true;
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = tokenValidationParams;
        });

      // Add JWT refresh service
      services.AddSingleton(tokenValidationParams);

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
      else
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      // Add custom middleware which checks accept headers and errors
      app.UseMiddleware<ErrorMessageMiddleware>();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
