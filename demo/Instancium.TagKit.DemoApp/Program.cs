using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();


// These services are required for Instancium Core to function properly:
builder.Services.AddHttpContextAccessor();    // Enables access to HttpContext in components and services
builder.Services.AddControllersWithViews();   // Enables MVC + Razor view rendering for component endpoints

// Bind configuration section if present
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Instancium"));

// Register the adapter to expose IAppSettings
// IAppSettings is provided via AppSettingsAdapter,
// which safely exposes configuration values through DI
// and decouples internal logic from direct use of IOptions<T>.
builder.Services.AddSingleton<IAppSettings, AppSettingsAdapter>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();


// These middleware and route mappings are required for Instancium Core to operate correctly:
app.UseStaticFiles();                        // Serves static assets (scripts, styles, etc.)
app.UseMiddleware<TagKitHostMiddleware>();   // Enables server-side routing for TagKit component endpoints
app.MapDefaultControllerRoute();             // Enables default MVC routing for controllers and views


app.UseAuthorization();

app.MapRazorPages();

app.Run();
