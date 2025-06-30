window.inst = window.inst || {};

inst.reload = async function (selector, options, mount = true) {
    const el = document.querySelector(selector);
    if (!el) return;

    const { tag, lang = "en", id = el.id } = options;
    const query = new URLSearchParams({ lang, id });
    const url = `/taghelper/${tag}?` + query;

    const { html, resources } = await fetch(url).then(r => r.json());

    const tmp = document.createElement("div");
    tmp.innerHTML = html.trim();

    // 🔄 Replace existing DOM content
    while (el.firstChild) el.removeChild(el.firstChild);
    for (const node of tmp.childNodes) {
        el.appendChild(node.cloneNode(true));
    }

    // ⛩️ Execute behavior only if mount is true
    if (mount) {
        // 🔥 Re-execute inline <script> elements
        el.querySelectorAll("script").forEach(script => {
            if (script.src) return;
            const s = document.createElement("script");
            for (const attr of script.attributes) s.setAttribute(attr.name, attr.value);
            s.textContent = script.textContent;
            script.replaceWith(s);
        });

        // 🎨 Load external styles if needed
        for (const hash of resources?.styles ?? []) {
            inst.loadStyle(hash);
        }

        // 💡 Load external scripts if needed
        for (const hash of resources?.scripts ?? []) {
            inst.loadScript(hash, true);
        }

        // 🔁 Run component lifecycle if defined
        el.api?.onInit?.();
    }

    return el;
};

inst.loadScript = function (hash, force = false) {
    const baseSrc = `/instancium/resources/script-${hash}.js`;

    // 🔎 Check if script is already loaded (ignoring query string)
    const existing = [...document.scripts].find(s => s.src.split("?")[0] === location.origin + baseSrc);
    if (existing && !force) {
        console.log("⚪ Script already present, skipping:", baseSrc);
        return;
    }

    // 🔄 Remove old version if force-reloading
    if (existing) existing.remove();

    // ✅ Inject new <script> with cache-busting
    const s = document.createElement("script");
    s.src = baseSrc + "?ts=" + Date.now();
    s.defer = true;
    document.head.appendChild(s);
    console.log("✅ Script inserted:", s.src);
};

inst.loadStyle = function (hash) {
    const href = `/instancium/resources/style-${hash}.css`;

    // 🎯 Prevent duplicate <link> injection
    const exists = [...document.styleSheets].some(s => s.href?.includes(href));
    if (exists) return;

    // 🎨 Inject <link> for the stylesheet with cache-busting
    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = href + "?ts=" + Date.now();
    document.head.appendChild(link);
};
