using Instancium.TagKit.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();


// These services are required for Instancium Core to function properly:
builder.Services.AddHttpContextAccessor();    // Enables access to HttpContext in components and services
builder.Services.AddLocalization();           // Supports localization/internationalization of UI content
builder.Services.AddControllersWithViews();   // Enables MVC + Razor view rendering for component endpoints

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
app.MapStaticAssets();                       // Maps /instancium/resources/* for script/style delivery
app.MapDefaultControllerRoute();             // Enables default MVC routing for controllers and views


app.UseAuthorization();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
