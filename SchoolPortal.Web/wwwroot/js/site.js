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

        // "json" (not "application/json"): ASP.NET's automatic 400s use
        // application/problem+json, which the longer string would miss.
        const data = contentType.includes("json")
            ? await response.json()
            : await response.text();

        if (!response.ok) {
            return {
                ok: false,
                status: response.status,
                data,
                // Read the SERVER's reply (data), not the request we sent (body).
                message: readErrorMessage(data) || fallbackMessage(response.status)
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

// 401/403/404 come back with an empty body, so without this they'd all show the
// same generic text, and you couldn't tell "not logged in" from "no permission".
function fallbackMessage(status) {
    switch (status) {
        case 401: return "Your session expired. Please log in again.";
        case 403: return "You don't have permission to do that.";
        case 404: return "That endpoint or record was not found.";
        default: return status >= 500
            ? "Server error. Check the server console for details."
            : "The request failed.";
    }
}

// Accepts { error: { description } }, { error: "text" }, the bare { code, description }
// shape, and ASP.NET problem details ({ title, detail }).
function readErrorMessage(body) {
    if (!body) return null;
    const err = body.error ?? body;
    if (typeof err === "string") return err;
    return err.description ?? err.message ?? err.detail ?? err.title ?? err.code ?? null;
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