using System.Text;
using Angular_Auth.Models;
using Angular_Auth.Repositories;
using Angular_Auth.Services;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddLogging()
    .AddTransient<IAuthenticationService, AuthenticationService>()
    .AddTransient<IUserService, UserService>()
    .AddTransient<IRoleService, RoleService>()
    .AddTransient<IBlogService, BlogService>()
    .AddTransient<IProfileService, ProfileService>()
    .AddTransient<IMailService, MailService>()
    .AddTransient<BlogRepository>()
    .AddTransient<ProfileRepository>()
    ;

builder.Services.AddControllers();
builder.Services.AddLogging();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("blogger"));
    options.UseOpenIddict();
});

// Identity
builder.Services.AddIdentity<User, IdentityRole>(options => {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_#";
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    // Adding Jwt Bearer
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    var secret = configuration["JWT:Secret"];
    if (secret is null) throw new ArgumentException("JWT:Secret is not configured.");

    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
    };

    options.Events = new JwtBearerEvents {
        OnMessageReceived = ctx => {
            if (ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken)) {
                ctx.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});


builder.Services.AddOpenApi();
// Swagger Gen
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Angular Blogs API", Version = "v1", });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = """
                      ## JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.

                      **Example:** 'Bearer xxxxxxx'
                      """,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
    });
});

// Add CORS policy
builder.Services.AddCors();

builder.Services.AddOpenIddict()
    .AddClient(options => {
        // Allow the OpenIddict client to negotiate the authorization code flow.
        options.AllowAuthorizationCodeFlow();

        // Register the signing and encryption credentials used to protect
        // sensitive data like the state tokens produced by OpenIddict.
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options.UseAspNetCore()
            .EnableRedirectionEndpointPassthrough();

        // Register the GitHub integration.
        options.UseWebProviders()
            .AddGitHub(githubOptions => {
                githubOptions.SetClientId("Ov23libmQBQcEuG5LIat")
                    .SetClientSecret("743f52005c48c3be78d0ea0093c131b63ef11972")
                    .SetRedirectUri("/api/v1/auth/callback/login/github");
            });
    }).AddCore(options => {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        options.UseEntityFrameworkCore().UseDbContext<AppDbContext>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// Use CORS
app.UseCors(b => b
    .WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost:5175", "https://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
app.UseHttpsRedirection();

//  Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();