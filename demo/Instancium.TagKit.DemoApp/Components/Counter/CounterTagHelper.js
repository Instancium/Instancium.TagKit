(() => {
    const el = document.querySelector("tag-helper");
    if (!el) return;

    let count = 0;
    let syncMode = "client"; 

    const input = el.querySelector("input[type='hidden']");
    const display = el.querySelector(".counter-value");

    const api = {
        onInit() {
            const val = parseInt(input?.value);
            count = isNaN(val) ? 0 : val;
            display.innerText = count;
        },

        incrementCounter() {
            count++;

            if (syncMode === "server") {
                const endpoint = el.getAttribute("endpoint");

                fetch(endpoint, {
                    method: "POST",
                    body: JSON.stringify({ value: count }),
                    headers: { "Content-Type": "application/json" },
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error(`Server responded with status ${res.status}`);
                        }
                        return res.json();
                    })
                    .then(data => {
                        if ("delta" in data) {
                            count += data.delta;
                            display.innerText = count;
                            el.dataset.value = count;
                        } else {
                            alert("⚠️ Server response missing 'delta' field.");
                        }
                    })
                    .catch(err => {
                        console.error("❌ Error during fetch:", err);
                        alert("🚫 Failed to connect to server.\nMake sure the endpoint is correct and the server is running.");
                    });
            } else {
                display.innerText = count;
                el.dataset.value = count;
            }
        }

    };

    el.querySelector("button")?.addEventListener("click", () => api.incrementCounter());

    const radios = el.querySelectorAll("input[name='syncMode']");
    radios.forEach(r => r.addEventListener("change", e => {
        syncMode = e.target.value;
    }));

    el.api = api;
    api.onInit?.();
})();