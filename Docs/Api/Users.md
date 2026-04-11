# Chess api
``` api/users/... ```

**HeaderAuth**: <br>
Header: `{ "username": <string>, "passwordHash": <string> }`

**AdminOnly**: <br>
Header: `{ "adminPassword": <string> }`


## Api

* POST 
    * Body: `{ "username": <string>, "passwordHash": <string> }`

* GET `{username}`
    * **Header Auth**