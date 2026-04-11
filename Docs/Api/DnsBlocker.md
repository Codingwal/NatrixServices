# Dns Blocker API

`api/dnsblocker/...`

## Structures

\<DeviceId\>, \<FilterId\> and \<Domain\> are strings.

DeviceConfig:
```
{
    "id": <DeviceId>, 
    "enableBlocking": <Bool>
}
```

FilterReference:
```
{
    "id": <FilterId>, 
    "enableBlocking": <Bool>
}
```

FilterConfig:
```
{
    "id": <FilterId>,
    "description": <String>,
    "domainsToBlock": [<Domain>, ...]
}
```

DnsRequest:
```
{
    "time": <TimeStamp>,
    "domain": <Domain>,
    "blocked": <Bool>,
    "username": <Username>,
    "deviceId": <DeviceId>
}
```

BlockingState:
```
{
    "enabled": <Bool>
}
```

## User API

#### User data
* GET `users/<Username>/requests/last`
    * **Header Auth**
    * Returns: `<DnsRequest>`
* GET `users/<Username>/requests/count`
    * **Header Auth**
    * Returns: `<int>`

#### User Blocking Management
* GET `users/<Username>/blocking-state`
    * **Header Auth**
    * Returns: BlockingState
* PUT `users/<Username>/blocking-state`
    * **Header Auth**
    * Body: BlockingState

#### Device Blocking Management
* GET `users/<Username>/devices/{deviceId}/blocking-state`
    * **Header Auth**
    * Returns: BlockingState
* PUT `users/<Username>/devices/{deviceId}/blocking-state`
    * **Header Auth**
    * Body: BlockingState

#### Filter Blocking Management
* GET `users/<Username>/filters/{filterId}/blocking-state`
    * **Header Auth**
    * Returns: BlockingState
* PUT `users/<Username>/filters/{filterId}/blocking-state`
    * **Header Auth**
    * Body: BlockingState

#### Device Management
* GET `users/<Username>/devices`
    * **Header Auth**
    * Returns: `{ <DeviceId>: <DeviceConfig>, ... }`
* POST `users/<Username>/devices`
    * **Header Auth**
    * Body: `{ "device": <DeviceConfig> }`
* DELETE `users/<Username>/devices/{deviceId}`

#### Filter Management
* GET `users/<Username>/filters`
    * **Header Auth**
    * Returns: `{ <FilterId>: <FilterReference>, ... }`
* POST `users/<Username>/filters`
    * **Header Auth**
    * Body: `{ "filter": <FilterReference> }`
* DELETE `users/<Username>/filters/{filterId}`
    * **Header Auth**

---

## Global API

#### Global data
* GET `global/requests/last`
    * **Admin Only**
    * Returns: `<DnsRequest>`
* GET `global/requests/count`
    * **Admin Only**
    * Returns: `<int>`

#### System Configuration
* GET `global/config/blocking-enabled`
    * Returns: `<Bool>`
* PATCH `global/config/blocking-enabled`
    * **Admin Only**
    * Body: `{ "enabled": <Bool> }`
* GET `global/config/dns-enabled`
    * Returns: `<Bool>`
* PATCH `global/config/dns-enabled`
    * **Admin Only**
    * Body: `{ "enabled": <Bool> }`

#### Global Filter Definitions
* GET `global/filters`
    * Returns: `{ <FilterId>: <FilterConfig>, ... }`
* POST `global/filters`
    * **Admin Only**
    * Body: `{ "filter": <FilterConfig> }`
* DELETE `global/filters/{filterId}`
    * **Admin Only**