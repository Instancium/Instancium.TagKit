# Instancium.TagKit

**A communication protocol for components — not just another UI library.**

This library is a core component of the **Instancium Protocol**,  
an architecture-first initiative for AI-native, structure-aware systems.  
Learn more in the [Protocol Manifesto](https://github.com/Instancium/.github/blob/main/profile/MANIFESTO.md).

Instancium.TagKit is a runtime and resource contract for server-rendered components. Built for predictable reload, seamless integration, and absolute autonomy.

---

## 🌌 Philosophy

Instancium does not introduce a runtime. It reveals one.

- 🧠 Protocol over framework  
- ✨ Predictability over magic  
- 📦 Components as autonomous units of interface — not frontend artifacts  
- 🤝 Server-first mindset with optional client-side activation
- ⛔ **No frameworks. No ceremony. No implicit runtime.**  
- 🕊️ **Stateless by default. Predictable by protocol. Never opaque.**

Components describe. Protocols coordinate. You remain in control.

### Plain HTML Preserves Clarity and Autonomy

By using `.html` templates without server directives:

- You get true separation of concerns — code in `.cs`, structure in `.html`
- Components don’t require compilation — edit and reload instantly
- Frontend work can happen in any environment or tool (even outside .NET)
- Markup becomes portable, testable, and safe for reuse

### Why No Razor in Markup Template (plain HTML)?

Instancium components intentionally avoid Razor (`.cshtml`) templates — and this is a core part of the protocol.

Allowing `.cshtml` in components opens the door to:

- Server-side logic inside markup (`@if`, `@foreach`, etc.)
- State mutations and service calls within HTML structure
- Tightly coupled UI that depends on compilation and backend context

This undermines the very purpose of components as declarative, self-contained units.  
Instancium chooses simplicity and direction over convenience and ambiguity.

## 🧱 Project Structure

| Folder | Purpose |
|--------|---------|
| [src/Instancium.TagKit.Core](./src/Instancium.TagKit.Core) | Minimal runtime with reload protocol and resource hooks |
| [src/DemoApp](./src/DemoApp) | Example app: TagHelpers, reload flows, and UI scaffolding |
| [tests](./tests) | Runtime and component reload tests |

---

## 🤝 Contributing

Instancium welcomes contributions that align with its vision: transparency, autonomy, and composability.

While a formal `CONTRIBUTING.md` is coming soon, feel free to explore the `Core`, suggest extensions, or raise issues.

👁️‍🗨️ **The Guild**  
Instancium maintains its architectural rhythm through a growing guild — a collective of engineers, philosophers, and designers who care about the clarity and direction of the protocol.  
If you seek to contribute not just code, but meaning — [consider joining](https://instancium.com/#guild).

## 🏢 Supporting the Project
Instancium is more than a runtime — it's a philosophy of transparent, declarative architecture.

If your organization finds value in adopting Instancium for clean, minimal, framework-free UI infrastructure, please consider supporting the project:

- 💼 Use it in production — and share feedback to help evolve the protocol
- 🤝 Sponsor development — support continuous, focused, transparent work
- 🌱 Join the Guild — help shape the architectural core and steward its direction
- 📬 Get in touch: support@protonmail.com 🧭 Learn more: instancium.com (soon)

📄 License MIT © 2025 Instancium Contributors  
✒️ [Authorship statement](https://github.com/Instancium/.github/blob/main/profile/Authorship.md)
