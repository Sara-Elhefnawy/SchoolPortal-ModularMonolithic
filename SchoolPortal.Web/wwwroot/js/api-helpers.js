// ============================================================
// API CALL INTO AN ELEMENT
// ============================================================
//
// Usage:
//     await callApiInto(resultEl, 'POST', '/api/students', student);
//
// Why this just delegates: CSRF and fetch logic live in ONE place (site.js).
// Copies of that logic in each module are what made this hard to debug.
// ============================================================

async function callApiInto(resultEl, method, path, body) {
    resultEl.textContent = 'Loading...';
    resultEl.className = 'alert alert-secondary mt-2';

    const result = await callApi(path, method, body);

    resultEl.className = result.ok
        ? 'alert alert-success mt-2'
        : 'alert alert-danger mt-2';

    // Why the fallback to message: network errors have no response body.
    const detail =
        result.data === null || result.data === undefined
            ? result.message
            : (typeof result.data === 'string'
                ? result.data
                : JSON.stringify(result.data, null, 2));

    resultEl.textContent =
        `${result.ok ? 'Success' : 'Failed'} (${result.status})\n` + detail;

    return result;
}