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
        <button data-action="genConfig" class="element circle">config</button>
        <button data-action="enable" class="element circle">${item.enableBlocking ? "ON" : "OFF"}</button>
        <button data-action="delete" class="deleteButton element circle"></button>
    </div>`;

export const filterReferenceTemplate = (item: FilterReference) =>
    /* html */ `
    <h3>${item.id}</h3>
    <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>`;