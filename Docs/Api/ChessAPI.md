# Chess API

` api/chess/... `

## Structures

GameData:
```
{
    "gameId": <GameId>,

    "name": <string>,
    "isPublic": <Bool>,
    "status": <GameStatus>,

    "player1": <Username?>,
    "player2": <Username?>,

    "nextPlayer": <char> // 'w' or 'b'

    "timePerPlayer": <double>,
    "timeLeft1": <double>,
    "timeLeft2": <double>,

    "fen": <string>, // Forsyth-Edwards-Notation

    "result": <char?> // optional. 'w' => white won, 'b' => black won, 'd' => draw
}
```

ChessBoard:
```
{
    // Row by row, from top left to bottom right
    // Lowercase => black, uppercase => white, ' ' => empty
    "board":
    [
        ['R', 'N', 'B', 'Q', 'K', ..., 'R'],
        ['P', 'P', ...],
        [' ', ' ', ...],
        ...
        ['r', 'n', ...]
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

DrawOffer:
```
{
    "player": <Username>
}
```

GameStatus: `"active" | "done" | "scheduled" | "waiting"`


## Game API

* GET `games/<GameId>`
    * Returns: `<GameData>`
* GET `games/<GameId>/board`
    * Returns: `<ChessBoard>`
* GET `games/<GameId>/moves`
    * Returns: `{ "moves": [<Move>, <Move>, ...] }`
* GET `games/<GameId>/allowed-moves?field=<string?>`
    * Returns: `{ "moves": [<Move>, <Move>, ...] }`
* GET `games?status=<GameStatus?>&username=<string?>`
    * Returns: `{ "games": [<GameData>, <GameData>, ...] }`
* POST `games`
    * Body: `{ "name": <string>, "isPublic": <Bool>, "timePerPlayer": <int> }`
    * Returns: `{ "gameId": <GameId> }`
* POST `games/<GameId>/players`
    * **Header Auth**
* POST `games/<GameId>/moves`
    * **Header Auth**
    * Body: `<Move>`
* GET `games/<GameId>/draw-offer`
    * Returns: `{ "offer": <DrawOffer?> }`
* POST `games/<GameId>/draw-offer`
    * **Header Auth**
* DELETE `games/<GameId>/draw-offer`
    * **Header Auth**
* POST `games/<GameId>/resign`
    * **Header Auth**

---

## User API
* GET `users/<Username>`
    * Returns: `<UserData>`
* GET `users/<Username>/games?status=<GameStatus?>`
    * **Header Auth**
    * Returns: `{ "games": [<GameData>, <GameData>, ...] }`
* GET `users/<Username>/stats`
    * Returns: `<UserStats>`
* GET `users/<Username>/invites`
    * **Header Auth**
    * Returns: `{ "invites": [<GameInvite>, ...] }`
* POST `users/<Username>/invites`
    * **Header Auth (as different user)**
    * Body: `{ "gameId": <GameId> }`
* DELETE `users/<Username>/invites`
    * **Header Auth**
    * Body: `{ "gameId": <GameId> }`