using Microsoft.EntityFrameworkCore;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Application.Interfaces.ServiceInterfaces;
using StealCatsImage.Application.Services;
using StealCatsImage.Infrastructure.Clients.CatApiClient;
using StealCatsImage.Infrastructure.Data;
using StealCatsImage.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICatRepository, CatRepository>();
builder.Services.AddScoped<ICatService, CatService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddHttpClient("TheCatApi", http =>
{
    http.BaseAddress = new Uri(builder.Configuration["TheCatApi:BaseUrl"]!);
    http.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["TheCatApi:ApiKey"]!);
});

builder.Services.AddScoped<ICatApiClient, CatApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
