# Chess api
``` api/chess/... ```


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


## Game api
GET ` games/<GameId> ` ( Returns GameData ) <br>
GET ``` games/<GameId>/board ``` ( Returns ChessBoard ) <br>
GET ``` games/<GameId>/moves ``` ( Returns: ``` { "moves": [<Move>, <Move>, ...] } ``` ) <br>
GET ``` games/<GameId>/allowed-moves?field=<string?> ``` ( Returns: ``` { "moves": [<Move>, <Move>, ...] } ``` ) <br>
GET ``` games?onlyActive=<Bool?>&username=<string?> ``` ( Returns: ``` { "games": [<GameData>, <GameData>, ...] } ``` ) <br>
POST ``` games ``` ( Body: ``` { "isPublic": <Bool>, "timePerPlayer": <int> } ```) <br>
POST ``` games/<GameId>/players ``` [HeaderAuth] <br>
POST ``` games/<GameId>/moves ``` [HeaderAuth] ( Body: Move) <br>

## User api
GET ``` users/<Username> ``` ( Returns UserData ) <br>
GET ``` users/<Username>/games ``` [HeaderAuth] ( Returns: ``` { "games": [<GameData>, <GameData>, ...] } ``` ) <br>
GET ``` users/<Username>/stats ``` ( Returns UserStats ) <br>
