export const listElementTemplate = (index, htmlContent) => 
/* html */ `
    <div data-index=${index} class="listElement">
        ${htmlContent}
    </div>`;
export const deviceConfigTemplate = (item) => 
/* html */ `
    <h3>${item.id}</h3>
    <div>
        <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>
        <button class="deleteButton element circle" data-action="delete"> </button>
    </div>`;
export const filterReferenceTemplate = (item) => 
/* html */ `
    <h3>${item.id}</h3>
    <button class="element circle" data-action="enable">${item.enableBlocking ? "ON" : "OFF"}</button>`;
