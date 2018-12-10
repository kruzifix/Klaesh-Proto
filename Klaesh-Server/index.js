const express = require('express')
const app = express()
const server = require('http').Server(app)

const dir = 'D:\\Klaesh-Proto-Build'
//const dir = 'D:\\Klaesh-Proto-Build-Dev'

app.use(express.static(dir))

const board = require('./board.json')

const WebSocket = require('ws')
const wss = new WebSocket.Server({ server })

wss.on('connection', socket => {
    console.log('client connected!')

    socket.send(JSON.stringify({
        code: "GameStart",
        data: JSON.stringify(board)
    }))

    // socket.on('message', msg => {
    //     console.log('received ' + msg)

    //     // if (msg == 'Hi there') {
    //         socket.send('hello')
    //     // }
    // })
})

server.listen(3000, () => {
    console.log('listening on *:3000')
})