import { DeviceConfig, FilterReference } from "./Data.js"

export const listElementTemplate = (index: number, htmlContent: string) =>
    /* html */ `
    <div data-index=${index} class="listElement">
        ${htmlContent}
    </div>`;

export const deviceConfigTemplate = (item: DeviceConfig) =>
    /* html */ `
    <h3>${item.id}</h3>
    <div>
        <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>
        <button class="deleteButton element circle" data-action="delete"> </button>
    </div>`;

export const filterReferenceTemplate = (item: FilterReference) =>
    /* html */ `
    <h3>${item.id}</h3>
    <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>`;