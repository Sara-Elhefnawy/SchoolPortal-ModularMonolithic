const API_BASE = '';

async function callApi(resultEl, method, path, body) {
    resultEl.textContent = 'Sending...';
    resultEl.className = 'alert alert-info mt-2';

    try {
        const response = await fetch(API_BASE + path, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: body !== undefined
                ? JSON.stringify(body)
                : undefined
        });

        const text = await response.text();

        let data = null;

        if (text) {
            try {
                data = JSON.parse(text);
            } catch {
                data = text;
            }
        }

        if (response.ok) {
            resultEl.className = 'alert alert-success mt-2';
            resultEl.textContent =
                'Success (' + response.status + ')\n' +
                formatApiResult(data);
        } else {
            resultEl.className = 'alert alert-danger mt-2';
            resultEl.textContent =
                'Failed (' + response.status + ')\n' +
                formatApiResult(data);
        }
    } catch (err) {
        resultEl.className = 'alert alert-danger mt-2';
        resultEl.textContent =
            'Network error: ' + err.message;
    }
}

function formatApiResult(data) {
    if (data === null || data === undefined) {
        return 'No response body.';
    }

    if (typeof data === 'string') {
        return data;
    }

    return JSON.stringify(data, null, 2);
}

function formToObject(form, numericFields = []) {
    const data = Object.fromEntries(
        new FormData(form).entries()
    );

    for (const field of numericFields) {
        if (field in data) {
            data[field] = Number(data[field]);
        }
    }

    return data;
}