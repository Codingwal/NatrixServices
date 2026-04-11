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
        const fullUrl = url + urlParams.toString();
        const response = await fetch(fullUrl, {
            method: method,
            headers: headers,
            body: body ? JSON.stringify(body) : undefined
        });
        if (!response.ok)
            throw new Error("Fetch error: " + response.status.toString());
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
        const data = [];
        return data;
        // return await API.get<T[]>(this.baseUrl, this.authHeaders);
    }
    async addItem(item) {
        return item;
        // return await API.post<T, Partial<T>>(this.baseUrl, this.authHeaders, item);
    }
    async removeItem(id) {
        // await API.delete(this.baseUrl + id, this.authHeaders);
    }
    async setProperty(id, property, value) {
        return value;
        // return await API.patch(`${this.baseUrl}${id}/${property}`, this.authHeaders, value);
    }
}
