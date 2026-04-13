import { ListRenderer } from "./ListRenderer.js"
import { BlockingState, DeviceConfig, FilterConfig, FilterReference } from "./Data.js";
import { deviceConfigTemplate, filterReferenceTemplate } from "./Templates.js";
import { ListController } from "./ListController.js";
import { API, ListAPI } from "./Utility/API.js";
import { hash } from "./Utility/Hash.js";
import { genAppleConfig } from "./ConfigGenerator.js";

const username = sessionStorage.getItem("username")!;
const password = sessionStorage.getItem("password")!;
const passwordHash = await hash(password);
const authHeader = new Headers({ "username": username, "passwordHash": passwordHash })

const userBaseUrl = `/api/dnsblocker/users/${username}/`;

const addDeviceButton = document.getElementById("deviceList-add")!;
const deviceCreationPopup = document.getElementById("deviceCreationPopup")!;
const createDeviceButton = document.getElementById("createDevice")!;
const cancelDeviceCreationButton = document.getElementById("cancelDeviceCreation")!;
const deviceNameField = document.getElementById("deviceName") as HTMLInputElement;
const refreshButton = document.getElementById("refresh")!;
const globalEnabledButton = document.getElementById("globalEnabled")!;

// Refresh data
refreshButton.addEventListener("click", async () => {
    deviceController.update();
    filterController.update();
    globalEnabled = await getGlobalEnabled();
    updateGlobalEnabledButton();
});

// Hide popup
deviceCreationPopup.style.visibility = "hidden";

// Open popup
addDeviceButton.addEventListener("click", () => {
    deviceCreationPopup.style.visibility = "visible";
    deviceNameField.value = ""
});

// Create device
createDeviceButton.addEventListener("click", () => {
    if (deviceNameField.value === "") return;
    deviceController.addItem({ id: deviceNameField.value, enableBlocking: true });
    deviceCreationPopup.style.visibility = "hidden";
});

// Close popup (cancel creation)
cancelDeviceCreationButton.addEventListener("click", () => deviceCreationPopup.style.visibility = "hidden")

// Toggle global enabled
let globalEnabled = await getGlobalEnabled();
updateGlobalEnabledButton();
globalEnabledButton.addEventListener("click", async () => {
    globalEnabled = !globalEnabled;
    await setGlobalEnabled(globalEnabled);
    updateGlobalEnabledButton();
});

const deviceController = new ListController<DeviceConfig>(
    new ListRenderer<DeviceConfig>({
        container: document.getElementById("deviceList")!,
        template: deviceConfigTemplate,
        buttonHandler: handleDeviceButton
    }),
    new ListAPI<DeviceConfig>(`${userBaseUrl}devices/`, authHeader),
    "id"
);

const filterController = new ListController<FilterReference>(
    new ListRenderer<FilterReference>({
        container: document.getElementById("filterList")!,
        template: filterReferenceTemplate,
        buttonHandler: handleFilterButton
    }),
    new ListAPI<FilterReference>(`${userBaseUrl}filters/`, authHeader),
    "id"
);

if (filterController.items.length == 0) {
    const filterMap = await API.get<Record<string, FilterConfig>>("/api/dnsblocker/global/filters", new Headers());
    Object.values(filterMap).forEach(config => {
        var filterRef: FilterReference = { id: config.id, enableBlocking: false };
        filterController.addItem(filterRef);
    });
}

async function handleDeviceButton(item: DeviceConfig, index: number, action: string): Promise<void> {
    if (action === "enable") {
        item.enableBlocking = !item.enableBlocking;
        const newState: Partial<BlockingState> = { enabled: item.enableBlocking };
        await deviceController.api.setProperty(item.id, "blocking-state", newState);
        deviceController.render();
    }
    else if (action === "delete")
        await deviceController.removeItem(item.id);
    else if (action === "genConfig") {
        const url = "";
        const config = genAppleConfig(url);
        // TODO: Download file
    }
}

async function handleFilterButton(item: FilterReference, index: number, action: string): Promise<void> {
    if (action === "enable") {
        item.enableBlocking = !item.enableBlocking;
        const newState: Partial<BlockingState> = { enabled: item.enableBlocking };
        await filterController.api.setProperty(item.id, "blocking-state", newState);
        filterController.render();
    }
}

async function getGlobalEnabled(): Promise<boolean> {
    const state = await API.get<BlockingState>(`${userBaseUrl}blocking-state`, authHeader);
    return state.enabled;
}

async function setGlobalEnabled(value: boolean): Promise<void> {
    const state: BlockingState = { enabled: value };
    await API.patch(`${userBaseUrl}blocking-state`, authHeader, state);
}

function updateGlobalEnabledButton(): void {
    globalEnabledButton.classList.remove("on", "off");
    globalEnabledButton.classList.add(globalEnabled ? "on" : "off");
    globalEnabledButton.innerText = globalEnabled ? "ON" : "OFF";
}