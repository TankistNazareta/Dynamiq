using AutoMapper;
using Dynamiq.API.Middlewares;
using Dynamiq.Application;
using Dynamiq.Application.Commands.EmailVerifications.Commands;
using Dynamiq.Application.Commands.EmailVerifications.Validators;
using Dynamiq.Application.DTOs.AuthDTOs;
using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Application.Interfaces.UseCases;
using Dynamiq.Application.UseCases;
using Dynamiq.Domain.Interfaces;
using Dynamiq.Infrastructure.BackgroundServices;
using Dynamiq.Infrastructure.Persistence.Context;
using Dynamiq.Infrastructure.Repositories;
using Dynamiq.Infrastructure.Services;
using Dynamiq.Infrastructure.Services.Auth;
using Dynamiq.Infrastructure.Services.Stripe;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add JWT

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new()
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddHttpClient("google-oauth", c =>
{
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddAuthentication();

//Add logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//Add Fluent validation
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ConfirmEmailByTokenCommandValidator>();

//AddRateLimiter
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("CreateCheckoutLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(3);
        limiterOptions.QueueLimit = 0;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("LogInLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("SignUpLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(2);
        limiterOptions.QueueLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        var response = new
        {
            error = "Too many requests, please wait some time"
        };

        var json = JsonSerializer.Serialize(response);
        await context.HttpContext.Response.WriteAsync(json, cancellationToken: ct);
    };
});

//Add Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

//AddressOptionsptions
builder.Services.Configure<GoogleOAuthOptions>(builder.Configuration.GetSection("GoogleOAuth"));

//AddMediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<ConfirmEmailByTokenCommand>();
});

//Add mapper
var mapperConfig = MapperConfig.RegisterMaps();
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//Add dbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionToLocalDb"));
});

//Repo
builder.Services.AddTransient<IUserRepo, UserRepo>();
builder.Services.AddTransient<IProductRepo, ProductRepo>();
builder.Services.AddTransient<IPaymentHistoryRepo, PaymentHistoryRepo>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<ICartRepo, CartRepo>();
builder.Services.AddTransient<ICouponRepo, CouponRepo>();

//Services
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
builder.Services.AddTransient<IGoogleOidcService, GoogleOidcService>();
builder.Services.AddTransient<ICouponService, CouponService>();

//UseCases
builder.Services.AddTransient<IUserCleanupUseCase, UserCleanupUseCase>();

//API.Stripe
builder.Services.AddTransient<IStripeCheckoutSession, StripeCheckoutSession>();
builder.Services.AddTransient<IStripeProductService, StripeProductService>();
builder.Services.AddTransient<IStripeWebhookParser, StripeWebhookParser>();
builder.Services.AddTransient<IStripeCouponService, StripeCouponService>();

builder.Services.AddHttpContextAccessor();

//Add background services
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<UserCleanupService>();
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    //app.UseRateLimiter();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }