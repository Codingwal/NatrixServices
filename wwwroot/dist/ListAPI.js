export class ListApi {
    baseUrl;
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }
    getItems() {
        throw new Error();
    }
    addItem(item) {
        throw new Error();
    }
    removeItem(id) {
        throw new Error();
    }
    setProperty(id, property, value) {
        throw new Error();
    }
}
