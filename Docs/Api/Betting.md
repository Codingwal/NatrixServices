# Betting API

`api/betting/...`

## Structures

Bet:
```
{
    "matchId": <MatchId>,
    "stake": <uint>,
    "player1": <uint>,
    "player2": <uint>
}
```

CreateMatchRequest:
```
{
    "name": <string>,
    "event": <string>,
    "player1": <string>,
    "player2": <string>,
    "startTime": <DateTime>,
    "oddsPlayer1": <float>,
    "oddsDraw": <float>,
    "oddsPlayer2": <float>
}
```

MatchResult:
```
{
    "player1": <uint>,
    "player2": <uint>
}
```

MatchInfo:
```
{
    "matchId": <MatchId>,
    "name": <string>,
    "event": <string>,
    "player1": <string>,
    "player2": <string>,
    "startTime": <DateTime>,
    "oddsPlayer1": <float>,
    "oddsDraw": <float>,
    "oddsPlayer2": <float>
}
```

UserData:
```
{
    "username": <string>,
    "balance": <float>
}
```

Status: "open" |"done"

## Betting API

* POST `bets`
    * **Header Auth**
    * Body: `<Bet>`

## Match API

* POST `matches`
    * **Admin Only**
    * Body: `<CreateMatchRequest>`
    * Returns: `{ "matchId": <MatchId> }`

* POST `matches/<MatchId>/result`
    * **Admin Only**
    * Body: `<MatchResult>`

* GET `matches?status=<Status?>&event=<string?>`
    * Returns: `{ "matches": [<MatchInfo>, ...] }`


## User API

* GET `users/<string>`
    * **Header Auth**
    * Returns: `<UserData>`

* GET `users/bets?status=<Status?>`
    * **Header Auth**
    * Returns: `{ "bets": [<Bet>, ...] }`