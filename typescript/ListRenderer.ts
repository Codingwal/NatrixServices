import { listElementTemplate } from "./Templates.js";

export interface ListRendererOptions<T> {
    container: HTMLElement;
    template: (item: T) => string;
    buttonHandler: (item: T, index: number, action: string) => void;
}

export class ListRenderer<T> {
    private items: T[] = [];

    public constructor(
        private options: ListRendererOptions<T>
    ) {
        this.options.container.addEventListener("click", (e) => this.onButtonPressed(e));
    }

    public render(items: T[]) {
        this.items = items;

        this.options.container.innerHTML = this.items.map(
            (item: T, index: number) => listElementTemplate(index, this.options.template(item))
        ).join("");
    }

    private onButtonPressed(event: Event): void {
        const target = event.target as HTMLElement;

        const button = target.closest("button[data-action]") as HTMLButtonElement | null;
        if (!button) return;

        const action = button.getAttribute("data-action");
        if (!action) return;

        const itemContainer = target.closest("[data-index]") as HTMLElement | null;
        if (!itemContainer) return;

        const index = Number(itemContainer.getAttribute("data-index"));
        if (isNaN(index)) return;

        const item = this.items[index];

        this.options.buttonHandler(item, index, action);
    }
}