const API_BASE = '';

async function callApi(url, method = "GET", body = null) {
    try {
        const options = {
            method,
            credentials: "same-origin",
            headers: { "Accept": "application/json" }
        };

        if (body !== null && method !== "GET") {
            options.headers["Content-Type"] = "application/json";
            options.body = JSON.stringify(body);
        }

        const response = await fetch(url, options);
        const contentType = response.headers.get("content-type") || "";
        const data = contentType.includes("application/json")
            ? await response.json()
            : await response.text();

        if (!response.ok) {
            return {
                ok: false,
                status: response.status,
                data,
                message: data?.message || data?.title || "The request failed."
            };
        }

        return {
            ok: true,
            status: response.status,
            data,
            message: data?.message
        };
    } catch (error) {
        return { ok: false, message: error.message || "Network error." };
    }
}

function formToObject(form) {
    const object = {};
    new FormData(form).forEach((value, key) => object[key] = value);
    return object;
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function showToast(message, type = "success") {
    const toast = document.createElement("div");
    toast.className = `app-toast toast-${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}