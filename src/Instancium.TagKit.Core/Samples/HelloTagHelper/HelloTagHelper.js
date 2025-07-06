(() => {
    var el = document.querySelector("tag-helper");
    if (!el) return;

    const api = {
        sayHello() {
            const h2 = el.querySelector("h2");
            alert(h2?.textContent ?? "No heading found.");
        },
        onInit() {
            const status = document.getElementById("hook-status");
            if (status) {
                status.textContent = "✅ Hook fired at " + new Date().toLocaleTimeString();
                status.classList.add("active");
                setTimeout(() => status.classList.remove("active"), 800);
            }
            console.log("🪄 Component initialized:", el);
        }
    };

    el.api = api;

    el.querySelector(".say-btn")?.addEventListener("click", api.sayHello);

    api.onInit?.();
})();

