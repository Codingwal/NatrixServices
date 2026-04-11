import { listElementTemplate } from "./Templates.js";
export class ListRenderer {
    options;
    items = [];
    constructor(options) {
        this.options = options;
        this.options.container.addEventListener("click", (e) => this.onButtonPressed(e));
    }
    render(items) {
        if (items)
            this.items = items;
        this.options.container.innerHTML = this.items.map((item, index) => listElementTemplate(index, this.options.template(item))).join("");
    }
    onButtonPressed(event) {
        const target = event.target;
        const button = target.closest("button[data-action]");
        if (!button)
            return;
        const action = button.getAttribute("data-action");
        if (!action)
            return;
        const itemContainer = target.closest("[data-index]");
        if (!itemContainer)
            return;
        const index = Number(itemContainer.getAttribute("data-index"));
        if (isNaN(index))
            return;
        const item = this.items[index];
        this.options.buttonHandler(item, index, action);
    }
}
