const api = '/api/dnsblocker/';

let userId = 'test';
let deviceId = 'best_device_of_the_world';

let devices = [];
let filters = [];

async function request(type, url, content = {}) {
    let options = {
        method: type,
        headers: {
            "Content-Type": "application/json"
        }
    };

    if (type != 'GET') {
        options.body = JSON.stringify(content);
    }

    const response = await fetch(api + url, options);
    return response.text();
}

async function createUser() {
    userId = await request('POST', 'createuser');
}

async function getDevices() {
    let response = await request('GET', 'config/devices/' + userId);

    response = JSON.parse(response);

    devices = response;
}

async function addDevice(device) {
    await request('PATCH', 'config/filters/' + userId, {
        device: device
    });
}

async function removeDevice(deviceId) {
    await request('PATCH', 'config/filters/' + userId + '?deviceId=' + deviceId);
}

async function isConnected() {
    let response = await request('GET', 'data/lastrequest/' + userId);
    response = JSON.parse(response);
    
    if (
        response.deviceId == deviceId &&
        (new Date - new Date(response.time) / 1000) < 60
    ) {
        return true;
    } else {
        return false;
    }
}

async function isBlocking() {
    return await request('GET', 'config/blockingenabled/' + userId + '?deviceId=' + deviceId);
}

async function enableBlocking() {
    return await request('PATCH', 'config/blockingenabled/' + userId + '?enabled=true&deviceId=' + deviceId);
}

async function disableBlocking() {
    return await request('PATCH', 'config/blockingenabled/' + userId + '?enabled=false&deviceId=' + deviceId);
}

async function getFilters() {
    filters = await request('GET', 'config/filters/' + userId);
    filters = JSON.parse(filters);
}

async function addFilter(filter) {
    await request('PATCH', 'config/filters/' + userId, {
        filter: filter
    });
}

async function removeFilter(filterId) {
    await request('PATCH', 'config/filters/' + userId + '?filterId=' + filterId);
}