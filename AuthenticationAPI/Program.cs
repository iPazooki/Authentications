using AuthenticationAPI.Interfaces;
using AuthenticationAPI.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "Clean Architecture", Version = "v1" });

    // Add JWT Authentication to Swagger
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, []
                }
            });
});

builder.Services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // The default authentication scheme is JwtBearerDefaults.AuthenticationScheme, where all requests are directed to this scheme before redirecting to the appropriate authentication method.
    .AddPolicyScheme(JwtBearerDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer", string.Empty).Trim();
            if (!string.IsNullOrEmpty(token) && jwtHandler.CanReadToken(token))
            {
                var tokenIssuer = jwtHandler.ReadJwtToken(token).Issuer;

                if (tokenIssuer == builder.Configuration["Jwt:Issuer"])
                {
                    return "Local_JWT_Scheme";
                }
                
                return "AzureAD_Scheme";
            }

            return "CustomToken";
        };
    })
    // Default Azure AD authentication
    .AddJwtBearer("AzureAD_Scheme", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AzureAD:Issuer"],
            ValidAudience = builder.Configuration["AzureAD:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["AzureAD:SecretKey"])),
            ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero to prevent token expiration delay
        };
    })
    .AddJwtBearer("Local_JWT_Scheme", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero to prevent token expiration delay
        };
    })
    // Custom Authentication handler
    .AddScheme<ApiAuthenticationHandlerOptions, ApiAuthenticationHandler>("CustomToken", options =>
    {
        // Configure any custom options here if needed
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
