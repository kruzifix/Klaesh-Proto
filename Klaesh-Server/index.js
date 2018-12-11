const express = require('express')
const app = express()
const server = require('http').Server(app)

const dir = 'D:\\Klaesh-Proto-Build'
//const dir = 'D:\\Klaesh-Proto-Build-Dev'

app.use(express.static(dir))

const board = require('./board.json')

function makeMsg(eventCode, data) {
    return JSON.stringify({
        code: eventCode,
        data: JSON.stringify(data)
    })
}

const WebSocket = require('ws')
const wss = new WebSocket.Server({ server })

const idBuffer = {}
function getId(type) {
    if (!(type in idBuffer))
        idBuffer[type] = 0
    return idBuffer[type]++
}

const clients = {}
const clientsSearchingForGame = []
const games = []

function startGame(game) {
    console.log('starting game')
    console.log(game)

    // send game start to players
    for (let i = 0; i < game.players.length; i++) {
        game.config.home_squad = i
        clients[game.players[i]].socket.send(makeMsg('gamestart', game.config))
    }
    delete game.config.home_squad

    // start first turn
    game.turnNumber = 1
    game.activeSquad = 0
    gameBroadcast(game, makeMsg('startturn', {
        turn_num: game.turnNumber,
        active_squad: game.activeSquad
    }))

    games.push(game)
}

function gameBroadcast(game, msg) {
    for (let id of game.players) {
        clients[id].socket.send(msg)
    }
}

function gameBroadcastToOthers(game, sourcePlayer, msg) {
    for (let id of game.players) {
        if (id != sourcePlayer) {
            clients[id].socket.send(msg)
        }
    }
}

function onEndTurn(userId, code, data) {
    // find game user is in; if any
    let game = games.filter(g => g.players.some(p => p == userId));
    if (game.length != 1) {
        console.log(`got endtturn message from player not in a game! games: ${game.length}`)
        return
    }
    game = game[0]
    // check if player is active player
    if (userId != game.players[game.activeSquad]) {
        console.log('got endtturn message from the wrong player!')
        return
    }
    // check turn number
    if (game.turnNumber != data.turn_num) {
        console.log('wrong turnnumber!')
        return
    }

    // everything checks out!
    // end current turn and start next
    game.turnNumber += 1
    game.activeSquad = (game.activeSquad + 1) % game.players.length
    gameBroadcast(game, makeMsg('startturn', {
        turn_num: game.turnNumber,
        active_squad: game.activeSquad
    }))
    console.log(`starting turn ${game.turnNumber}`)
}

function onMoveUnit(userId, code, data) {
    let game = games.filter(g => g.players.some(p => p == userId));
    if (game.length != 1) {
        console.log(`got moveunit message from player not in a game! games: ${game.length}`)
        return
    }
    game = game[0]

    // check validitiy of msg?

    gameBroadcastToOthers(game, userId, makeMsg(code, data))
}

function handleMsg(userId, code, data) {
    console.log(`${userId} -- ${code}:`)
    console.log(data)

    const handler = {
        'endturn': onEndTurn,
        'moveunit': onMoveUnit
    }
    
    if (code in handler) {
        handler[code](userId, code, data)
    } else {
        console.log(`no handler for code ${code}`)
    }
}

wss.on('connection', socket => {
    const id = getId('client')
    clients[id] = {
        id: id,
        socket: socket,
        ingame: false
    }

    console.log('client connected! id: ' + id)

    clientsSearchingForGame.push(id)

    if (clientsSearchingForGame.length >= 2) {
        const player2 = clientsSearchingForGame.pop()
        const player1 = clientsSearchingForGame.pop()

        const gid = getId('game')
        const game = {
            id: gid,
            config: board,
            players: [player1, player2],
            randomSeed: Math.floor(Math.random() * 10000)
        }
        game.config.id = gid
        game.config.squads[0].id = player1
        game.config.squads[1].id = player2
        game.config.random_seed = game.randomSeed
        startGame(game)
    }

    socket.on('message', msg => {
        console.log('got message from id ' + id)
        data = JSON.parse(msg)
        data.data = JSON.parse(data.data)

        handleMsg(id, data.code, data.data)
    })

    socket.on('error', error => {
        console.log('client with id ' + id + ' threw error!')
        console.log(error)
    })

    socket.on('close', event => {
        if (clients[id]) {
            console.log('closing connection to ' + id)
            delete clients[id]
            
            const idx = clientsSearchingForGame.indexOf(id);
            if (idx >= 0) {
                clientsSearchingForGame.splice(idx, 1);
            }
        }
    })
})

// wss.on('connection', function connection(ws, req) {

//     const parameters = url.parse(req.url, true);

//     ws.uid = wss.getUniqueID();
//     ws.chatRoom = {uid: parameters.query.myCustomID};
//     ws.hereMyCustomParameter = parameters.query.myCustomParam;
// }

server.listen(3000, () => {
    console.log('listening on *:3000')
})