// inst.reload(selector: string, options: { tag, lang, id? })
inst.reload = async function (selector, options) {
    const el = document.querySelector(selector);
    if (!el) return;

    const { tag, lang = "en", id = el.id } = options;
    const query = new URLSearchParams({ lang, id });
    const url = `/taghelper/${tag}?` + query;

    const html = await fetch(url).then(r => r.text());

    const tmp = document.createElement("div");
    tmp.innerHTML = html.trim();
    const next = tmp.firstElementChild;

    if (next) {
        el.replaceWith(next);
        next.api?.onInit?.();
    }
};
