# Dns Blocker API

## Structures

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
    "userId": <UserId>,
    "deviceId": <DeviceId>
}
```

## User API

`api/dnsblocker/...`

#### User data
* GET `users/{userId}/requests/last`
    * Returns: DnsRequest
* GET `users/{userId}/requests/count`
    * Returns: Integer

#### User Blocking Management
* GET `users/{userId}/blocking-state`
    * Returns: Bool
* PUT `users/{userId}/blocking-state`
    * Body: `{ "enabled": <Bool> }`

#### Device Blocking Management
* GET `users/{userId}/devices/{deviceId}/blocking-state`
    * Returns: Bool
* PUT `users/{userId}/devices/{deviceId}/blocking-state`
    * Body: `{ "enabled": <Bool> }`

#### Filter Blocking Management
* GET `users/{userId}/filters/{filterId}/blocking-state`
    * Returns: Bool
* PUT `users/{userId}/filters/{filterId}/blocking-state`
    * Body: `{ "enabled": <Bool> }`

#### Device Management
* GET `users/{userId}/devices`
    * Returns: { <DeviceId>: <DeviceConfig>, ... }
* POST `users/{userId}/devices`
    * Body: `{ "device": <DeviceConfig> }`
* DELETE `users/{userId}/devices/{deviceId}`

#### Filter Management
* GET `users/{userId}/filters`
    * Returns: { <FilterId>: <FilterReference>, ... }
* POST `users/{userId}/filters`
    * Body: `{ "filter": <FilterReference> }`
* DELETE `users/{userId}/filters/{filterId}`

---

### Global API

#### Global data
* `` global/lastrequest `

#### System Configuration
* GET `global/config/blocking-state`
    * Returns: Bool
* PATCH `global/config/blocking-state`
    * **Admin Only**
    * Body: `{ "enabled": <Bool> }`
* GET `global/config/dns-state`
    * Returns: Bool
* PATCH `global/config/dns-state`
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