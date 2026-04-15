import { DeviceConfig, FilterReference } from "./Data.js"

export const deviceConfigTemplate = (item: DeviceConfig) =>
    /* html */ `
    <h3>${item.id}</h3>
    <div>
        <button data-action="genConfig" class="element circle download"></button>
        <button data-action="enable" class="element circle">${item.enableBlocking ? "ON" : "OFF"}</button>
        <button data-action="delete" class="element circle delete"></button>
    </div>`;

export const filterReferenceTemplate = (item: FilterReference) =>
    /* html */ `
    <h3>${item.id}</h3>
    <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>`;