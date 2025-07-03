# Instancium Core

**Render protocol, not framework.**  
Instancium Core is a lightweight, render-agnostic runtime designed for component-based systems.  
It embraces server-side rendering and declarative rehydration — with no frontend frameworks, no ceremony.

You control markup. It brings it to life.

## ✨ What It Does

- Seamlessly reloads server-rendered components via `inst.reload(...)`
- Resolves scripts and styles as inline or external via configuration
- Enables lifecycle hooks like `el.api?.onInit()` for component activation
- Works with Razor, TagHelpers, static HTML, or custom build pipelines

### What is a Component in Instancium?

A component is a server-rendered interface unit that:

- **Has a unique ID and tag**
- **May include** HTML, CSS, JavaScript, and localization resources
- **Can be rendered once** (SSR) or updated dynamically using `inst.reload(...)`
- **Requires no frontend framework or global runtime**

### Component Structure

An Instancium component may consist of:

| Part             | Description                                      |
|------------------|--------------------------------------------------|
| `.cs`            | Component logic (e.g. `ComponentTagHelper : TagHelperBase`) |
| `.html`		   | Markup template (plain HTML)     |
| `.css`           | Optional styles, inline or external              |
| `.js`            | Optional behavior, scoped per component          |
| `.resx`          | Optional localization resources                  |

### Key Advantages

- **Minimal dependencies** — No SPA frameworks or complex tooling required  
- **Flexible asset loading** — Choose between inline or external scripts/styles using `UseResourceLink`  
- **No build pipeline required** — Component markup can be created in plain HTML without compiling the project  
- **Decoupled workflows** — Frontend developers can work independently, even outside .NET environments  
- **Reloadable** — Components can be updated dynamically at runtime with `inst.reload(...)` and rehydrated on demand


### Reload Behavior: SSR by Default, SPA When Desired

Instancium treats every component as **reloadable** — but only when you decide.  
Each component can behave as either:

- **A classic SSR fragment** — rendered once on initial page load
- **A dynamic, client-updated module** — reloaded at runtime via `inst.reload(...)`

### 🔧 Choose Reload Strategy per Component

- Use **server rendering** for static or non-interactive UI fragments  
- Use **SPA-style reload** (`inst.reload(...)`) for interactive zones, detail views, or partial updates  
- Set `UseResourceLink = false` to **inline styles/scripts** — ideal for email, previews, and static pages  
- Set `UseResourceLink = true` to **load resources externally** for shared caching and cleaner markup

📌 **You’re in control.** No hydration cliffs. No global runtime. No tight coupling.

## 🔁 Usage Examples: `inst.reload(...)`

Instancium provides a flexible runtime API to reload components dynamically.  
You can control how much context you pass — the system fills in the rest.

```js
// 🔹 Minimal reload — auto-detects tag and ID from the element
inst.reload("#test-tag");

// 🔹 Reload with parameters and mount control
inst.reload("#test-tag", {
  lang: "en"
}, false); // replaces markup only; skips script execution and lifecycle

```


This will:

- Fetch the latest HTML for the component
- Replace its DOM subtree
- Apply inline scripts and external styles (if `UseResourceLink` is `true`)
- Trigger `el.api?.onInit()` if defined



## ⚙️ Installation (Setup)

```csharp
	// Required services
	builder.Services.AddHttpContextAccessor();
	builder.Services.AddLocalization();
	builder.Services.AddControllersWithViews();

	// Optional configuration
	builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Instancium"));
	builder.Services.AddSingleton<IAppSettings, AppSettingsAdapter>();

	// Required middleware
	app.UseStaticFiles();
	app.UseMiddleware<TagKitHostMiddleware>();
	app.MapStaticAssets();
	app.MapDefaultControllerRoute();
```

