// Shared by Students/Grades/Attendances Index views. Kept separate from the
// default site.js the scaffold generated, so this doesn't collide with
// whatever that file is already doing.
const API_BASE = '';

async function callApi(resultEl, method, path, body) {
    resultEl.textContent = 'Sending...';
    resultEl.className = 'alert alert-secondary mt-2';
    try {
        const response = await fetch(API_BASE + path, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: body !== undefined ? JSON.stringify(body) : undefined
        });
        const text = await response.text();
        const data = text ? JSON.parse(text) : null;
        if (response.ok) {
            resultEl.className = 'alert alert-success mt-2';
            resultEl.textContent = `Success (${response.status})\n` + JSON.stringify(data, null, 2);
        } else {
            resultEl.className = 'alert alert-danger mt-2';
            resultEl.textContent = `Failed (${response.status})\n` + JSON.stringify(data, null, 2);
        }
    } catch (err) {
        resultEl.className = 'alert alert-danger mt-2';
        resultEl.textContent = 'Network/parse error: ' + err.message;
    }
}

function formToObject(form, numericFields = []) {
    const data = Object.fromEntries(new FormData(form).entries());
    for (const field of numericFields) {
        if (field in data) data[field] = Number(data[field]);
    }
    return data;
}