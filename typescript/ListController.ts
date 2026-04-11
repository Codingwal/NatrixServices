import { ListAPI } from "./Utility/API";
import { ListRenderer } from "./ListRenderer";
import { FilteredKeysOf } from "./Utility/Utillity";

export class ListController<T> {
    private items: T[] = [];

    public constructor(
        private renderer: ListRenderer<T>,
        public api: ListAPI<T>,
        private idKey: FilteredKeysOf<T, string>
    ) { }

    public async update(): Promise<void> {
        this.items = await this.api.getItems();
        this.render();
    }

    public async addItem(item: Partial<T>): Promise<T> {
        const newItem: T = await this.api.addItem(item);
        this.items.push(newItem);
        this.render();
        return newItem;
    }

    public async removeItem(id: string): Promise<void> {
        await this.api.removeItem(id);
        this.items = this.items.filter((item) => item[this.idKey] !== id);
        this.render();
    }

    public render(): void {
        this.renderer.render(this.items);
    }
}