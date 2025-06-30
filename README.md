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

---

## 🧱 Project Structure

| Folder | Purpose |
|--------|---------|
| `src/Instancium.TagKit.Core` | Minimal runtime with reload protocol and resource hooks |
| `src/DemoApp` | Example app: TagHelpers, reload flows, and UI scaffolding |
| `tests/` | Runtime and component reload tests |

---

## 🚀 Getting Started

```bash
git clone https://github.com/Instancium/Instancium.TagKit.git
cd Instancium.TagKit
dotnet restore
dotnet run --project src/DemoApp
```

## 🤝 Contributing
Instancium welcomes contributions that align with its vision: transparency, autonomy, and composability.  
A proper CONTRIBUTING.md is coming soon — until then, feel free to explore the Core, suggest extensions, or raise issues.  

📄 License
Apache 2.0 © 2025 Instancium Contributors

