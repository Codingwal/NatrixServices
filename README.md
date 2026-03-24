# NatrixServices

NatrixServices contains multiple independent services hosted on a server. These are supposed to be used by Natrix.

## API

Examples are in Examples/

\<UserId\>, \<DeviceId\>, \<FilterId\> and \<Domain\> are strings.

\[AdminOnly\]: <br>
Header: ``` { "password": <AdminPassword> } ```

### Users
``` api/users/... ```

#### api

GET ``` create ``` ( Returns UserId ) <br>


### DnsBlocker/Config
``` api/dnsblocker/config/... ```

#### Structures

DeviceList:
```
[
    {"device": <DeviceId>, "enableBlocking": <Bool>},
    ...
]
```

FilterReferenceList:
```
[
    {"filter": <FilterId>, "enableBlocking": <Bool>},
    ...
]
```

FilterConfig:
```
{
    "id": <FilterId>,
    "domainsToBlock": [<Domain>, ...]
}
```

FilterConfigs:
```
{
    <FilterId>: <FilterConfig>,
    ...
}
```

#### User api
GET ``` blockingenabled/<UserId>?deviceId=<DeviceId> ``` ( Returns Bool ) <br> 
PATCH ``` blockingenabled/<UserId>?enabled=<Bool>&deviceId=<DeviceId?> ```<br>
GET ``` devices/<UserId> ``` ( Returns DeviceList ) <br>
PATCH ``` devices/<UserId> ``` ( Body: ``` { "devices": <DeviceList>} ``` ) <br>
GET ``` filters/<UserId> ``` ( Returns FilterReferenceList ) <br>
PATCH ``` filters/<UserId> ``` ( Body: ``` { "filters": <FilterReferenceList>} ``` ) <br>

#### Global api
GET ``` global/blockingenabled ``` ( Returns Bool ) <br>
PATCH ``` global/blockingenabled?enabled=<Bool> ``` [AdminOnly] <br>
GET ``` global/dnsenabled ``` ( Returns Bool ) <br>
PATCH ``` global/dnsenabled?enabled=<Bool> ``` [AdminOnly] <br>
GET ``` global/filters ``` ( Returns FilterConfigList ) <br>
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