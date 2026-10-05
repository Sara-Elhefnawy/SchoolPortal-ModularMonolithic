// ============================================================
// CSRF
// ============================================================
//
// Why we ask the server instead of reading a cookie:
// the token is tied to the identity (anonymous / logged-in user) at the
// moment it is created. Asking right before the request guarantees it
// matches who the user is now.
// ============================================================

async function getCsrfToken() {
    const response = await fetch("/api/antiforgery/token", {
        method: "GET",
        credentials: "same-origin",
        cache: "no-store",
        headers: { "Accept": "application/json" }
    });

    if (!response.ok) {
        const text = await response.text();
        throw new Error("Could not get a security token (" + response.status + "): " + text);
    }

    const data = await response.json();

    if (!data.token)
        throw new Error("Server did not return a CSRF token.");
    
    return data.token;
}


// ============================================================
// API FETCH  (the ONLY place CSRF is attached)
// ============================================================
//
// Every module's frontend goes through this, directly or via callApi.
// ============================================================

const STATE_CHANGING_METHODS = ["POST", "PUT", "PATCH", "DELETE"];

async function apiFetch(url, options = {}) {
    const method = (options.method ?? "GET").toUpperCase();
    const needsCsrf = STATE_CHANGING_METHODS.includes(method);

    // Why a local function: we may need to send the request twice (retry below),
    // and each attempt must build its own headers with a fresh token.
    async function send() {
        const headers = new Headers(options.headers || {});
        headers.set("Accept", "application/json");

        if (needsCsrf) {
            headers.set("X-CSRF-TOKEN", await getCsrfToken());
        }

        return fetch(url, {
            ...options,
            method: method,
            headers: headers,
            credentials: "same-origin"
        });
    }

    let response = await send();

    // Why retry once: if two requests fire at the same moment before the browser
    // has any antiforgery cookie, their cookie/token pairs can mismatch. A second
    // attempt with a fresh token fixes that. We only retry when the server says
    // the failure was CSRF, so real validation errors are never repeated.
    if (needsCsrf && response.status === 400) {
        const body = await response.clone().json().catch(() => null);

        console.log("CSRF response:", {
            url,
            body
        });
        
        if (body && body.code === "csrf") {
            response = await send();
        }
    }

    return response;
}


// ============================================================
// API CALL  (returns { ok, status, data, message })
// ============================================================

async function callApi(path, method = "GET", body = null) {
    try {
        const options = { method: method };

        if (body !== null && body !== undefined) {
            options.headers = { "Content-Type": "application/json" };
            options.body = JSON.stringify(body);
        }

        const response = await apiFetch(path, options);

        const text = await response.text();

        let data = null;

        if (text) {
            try {
                data = JSON.parse(text);
            }
            catch {
                data = text;
            }
        }

        if (!response.ok) {
            let message = "Request failed.";

            if (typeof data === "string") {
                message = data;
            }
            else if (data && typeof data === "object") {
                // Why data.error first: the Identity endpoints return { error: "..." },
                // while validation problems use title/detail and our CSRF response uses message.
                message = data.error || data.message || data.title || data.detail || "Request failed.";
            }

            return { ok: false, status: response.status, data: data, message: message };
        }

        return {
            ok: true,
            status: response.status,
            data: data,
            message: data?.message ?? "Success."
        };
    }
    catch (error) {
        return {
            ok: false,
            status: 0,
            data: null,
            message: "Network error: " + error.message
        };
    }
}

// ============================================================
// FORM -> OBJECT
// ============================================================
//
// Why it lives here (and only here): site.js loads first in _Layout,
// so every page and every other script can use it.
// ============================================================

function formToObject(form, numericFields = []) {
    const data = Object.fromEntries(new FormData(form).entries());

    for (const field of numericFields) {
        if (field in data) {
            data[field] = Number(data[field]);
        }
    }

    return data;
}


// ============================================================
// ESCAPE HTML
// ============================================================
//
// Why: values from the API are inserted into the page as HTML.
// Escaping them stops a student name like "<script>..." from running as code.
// ============================================================

function escapeHtml(value) {
    if (value === null || value === undefined) {
        return "";
    }

    return String(value)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// ============================================================
// TOAST
// ============================================================
//
// Why `window.showToast ||`: if _Layout already defines one, keep that
// and don't override it.
// Why textContent: messages can contain server text, so never inject it as HTML.
// ============================================================

window.showToast = window.showToast || function (message, type = "success") {
    // Fallback if Bootstrap's JS isn't loaded on this page.
    if (typeof bootstrap === "undefined") {
        alert(message);
        return;
    }

    let container = document.getElementById("toastContainer");
    if (!container) {
        container = document.createElement("div");
        container.id = "toastContainer";
        container.className = "toast-container position-fixed top-0 end-0 p-3";
        document.body.appendChild(container);
    }

    const el = document.createElement("div");
    el.className = `toast align-items-center text-bg-${type} border-0`;
    el.setAttribute("role", "alert");
    el.innerHTML = `
        <div class="d-flex">
            <div class="toast-body"></div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>`;
    el.querySelector(".toast-body").textContent = message;

    container.appendChild(el);
    el.addEventListener("hidden.bs.toast", () => el.remove());
    new bootstrap.Toast(el, { delay: 4000 }).show();
};