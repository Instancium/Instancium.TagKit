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
        for (const hash of resources?.scripts ?? []) utils.loadScript(hash, true);

        // Optional lifecycle hook
        el.api?.onInit?.();
    }

    return el;
};

/**
 * Dynamically loads a JavaScript file by hash.
 * Prevents duplicates unless `force` is true.
 */
utils.loadScript = function (hash, force = false) {
    const baseSrc = `/instancium/resources/script-${hash}.js`;
    const existing = [...document.scripts].find(s => s.src.split("?")[0] === location.origin + baseSrc);

    if (existing && !force) {
        console.log("⚪ Script already present, skipping:", baseSrc);
        return;
    }

    if (existing) existing.remove();

    const s = document.createElement("script");
    s.src = baseSrc + "?ts=" + Date.now();
    s.defer = true;
    document.head.appendChild(s);
    console.log("✅ Script inserted:", s.src);
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
