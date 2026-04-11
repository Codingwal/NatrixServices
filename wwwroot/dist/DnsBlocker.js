import { ListRenderer } from "./ListRenderer.js";
import { deviceConfigTemplate, filterReferenceTemplate } from "./Templates.js";
import { ListController } from "./ListController.js";
import { ListAPI } from "./Utility/API.js";
import { hash } from "./Utility/Hash.js";
const username = sessionStorage.getItem("username");
const password = sessionStorage.getItem("password");
const passwordHash = await hash(password);
const authHeader = new Headers({ "username": username, "passwordHash": passwordHash });
const userBaseUrl = `/api/dnsblocker/users/${username}/`;
const addDeviceButton = document.getElementById("deviceList-add");
const deviceCreationPopup = document.getElementById("deviceCreationPopup");
const createDeviceButton = document.getElementById("createDevice");
const cancelDeviceCreationButton = document.getElementById("cancelDeviceCreation");
const deviceNameField = document.getElementById("deviceName");
const refreshButton = document.getElementById("refresh");
const globalEnabledButton = document.getElementById("globalEnabled");
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
    deviceNameField.value = "";
});
// Create device
createDeviceButton.addEventListener("click", () => {
    if (deviceNameField.value === "")
        return;
    deviceController.addItem({ id: deviceNameField.value, enableBlocking: true });
    deviceCreationPopup.style.visibility = "hidden";
});
// Close popup (cancel creation)
cancelDeviceCreationButton.addEventListener("click", () => deviceCreationPopup.style.visibility = "hidden");
// Toggle global enabled
let globalEnabled = await getGlobalEnabled();
updateGlobalEnabledButton();
globalEnabledButton.addEventListener("click", async () => {
    globalEnabled = !globalEnabled;
    await setGlobalEnabled(globalEnabled);
    updateGlobalEnabledButton();
});
const deviceController = new ListController(new ListRenderer({
    container: document.getElementById("deviceList"),
    template: deviceConfigTemplate,
    buttonHandler: handleDeviceButton
}), new ListAPI(`${userBaseUrl}devices`, authHeader), "id");
const filterController = new ListController(new ListRenderer({
    container: document.getElementById("filterList"),
    template: filterReferenceTemplate,
    buttonHandler: handleFilterButton
}), new ListAPI(`${userBaseUrl}filters`, authHeader), "id");
async function handleDeviceButton(item, index, action) {
    if (action === "enable") {
        item.enableBlocking = !item.enableBlocking;
        const newState = { enabled: item.enableBlocking };
        await deviceController.api.setProperty(item.id, "blocking-state", newState);
        deviceController.render();
    }
    else if (action === "delete")
        await deviceController.removeItem(item.id);
}
async function handleFilterButton(item, index, action) {
    if (action === "enable") {
        item.enableBlocking = !item.enableBlocking;
        const newState = { enabled: item.enableBlocking };
        await filterController.api.setProperty(item.id, "blocking-state", newState);
        filterController.render();
    }
}
async function getGlobalEnabled() {
    return true;
    // const state = await API.get<BlockingState>(`${userBaseUrl}blocking-state`, authHeader);
    // return state.enabled;
}
async function setGlobalEnabled(value) {
    // const state: BlockingState = { enabled: value };
    // await API.put(`${userBaseUrl}blocking-state`, authHeader, state);
}
function updateGlobalEnabledButton() {
    globalEnabledButton.classList.remove("on", "off");
    globalEnabledButton.classList.add(globalEnabled ? "on" : "off");
    globalEnabledButton.innerText = globalEnabled ? "ON" : "OFF";
}
