using LibraryApp.Domain.Constants;
using LibraryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Services;
using LibraryApp.Application.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using LibraryApp.Application.Validation;
using LibraryApp.Api.Middlewares;
using LibraryApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "FrontendCors";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \n\nEnter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtSecretKey = builder.Configuration["JwtOptions:SecretKey"] ?? "superSecretKey_veryLongForHmacSha256_whichIsRequired";
var jwtIssuer = builder.Configuration["JwtOptions:Issuer"] ?? "LibraryApp";
var jwtAudience = builder.Configuration["JwtOptions:Audience"] ?? "LibraryAppClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };
    });

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("LibraryDb");

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(connectionString));
    
builder.Services.AddScoped<AuthorsService>();
builder.Services.AddScoped<CategoriesService>();
builder.Services.AddScoped<BooksService>();
builder.Services.AddScoped<MembersService>();
builder.Services.AddScoped<LoansService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddAutoMapper(typeof(LibraryApp.Application.Common.MappingProfile).Assembly);


builder.Services.AddScoped<ILibraryDb, LibraryDbContext>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
    
    // Seed Admin user at runtime with properly hashed password
    var adminEmail = "admin@library.com";
    var adminExists = db.Members.IgnoreQueryFilters().Any(m => m.Email == adminEmail);
    if (adminExists)
    {
        var admin = db.Members.IgnoreQueryFilters().First(m => m.Email == adminEmail);
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        admin.PasswordHash = hasher.Hash("admin123");
        admin.Role = Roles.Admin;
        db.SaveChanges();
    }
}
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseCors(FrontendCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("HealthCheck");

app.MapGet("/dev/db-check", async (LibraryDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    var authors = await db.Authors.CountAsync();
    var categories = await db.Categories.CountAsync();
    var books = await db.Books.CountAsync();
    var members = await db.Members.CountAsync();
    var loans = await db.Loans.CountAsync();

    return Results.Ok(new { canConnect, authors, categories, books, members, loans });
})
.WithName("DbCheck");


app.Run();

