# User api
``` api/users/... ```

**Header Auth**: <br>
Header: `{ "username": <string>, "passwordHash": <string> }`

**Admin Only**: <br>
Header: `{ "adminPassword": <string> }`


## Structures

UserData:
```
{
    "username": <string>,
    "linkedAccount": <string?>
}
```


## Api

* POST 
    * Body: `{ "username": <string>, "passwordHash": <string> }`

* GET `{username}`
    * **Header Auth**
    * Returns: `<UserData>`

* POST `{username}/linked-account`
    * **Admin Only**
    * Body: `{ "account": <string> }`