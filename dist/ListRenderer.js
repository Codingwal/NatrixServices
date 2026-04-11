"use strict";
// import * from "./dist/Utility.js";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ListRenderer = void 0;
class ListRenderer {
    options;
    items = [];
    constructor(options) {
        this.options = options;
    }
    render(items) {
        this.items = items;
        this.options.container.innerHTML = items.map((item, index) => `<div data-index=${index}>${this.options.template(item)}</div>`).join("");
    }
}
exports.ListRenderer = ListRenderer;
