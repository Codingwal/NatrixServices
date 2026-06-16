export class APIError extends Error {
    statusCode;
    serverMessage;
    constructor(statusCode, serverMessage) {
        super(`Fetch error: ${statusCode.toString()} (\"${serverMessage}\")`);
        this.statusCode = statusCode;
        this.serverMessage = serverMessage;
        this.name = "API Error";
        Object.setPrototypeOf(this, APIError.prototype);
    }
}
export class API {
    static async get(url, headers, urlParams = new URLSearchParams()) {
        return this.request(url, "GET", headers, urlParams);
    }
    static async post(url, headers, body) {
        return this.request(url, "POST", headers, new URLSearchParams(), body);
    }
    static async put(url, headers, body) {
        return this.request(url, "PUT", headers, new URLSearchParams(), body);
    }
    static async patch(url, headers, body) {
        return this.request(url, "PATCH", headers, new URLSearchParams(), body);
    }
    static async delete(url, headers) {
        return this.request(url, "DELETE", headers, new URLSearchParams());
    }
    static async request(url, method, headers, urlParams, body) {
        headers.set("Content-Type", "application/json");
        const urlParamsStr = urlParams.toString();
        const fullUrl = urlParamsStr ? `${url}?${urlParamsStr}` : url;
        const response = await fetch(fullUrl, {
            method: method,
            headers: headers,
            body: body ? JSON.stringify(body) : undefined
        });
        if (!response.ok) {
            let errorMessage;
            if (response.status === 404)
                errorMessage = "Not found";
            else if (response.status === 401)
                errorMessage = "Unauthorized";
            else if (response.status >= 500)
                errorMessage = "Server error";
            else
                errorMessage = "Unknown error";
            throw new APIError(response.status, errorMessage);
        }
        if (response.headers.get("content-length") === "0")
            return null;
        return (await response.json());
    }
}
export class ListAPI {
    baseUrl;
    authHeaders;
    constructor(baseUrl, authHeaders) {
        this.baseUrl = baseUrl;
        this.authHeaders = authHeaders;
    }
    async getItems() {
        return await API.get(this.baseUrl, this.authHeaders);
    }
    async addItem(item) {
        return await API.post(this.baseUrl, this.authHeaders, item);
    }
    async removeItem(id) {
        await API.delete(this.baseUrl + id, this.authHeaders);
    }
    async setProperty(id, property, value) {
        return await API.patch(`${this.baseUrl}${id}/${property}`, this.authHeaders, value);
    }
    async getProperty(id, property) {
        return await API.get(`${this.baseUrl}${id}/${property}`, this.authHeaders);
    }
}
