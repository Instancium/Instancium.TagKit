# Instancium.TagKit

**A communication protocol for components — not just another UI library.**

Instancium.TagKit is a runtime and resource contract for server-rendered components. Built for predictable reload, seamless integration, and absolute autonomy.

---

## 🌌 Philosophy

Instancium does not introduce a runtime. It reveals one.

- 🧠 Protocol over framework  
- ✨ Predictability over magic  
- 📦 Components as autonomous units of interface — not frontend artifacts  

- ⛔ **No frameworks. No ceremony. No implicit runtime.**  
- 🕊️ **Stateless by default. Predictable by protocol. Never opaque.**

Components describe. Protocols coordinate. You remain in control.

### Using Razor (or not)

Instancium components can be rendered within Razor Pages or Views, but Razor is not required — nor is it central to the protocol.

- You can use Razor as a layout engine or bootstrapping host.
- Components are TagHelpers, not `.cshtml` views.
- There is no dependency on Razor compilation or lifecycle.

Razor is optional. Protocol is primary.

---

## 🧱 Project Structure

| Folder | Purpose |
|--------|---------|
| `src/Instancium.TagKit.Core` | Minimal runtime with reload protocol and resource hooks |
| `src/DemoApp` | Example app: TagHelpers, reload flows, and UI scaffolding |
| `tests/` | Runtime and component reload tests |

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

📄 License
Apache 2.0 © 2025 Instancium Contributors

