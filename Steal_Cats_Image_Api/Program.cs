using Microsoft.EntityFrameworkCore;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Application.Interfaces.ServiceInterfaces;
using StealCatsImage.Application.Services;
using StealCatsImage.Infrastructure.Clients.CatApiClient;
using StealCatsImage.Infrastructure.Data;
using StealCatsImage.Infrastructure.Repositories;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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

builder.Services.AddHangfire(configuration => configuration
.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
