window.inst = window.inst || {};

inst.reload = async function (selector, options = {}, mount = true) {
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
        // 🔥 Re-execute inline scripts
        el.querySelectorAll("script:not([src])").forEach(script => {
            const s = document.createElement("script");
            [...script.attributes].forEach(attr => s.setAttribute(attr.name, attr.value));
            s.textContent = script.textContent;
            script.replaceWith(s);
        });

        for (const hash of resources?.styles ?? []) inst.loadStyle(hash);
        for (const hash of resources?.scripts ?? []) inst.loadScript(hash, true);

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
