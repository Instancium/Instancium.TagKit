// Create a safe global namespace for Instancium
window.instancium = window.instancium || {};
const core = window.instancium.core = window.instancium.core || {};
const utils = window.instancium.utils = window.instancium.utils || {};

/**
 * Reloads a tag component dynamically from the server.
 * @param {string} selector - CSS selector for the target element.
 * @param {object} options - Optional parameters: tag, id, lang.
 * @param {boolean} mount - Whether to re-execute scripts and load resources.
 */
core.reload = async function (selector, options = {}, mount = true) {
    const el = document.querySelector(selector);
    if (!el) return;

    const tag = options.tag || el.getAttribute("is") || el.tagName.toLowerCase();
    const id = options.id || el.id;
    const lang = options.lang || "en";

    const query = new URLSearchParams({ lang, id });
    const url = `/taghelper/${tag}?` + query.toString();

    const { html, resources } = await fetch(url).then(r => r.json());

    const tmp = document.createElement("div");
    tmp.innerHTML = html.trim();
    el.replaceChildren(...tmp.childNodes);

    if (mount) {
        // 🔁 Re-execute inline <script> tags
        el.querySelectorAll("script:not([src])").forEach(script => {
            const s = document.createElement("script");
            [...script.attributes].forEach(attr => s.setAttribute(attr.name, attr.value));
            s.textContent = script.textContent;
            script.replaceWith(s);
        });

        // Load external styles and scripts
        for (const hash of resources?.styles ?? []) utils.loadStyle(hash);
        for (const hash of resources?.scripts ?? []) utils.loadScriptStable(hash);

        // Optional lifecycle hook
        el.api?.onInit?.();
    }

    return el;
};


/**
 * Safely reloads a script by hash without polluting Sources panel.
 * Removes previous instance, inserts with stable URL.
 * @param {string} hash - Script identifier hash (already versioned).
 */
utils.loadScriptStable = function (hash) {
    const baseSrc = `/instancium/resources/script-${hash}.js`;
    const absoluteSrc = location.origin + baseSrc;

    const existing = [...document.scripts].find(s => s.src === absoluteSrc);
    if (existing) {
        existing.remove(); // ⛔️ Explicit cleanup
        console.log("🧹 Removed old script:", baseSrc);
    }

    const script = document.createElement("script");
    script.src = baseSrc;
    script.defer = true;
    document.head.appendChild(script);
    console.log("✅ Inserted stable script:", baseSrc);
};


/**
 * Dynamically loads a CSS file by hash.
 * Skips if already present.
 */
utils.loadStyle = function (hash) {
    const href = `/instancium/resources/style-${hash}.css`;
    const exists = [...document.styleSheets].some(s => s.href?.includes(href));
    if (exists) return;

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = href + "?ts=" + Date.now();
    document.head.appendChild(link);
};

/**
 * Optional compatibility bridge for legacy `window.inst` usage.
 * Only applies if `inst` is not already defined.
 */
if (!window.inst?.reload) {
    window.inst = window.inst || {};
    window.inst.reload = (...args) => {
        console.warn("⚠️ inst.reload is deprecated. Use instancium.core.reload instead.");
        return core.reload(...args);
    };
    window.inst.loadScript = utils.loadScript;
    window.inst.loadStyle = utils.loadStyle;
}


utils.auditMemory = function () {
    const summary = {
        detached: [],   // Tracks orphaned DOM nodes not attached to document.body
        listeners: [],  // Tracks registered event listeners on the target component
        globals: [],    // Tracks global pollution in the window scope
    };

    // 🔍 Detect detached DOM nodes (not part of the live document tree)
    const all = performance?.getEntriesByType?.("resource") ?? [];
    summary.detached = [...document.querySelectorAll("*")].filter(el => {
        return !document.body.contains(el) && el.tagName !== "SCRIPT";
    });

    // 👂 Detect event listeners (DevTools API only, may require inspection mode)
    try {
        const el = document.querySelector("tag-helper");
        const events = getEventListeners?.(el) ?? {};
        summary.listeners = Object.entries(events).map(([type, handlers]) => ({
            type,
            count: handlers.length,
        }));
    } catch {
        summary.listeners.push({ type: "DevToolsOnly", count: "N/A" });
    }

    // 🌍 Detect global pollution — variables leaking into window scope
    for (const key in window) {
        if (key.startsWith("inst_") || key.includes("tag")) {
            summary.globals.push(key);
        }
    }


    // 📊 Output audit summary to console
    console.group("🧠 TagKit Memory Audit");
    console.log("📦 Detached DOM Elements:", summary.detached.length);
    console.log("🎧 Active Event Listeners:", summary.listeners);
    console.log("🌍 Global Variables:", summary.globals);
    console.groupEnd();


    return summary;
};

