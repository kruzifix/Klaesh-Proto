const url = require('url')
const express = require('express')
const app = express()
const server = require('http').Server(app)

const idgen = require('./idgen.js')

const dir = 'C:\\Dev\\Klaesh-Proto-Build'
//const dir = 'D:\\Klaesh-Proto-Build-Dev'

app.use(express.static(dir))

const WebSocket = require('ws')
const wss = new WebSocket.Server({ server })

const board = require('./board.json')
const clients = {}
const clientsSearchingForGame = []
const games = []

function createGame(player1, player2) {
    const gid = idgen('game')
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
    game.config.map.noffset = game.randomSeed

    return game
}

function makeMsg(eventCode, data) {
    return JSON.stringify({
        code: eventCode,
        data: JSON.stringify(data)
    })
}

function startGame(game) {
    console.log('starting game')
    console.log(game)

    // send game start to players
    for (let i = 0; i < game.players.length; i++) {
        game.config.home_squad = i
        // todo: handle disconnect here!
        const pid = game.players[i]
        // KI GAME
        if (pid == -1)
            continue;
        clients[pid].socket.send(makeMsg('gamestart', game.config))
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

function abortGame(game, reason) {
    console.log(`aborting game with id ${game.id}\nreason: ${reason}`)

    // send game abort message to all clients still connected
    gameBroadcast(game, makeMsg('gameabort', { reason: reason }))

    const idx = games.indexOf(game)
    if (idx >= 0) {
        games.splice(idx, 1)
    }
}

function getGameFromUser(userId) {
    let res = games.filter(g => g.players.some(p => p == userId))
    if (res.length > 1) {
        throw new Error('PLAYER IS IN TWO GAMES. WTF??!?')
    } else if (res.length == 0) {
        // player not in game
        return null
    }
    return res[0]
}

function gameBroadcast(game, msg) {
    for (let id of game.players) {
        if (clients[id]) {
            clients[id].socket.send(msg)
        }
    }
}

function gameBroadcastToOthers(game, sourcePlayer, msg) {
    for (let id of game.players) {
        if (id != sourcePlayer) {
            if (clients[id]) {
                clients[id].socket.send(msg)
            }
        }
    }
}

function onEndTurn(userId, code, data) {
    // find game user is in; if any
    let game = getGameFromUser(userId)
    if (!game) {
        console.log(`got endtturn message from player not in a game! games: ${game.length}`)
        return
    }
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

    // KI GAME, advance to real player
    while (game.players[game.activeSquad] == -1) {
        game.activeSquad = (game.activeSquad + 1) % game.players.length
    }

    gameBroadcast(game, makeMsg('startturn', {
        turn_num: game.turnNumber,
        active_squad: game.activeSquad
    }))
    console.log(`starting turn ${game.turnNumber}`)
}

function onDoJob(userId, code, data) {
    let game = getGameFromUser(userId)
    if (!game) {
        console.log(`got dojob message from player not in a game! games: ${game.length}`)
        return
    }
    // check validitiy of msg?

    gameBroadcastToOthers(game, userId, makeMsg(code, data))
}

function handleMsg(userId, code, data) {
    console.log(`${userId} -- ${code}:`)
    console.log(data)

    const handler = {
        'endturn': onEndTurn,
        'dojob': onDoJob
    }
    
    if (code in handler) {
        handler[code](userId, code, data)
    } else {
        console.log(`no handler for code ${code}`)
    }
}

wss.on('connection', (socket, req) => {
    const id = idgen('client')
    clients[id] = {
        id: id,
        socket: socket,
        ingame: false
    }

    console.log('client connected! id: ' + id)

    const params = url.parse(req.url, true)
    if (params.query.ki !== undefined) {
        console.log('starting ki game!')

        const game = createGame(id, -1)
        startGame(game)
    } else {
        clientsSearchingForGame.push(id)

        if (clientsSearchingForGame.length >= 2) {
            const player2 = clientsSearchingForGame.pop()
            const player1 = clientsSearchingForGame.pop()

            const game = createGame(player1, player2)
            startGame(game)
        }
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

            // if client is in game, abort it!
            const game = getGameFromUser(id);
            if (game) {
                abortGame(game, `user with id ${id} disconnected`)
            }
        }
    })
})

server.listen(3000, () => {
    console.log('listening on *:3000')
})