(() => {
    const el = document.querySelector("tag-helper");
    if (!el) return;

    // Internal listener registry for cleanup
    const listeners = [];

    // Component API — scoped to this element only
    const api = {
        // Called when component is inserted or rehydrated
        onInit() {
            const status = document.getElementById("hook-status");
            if (status) {
                status.textContent = "✅ Hook fired at " + new Date().toLocaleTimeString();
                status.classList.add("active");
                setTimeout(() => status.classList.remove("active"), 800);
            }
            console.log("🪄 Component initialized:", el);
        },

        // Business logic
        sayHello() {
            const h2 = el.querySelector("h2");
            alert(h2?.textContent ?? "No heading found.");
        },

        // Called before reload — removes listeners
        detach() {
            listeners.forEach(off => off());
            listeners.length = 0;
        }
    };

    // Attach behavior
    const btn = el.querySelector(".say-btn");
    const handler = () => api.sayHello();
    btn?.addEventListener("click", handler);
    listeners.push(() => btn?.removeEventListener("click", handler));

    // Bind API to element for external lifecycle access
    el.api = api;

    // Initialize component behavior
    api.onInit?.();
})();
