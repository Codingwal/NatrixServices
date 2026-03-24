# NatrixServices

**NatrixServices contains multiple independent services hosted on a server. These are supposed to be used by Natrix.**

## API

Examples are in Examples/

### DnsBlocker

DeviceList:
```
[
    {"device": "{deviceId}", "enableBlocking": "{enabled}"},
    ...
]
```

FilterReferenceList:
```
[
    {"filter": "{filterId}", "enableBlocking": "{enabled}"},
    ...
]
```

FilterConfig:
```
{
    "id": "{filterId}",
    "domainsToBlock": ["{domain1}", "{domain2}", ...]
}
```

FilterConfigs:
```
{
    "{filterId}": \<FilterConfig\>
    ...
}
```

\[AdminOnly\]: <br>
Header: { "password" "{adminPassword}" }

*GET config/blockingenabled/{userId}?deviceId={deviceId}* <br>
*PATCH config/blockingenabled/{userId}?enabled={enabled}&deviceId={deviceId}* <br>
*GET config/devices/{userId}* ( Returns: \<DeviceList\> ) <br>
*PATCH config/devices/{userId}* ( Body: { "devices": \<DeviceList\>} ) <br>
*GET config/filters/{userId}* ( Returns: \<FilterReferenceList\> ) <br>
*PATCH config/filters/{userId}* ( Body: { "filters": \<FilterReferenceList\>} ) <br>

*GET config/global/blockingenabled* <br>
*PATCH config/global/blockingenabled?enabled={enabled}* [AdminOnly] <br>
*GET config/global/dnsenabled* <br>
*PATCH config/global/dnsenabled?enabled={enabled}* [AdminOnly] <br>
*GET config/global/filters* ( Body: { "filters": \<FilterConfigList\>} ) <br>
*PATCH config/global/filters/add* [AdminOnly] ( Body: { "filter": \<FilterConfig\> } ) <br>
*PATCH config/global/filters/remove?filterId={filterId}* [AdminOnly] <br>
