import { ListRenderer } from "./ListRenderer.js";
import { deviceConfigTemplate, filterReferenceTemplate } from "./Templates.js";
import { ListController } from "./ListController.js";
import { ListAPI } from "./API.js";
const baseUrl = "/api/dnsblocker/";
main();
async function hash(str) {
    const encoder = new TextEncoder();
    const buffer = await window.crypto.subtle.digest("SHA-256", encoder.encode(str));
    return buffer2hexStr(buffer);
}
function buffer2hexStr(buffer) {
    return [...new Uint8Array(buffer)]
        .map(x => x.toString(16).padStart(2, "0"))
        .join("");
}
async function main() {
    const username = "user1";
    const password = "secret";
    const passwordHash = await hash(password);
    const authHeader = new Headers({ "username": username, "passwordHash": passwordHash });
    const deviceRenderer = new ListRenderer({
        container: document.getElementById("deviceList"),
        template: deviceConfigTemplate,
        buttonHandler: (item, index, action) => {
            if (action === "enable") {
                item.enableBlocking = !item.enableBlocking;
                deviceRenderer.render();
            }
            else if (action === "delete")
                deviceController.removeItem(item.id);
        }
    });
    const deviceController = new ListController(deviceRenderer, new ListAPI(`${baseUrl}users/${username}/devices`, authHeader), "id");
    deviceController.addItem({ id: "ipad", enableBlocking: true });
    deviceController.addItem({ id: "pc", enableBlocking: false });
    deviceController.addItem({ id: "mobile", enableBlocking: true });
    const filterRenderer = new ListRenderer({
        container: document.getElementById("filterList"),
        template: filterReferenceTemplate,
        buttonHandler: (item, index, action) => { }
    });
    let filters = [];
    filters.push({ id: "filter1", enableBlocking: true });
    filters.push({ id: "filter2", enableBlocking: false });
    filters.push({ id: "filter3", enableBlocking: true });
    filters.push({ id: "filter4", enableBlocking: false });
    filters.push({ id: "filter5", enableBlocking: true });
    filters.push({ id: "filter6", enableBlocking: false });
    filters.push({ id: "filter7", enableBlocking: true });
    filterRenderer.render(filters);
}
