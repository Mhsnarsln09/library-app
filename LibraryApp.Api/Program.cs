using LibraryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Services;
using LibraryApp.Application.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


var connectionString = builder.Configuration.GetConnectionString("LibraryDb");

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(connectionString));
    
builder.Services.AddScoped<AuthorsService>();


builder.Services.AddScoped<ILibraryDb, LibraryDbContext>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
}
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

