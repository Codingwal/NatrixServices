const UNKNOWN = -1;
const INACTIVE = 0;
const ACTIVE = 1;

let currentstatus = UNKNOWN;

let toogleButton = document.getElementById('toogle-btn');
toogleButton.onclick = toogle;
let statusCircle = document.getElementById('main-status-circle');
let statusText = document.getElementById('status-text');

async function start() {
    userId = localStorage.getItem('userId');
    if (userId === null) {
        await createUser();
        localStorage.setItem('userId', userId);
    }
    document.getElementById('userid').innerText = 'UserId: ' + userId.toUpperCase();

    deviceId = localStorage.getItem('deviceId');
    if (deviceId === null) { // create device if needed
        await getDevices();
        deviceId = prompt('How do you want name your device?');
        devices.push({
            device: deviceId,
            enableBlocking: true
        });
        await patchDevices();
        localStorage.setItem('deviceId', deviceId);
    }

    if (await isConnected() !== true) {
        currentstatus = UNKNOWN;
    } else if (await isBlocking()) {
        currentstatus = ACTIVE;
    } else {
        currentstatus = INACTIVE;
    }

    await getFilters();
    refreshUI();
}
start();

function toogle() {
    toogleButton.style.animation = 'click 1s';

    if (currentstatus === ACTIVE) {
        currentstatus = INACTIVE;
        disableBlocking();
    } else {
        currentstatus = ACTIVE;
        enableBlocking();
    }
    refreshUI();
    setTimeout(() => {
        toogleButton.style.animation = '';
    }, 1000);
}

function refreshUI() {
    if (currentstatus === ACTIVE) {
        document.body.style.background = 'var(--bg-active)';
        toogleButton.style.backgroundColor = 'var(--active)';

        statusCircle.removeAttribute('inactive');
        statusCircle.setAttribute('active', true);
        statusText.innerText = 'Connected';

        toogleButton.innerText = 'Disconnect';
    } else if (currentstatus === INACTIVE) {
        document.body.style.background = 'var(--bg-inactive)';
        toogleButton.style.backgroundColor = 'var(--inactive)';

        statusCircle.removeAttribute('active');
        statusCircle.setAttribute('inactive', true);
        statusText.innerText = 'Not connected';

        toogleButton.innerText = 'Connect';
    } else {
        document.body.style.background = 'var(--bg-unknown)';
        toogleButton.style.backgroundColor = 'var(--unknown)';

        openInstructions();
    }

    displayFilters();
}

function openInstructions() {
    document.getElementById('main').hidden = true;
    document.getElementById('instructions').hidden = false;
}

function closeInstructions() {
    document.getElementById('main').hidden = false;
    document.getElementById('instructions').hidden = true;
}

function displayFilters() {
    let html = '';

    Object.keys(filters).forEach(filterId => {
            html += `<div clickable onclick="toogleFilter('${filterId}')"><span class="status-circle" ${filters[filterId].enableBlocking ? 'active' : ''} filter="${filters[filterId].filter}"></span> ${filters[filterId].description} ${filters[filterId].enableBlocking ? ' - Active' : ''}</div>`;
    });

    document.querySelector('filters').innerHTML = html;
}

function toogleFilter(filterId) {
    filters[filterId].enableBlocking = !filters[filterId].enableBlocking;
    if (filters[filterId].enableBlocking) {
        enableFilter(filterId);
    } else {
        disableFilter(filterId);
    }

    displayFilters();
}