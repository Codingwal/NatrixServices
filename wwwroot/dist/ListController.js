export class ListController {
    renderer;
    api;
    idKey;
    items = [];
    constructor(renderer, api, idKey) {
        this.renderer = renderer;
        this.api = api;
        this.idKey = idKey;
        this.update();
    }
    async update() {
        this.items = Object.values(await this.api.getItems());
        this.render();
    }
    async addItem(item) {
        const newItem = await this.api.addItem(item);
        this.items.push(newItem);
        this.render();
        return newItem;
    }
    async removeItem(id) {
        await this.api.removeItem(id);
        this.items = this.items.filter((item) => item[this.idKey] !== id);
        this.render();
    }
    render() {
        this.renderer.render(this.items);
    }
}
