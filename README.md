# NatrixServices

NatrixServices contains multiple independent services hosted on a server. These are supposed to be used by Natrix.

## API

Examples are in Examples/

\<UserId\>, \<DeviceId\>, \<FilterId\> and \<Domain\> are strings.

\[AdminOnly\]: <br>
Header: ``` { "password": <AdminPassword> } ```


### DnsBlocker
``` api/dnsblocker ```

POST ``` createuser ``` ( Returns UserId ) <br>


### DnsBlocker/Config
``` api/dnsblocker/config/... ```

#### Structures

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


#### User api
GET ``` blockingenabled/<UserId>?deviceId=<DeviceId>&filterId=<FilterId> ``` ( Returns Bool ) <br> 
PATCH ``` blockingenabled/<UserId>?enabled=<Bool>&deviceId=<DeviceId?>&filterId=<FilterId> ```<br>
GET ``` devices/<UserId> ``` ( Returns: ``` {<DeviceId>: <DeviceConfig>, ...} ``` ) <br>
PATCH ``` devices/add/<UserId> ``` ( Body: ``` { "device": <DeviceConfig>} ``` ) <br>
PATCH ``` devices/remove/<UserId>?deviceId=<DeviceId> ``` <br>
GET ``` filters/<UserId> ``` ( Returns: ``` {<FilterId>: <FilterReference>, ...} ``` ) <br>
PATCH ``` filters/add/<UserId> ``` ( Body: ``` { "filter": <FilterReference>} ``` ) <br>
PATCH ``` filters/remove/<UserId>?filterId=<FilterId> ``` <br>

#### Global api
GET ``` global/blockingenabled ``` ( Returns Bool ) <br>
PATCH ``` global/blockingenabled?enabled=<Bool> ``` [AdminOnly] <br>
GET ``` global/dnsenabled ``` ( Returns Bool ) <br>
PATCH ``` global/dnsenabled?enabled=<Bool> ``` [AdminOnly] <br>
GET ``` global/filters ``` ( Returns: ``` {<FilterId>: <FilterConfig>, ...} ``` ) <br>
PATCH ``` global/filters/add ```[AdminOnly] ( Body: ``` { "filter": <FilterConfig> } ``` ) <br>
PATCH ``` global/filters/remove?filterId=<FilterId> ``` [AdminOnly] <br>


### DnsBlocker/Data
``` api/dnsblocker/data/... ```

#### Structures

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

#### User api
GET ``` lastrequest/<UserId> ``` ( Returns DnsRequest ) <br>
GET ``` dnsrequestcount/<UserId> ``` ( Returns Integer ) <br>

#### Global api
GET ``` global/lastrequest/<UserId> ``` [AdminOnly] ( Returns DnsRequest ) <br>
GET ``` global/dnsrequestcount/<UserId> ``` [AdminOnly] ( Returns Integer ) <br>