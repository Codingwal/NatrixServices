# NatrixServices

**NatrixServices contains multiple independent services hosted on a server. These are supposed to be used by Natrix.**

## API

Examples are in Test/

### DnsBlocker

*GET api/dnsblocker/config/\<userId\>* **Gets the current config for the user** <br>
*POST api/dnsblocker/config* **Sets the config for the user** (userId is part of the json) <br>
*GET api/dnsblocker/config/global* **Gets the current global config** (requires admin password in header!) <br>
*POST api/dnsblocker/config/global* **Sets the current global config** (requires admin password in header!) <br>

*GET api/dnsblocker/data/\<userId\>* **Gets the current data for the user** <br>
*POST api/dnsblocker/data* **Sets the data for the user** (userId is part of the json) (requires admin password in header!) <br>
*GET api/dnsblocker/data/global* **Gets the current global data** (requires admin password in header!) <br>
*POST api/dnsblocker/data/global* **Sets the current global data** (requires admin password in header!) <br>