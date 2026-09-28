using ObiletCase.Services.Extensions;
using ObiletCase.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Yapılandırması (appsettings.json ayarlarını okur)
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());


// Servis Kayıtları
builder.Services.AddControllersWithViews();


// DI Container
builder.Services.AddServiceExtensionsDIContainer();
builder.Services.AddWebExtensionsDIContainer(builder.Configuration);



builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 2. HTTP İstek Loglaması (Gelen tüm istekleri ve yanıt sürelerini loglar)
app.UseSerilogRequestLogging();

// Pipeline Yapılandırması
// GlobalExceptionHandler'ın çalışması için bu middleware şarttır:
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
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