using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhenomenologicalStudy.API.Authorization.Handlers;
using PhenomenologicalStudy.API.Authorization.Requirements;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Services;
using PhenomenologicalStudy.API.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
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
      services.AddCors();
      services.AddTransient<IEmailSender, EmailSender>(); // Needed to set up email sender service
      services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSenderOptions"));

      services.AddAutoMapper(typeof(Startup));            // Map DTO models and database models
      services.AddScoped<IAuthService, AuthService>();    // JWT authentication with email confirmation
      services.AddScoped<IReflectionService, ReflectionService>();    // Main collection service
      services.AddScoped<IEmotionService, EmotionService>();          
      services.AddScoped<IUserService, UserService>();    
      services.AddScoped<IChildService, ChildService>();    
      services.AddScoped<IQuestionnaireService, QuestionnaireService>();    
      services.AddScoped<IBadgeService, BadgeService>();    

      services.AddSingleton(Configuration); // Needed to access secrets from Azure Key Vault
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Retrieve request data from HTTP context (such as claims: NameIdentifier -> User.Id)

      // Add JWT from 'JwtConfig' section of configuration (secret)
      services.Configure<JwtConfiguration>(Configuration.GetSection("JwtConfig"));

      // EXAMPLE: Authorization policies added using custom requirements from PhenomenologicalStudy.API.Authorization.Requirements namespace
      services.AddAuthorization(options =>
      {
        options.AddPolicy("Admin",
            authBuilder =>
            {
              authBuilder.RequireRole("Admin");
            });

        options.AddPolicy("Participant",
            authBuilder =>
            {
              authBuilder.RequireRole("Participant");
            });
        options.AddPolicy("ExamplePermissionPolicy", p =>
          {
            p.RequireRole("Admin");
            p.Requirements.Add(new ExamplePermissionRequirement("grant"));
          });
      });

      // EXAMPLE: Needed to handle authorization requirements added to authorization policies above
      services.AddSingleton<IAuthorizationHandler, ExamplePermissionRequirementHandler>();

      // Needed to create DbContext interface as dependency injectable service for adding/saving to database within middleware invokation.
      //services.AddScoped<IPhenomenologicalStudyContext, PhenomenologicalStudyContext>();

      // Disable the default behaviour where invalid model state is automatically thrown because it is handled with middleware
      //services.Configure<ApiBehaviorOptions>(options =>
      //{
      //  options.SuppressModelStateInvalidFilter = true;
      //});

      // Add SQL server with Entity Framework using DefaultConnection from appsettings.json
      services.AddDbContext<PhenomenologicalStudyContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      // Add Identity
      services.AddIdentity<User, Role>(options =>
                  {
                    options.SignIn.RequireConfirmedAccount = true;
                    //options.Stores.ProtectPersonalData = true;                  // Enable use of ProtectedPersonalDataAnnotation
                  })
                .AddEntityFrameworkStores<PhenomenologicalStudyContext>()
                //.AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()  // For adding custom claims
                .AddDefaultTokenProviders();                                    // Used to generate and verify tokens for API actions performed by various actors (users) with various claims
      //.AddTokenProvider<CreateReflectionTokenProvider>("CreateReflectionTokenProvider");

      // Protected data requires these 3 classes extended from Identity interfaces (implemented in namespace Authorization.Extensions)
      //services.AddScoped<ILookupProtectorKeyRing, KeyRing>();
      //services.AddScoped<ILookupProtector, LookupProtector>();
      //services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();

      // Instantiate JWT validation parameters here to ensure single issuer signing key for token validation
      var jwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfiguration>();
      TokenValidationParameters tokenValidationParams = new()
      {
        ValidateIssuerSigningKey = true,  // Validates 3rd part of JWT token (encrypted part) generated from secret in JwtConfig 
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = true,
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
      
      services.AddSingleton(tokenValidationParams); // Add JWT refresh service

      services.AddControllers() // Configures common services for controllers
        // Next line prevents an internal server error where object depth exceeds 32 or an object cycle occurs
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

      // OpenAPI documentation configuration with swagger docs
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhenomenologicalStudy.API", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
          Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {Value}\"",
          In = ParameterLocation.Header,
          Name = "Authorization", // Name of actual header
          Type = SecuritySchemeType.ApiKey
        });
        c.OperationFilter<SecurityRequirementsOperationFilter>();
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

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhenomenologicalStudy.API v1"));
      
      app.UseHttpsRedirection();
      app.UseRouting();

      // Allows cross-origin requests (needed for communicating with this API)
      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

      app.UseAuthentication();
      app.UseAuthorization();

      // Add custom middleware which checks accept headers and errors
      //app.UseMiddleware<ErrorMessageMiddleware>();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
