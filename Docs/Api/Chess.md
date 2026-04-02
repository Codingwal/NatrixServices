# Chess API

``` api/chess/... `

## Structures

GameData:
```
{
    "gameId": <GameId>,
    "isPublic": <Bool>,

    "player1": <Username?>,
    "player2": <Username?>,
    "timeLeft1": <int>, // In minutes
    "timeLeft2": <int>, // In minutes

    "fen": <string>, // Forsyth-Edwards-Notation

    "result": <char?> // optional. 'w' => white won, 'b' => black won, 'd' => draw
}
```

ChessBoard:
```
{
    // Row by row, from top left to bottom right
    // Lowercase => black, upppercase => white, ' ' => empty
    "board":
    [
        ['r', 'n', 'b', ..., 'r'],
        ['p', 'p', ...],
        [' ', ' ', ...],
        ...
        ['R', 'N', ...]
    ]
}
```

UserStats:
```
{
    "gamesPlayed": <int>,
    "gamesWon": <int>,
    "winRate": <string>,
    ...
}
```

UserData:
```
{
    "username": <Username>,
    "title": <string>,
    "titleDescription": <string>,
    "seasonsWon": <int>,
    "tournamentsWon": <int>
}
```

Move:
```
{
    "from": <string>,
    "to": <string>,
    "promotion": <char?>
}
```


## Game API

* GET `games/<GameId>`
    * Returns: `<GameData>`
* GET `games/<GameId>/board`
    * Returns: `<ChessBoard>`
* GET `games/<GameId>/moves`
    * Returns: `{ "moves": [<Move>, <Move>, ...] }`
* GET `games/<GameId>/allowed-moves?field=<string?>`
    * Returns: `{ "moves": [<Move>, <Move>, ...] }`
* GET `games?onlyActive=<Bool?>&username=<string?>`
    * Returns: `{ "games": [<GameData>, <GameData>, ...] }`
* POST `games`
    * Body: `{ "isPublic": <Bool>, "timePerPlayer": <int> }`
* POST `games/<GameId>/players`
    * **Header Auth**
* POST `games/<GameId>/moves`
    * **Header Auth**
    * Body: `<Move>`

---

## User API
* GET `users/<Username>`
    * Returns: `<UserData>`
* GET `users/<Username>/games`
    * **Header Auth**
    * Returns: `{ "games": [<GameData>, <GameData>, ...] }`
* GET `users/<Username>/stats`
    * Returns: `<UserStats>`