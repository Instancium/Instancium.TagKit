# Instancium.TagKit (Archived / Concept)

**Instancium.TagKit** is an experimental server-side component engine for ASP.NET Core, engineered with a single objective: **to make UI code generation by AI models (LLMs) highly reliable and predictable.** 

The project completely eliminates the Razor view engine from the rendering pipeline, replacing it with a strict "pure HTML + pure C#" contract.

---

## 🎯 The Purpose

Modern LLMs (Claude, GPT, Cursor) are exceptional at generating standard frontend code but frequently introduce syntax errors when dealing with Razor directives (`.cshtml`). They struggle with heavy compilation contexts and often break the .NET build when trying to generate inline server logic like `@if` or `@foreach`.

TagKit was built to solve this problem by enforcing a strict separation of concerns:

* **Ideal AI Context:** Components use pure, valid HTML entirely free of server-side directives. An AI can safely generate, update, and validate this markup autonomously using standard frontend tools.
* **Backend Safety:** All server-side logic is decoupled into isolated C# classes. The AI can rewrite the interface without risking breaking core system architecture or state mutations.
* **Instant Feedback Loop:** Changes to HTML templates apply instantly (**Zero-Compilation Hot-Reload**). This is crucial for autonomous AI agents that iteratively refine the UI in real time.

As a result, each UI component becomes an isolated, predictable, and safe building block that an AI can generate and maintain without human friction.

---

## ⚙️ Key Features

Instancium.TagKit is not designed for building universal component libraries. It exists to help you quickly create exactly what your project needs, without overhead or abstraction.

With Instancium.TagKit, you can:
* **Prompt an AI** (or write manually) to generate HTML structure, JS behavior, and CSS styling.
* **Instantly test your component** — no compilation, no Razor, no hidden layers.
* **Define server-side logic cleanly in C#** — decoupled and explicit.
* **Use it as a TagHelper** — an autonomous, self-contained unit of interface.

---

## 🧱 Project Structure

| Folder | Purpose |
| :--- | :--- |
| **`src/Instancium.TagKit.Core`** | Minimal runtime with reload protocol and resource hooks. |
| **`demo/DemoApp`** | Example app: TagHelpers, reload flows, and UI scaffolding (including HTMX/Alpine.js context). |
| **`tests`** | Runtime and component reload tests. |

---

## 🛑 Project Status: Frozen

This project is now a **conceptual archive**. Active development has been suspended. 

The source code and the underlying architecture are shared "as is" to document this approach. Feel free to use the code, fork the concept, or adapt these principles for your own AI-driven development workflows.

---

## 📄 License

MIT © 2025 Instancian Contributors
