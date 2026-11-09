using AgroConecta.Infrastructure;
using AgroConecta.Infrastructure.Persistence;
using AgroConecta.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "A string de conexão 'Default' não foi configurada. Veja appsettings.json ou docker/docker-compose.yml.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var escopo = app.Services.CreateAsyncScope();
    var contexto = escopo.ServiceProvider.GetRequiredService<AgroConectaDbContext>();
    await contexto.Database.MigrateAsync();
    await SeedDados.AplicarAsync(contexto, app.Logger);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
