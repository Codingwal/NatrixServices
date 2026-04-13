export class APIError extends Error {
    public constructor(
        public statusCode: number,
        public serverMessage: string
    ) {
        super(`Fetch error: ${statusCode.toString()} (\"${serverMessage}\")`);
        this.name = "API Error";

        Object.setPrototypeOf(this, APIError.prototype);
    }
}

export class API {
    public static async get<TReturn>(url: string, headers: Headers, urlParams = new URLSearchParams()): Promise<TReturn> {
        return this.request(url, "GET", headers, urlParams);
    }

    public static async post<TReturn, TBody>(url: string, headers: Headers, body: Partial<TBody>): Promise<TReturn> {
        return this.request(url, "POST", headers, new URLSearchParams(), body);
    }

    public static async put<TReturn, TBody>(url: string, headers: Headers, body: Partial<TBody>): Promise<TReturn> {
        return this.request(url, "PUT", headers, new URLSearchParams(), body);
    }

    public static async patch<TReturn, TBody>(url: string, headers: Headers, body: Partial<TBody>): Promise<TReturn> {
        return this.request(url, "PATCH", headers, new URLSearchParams(), body);
    }

    public static async delete(url: string, headers: Headers): Promise<void> {
        return this.request(url, "DELETE", headers, new URLSearchParams());
    }

    private static async request<TReturn, TBody>(
        url: string,
        method: string,
        headers: Headers,
        urlParams: URLSearchParams,
        body?: TBody
    ): Promise<TReturn> {
        headers.set("Content-Type", "application/json");
        const urlParamsStr = urlParams.toString()
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
            return null as TReturn;

        return (await response.json()) as TReturn;
    }
}

export class ListAPI<T> {

    public constructor(
        private baseUrl: string,
        private authHeaders: Headers
    ) { }

    public async getItems(): Promise<Record<string, T>> {
        return await API.get(this.baseUrl, this.authHeaders);
    }

    public async addItem(item: Partial<T>): Promise<T> {
        return await API.post<T, Partial<T>>(this.baseUrl, this.authHeaders, item);
    }

    public async removeItem(id: string): Promise<void> {
        await API.delete(this.baseUrl + id, this.authHeaders);
    }

    public async setProperty<TProperty>(id: string, property: string, value: Partial<TProperty>): Promise<TProperty> {
        return await API.patch(`${this.baseUrl}${id}/${property}`, this.authHeaders, value);
    }

    public async getProperty<TProperty>(id: string, property: string): Promise<TProperty> {
        return await API.get<TProperty>(`${this.baseUrl}${id}/${property}`, this.authHeaders);
    }
}