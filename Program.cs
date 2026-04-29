using Microsoft.EntityFrameworkCore;
using SosDog.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. ADICIONANDO AS FERRAMENTAS (SERVICES) ANTES DO BUILD
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("ConexaoPadrao");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- LIGANDO A SEGURANÇA (COOKIES) AQUI EM CIMA! ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Se alguém sem crachá tentar entrar no Reportar, mande-o para esta tela:
        options.LoginPath = "/Usuarios/Login"; 
    });

// 2. AGORA SIM, FECHAMOS A CAIXA E CONSTRUÍMOS O APP
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 3. O "SEGURANÇA" NA PORTA (A ordem aqui está perfeita agora)
app.UseAuthentication(); // O Segurança que olha o crachá
app.UseAuthorization();  // O Gerente que vê se você tem permissão

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();