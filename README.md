# Klaesh-Proto

[Build](https://kruzifix.github.io/Klaesh-Proto-Build/)

## Setup:
- Download & Install UnityHub
- Install Unity 2018.3.0f2
- Clone repo
- Start project with Unity version 2018.3.0f2

## Server:
To run the server locally:
- Install node.js
- go into Klaesh-Server directory
- install packages with `npm i`
- run server with `npm start`

If server is used to host WEBGL build:
- adjust dir variable in index.js to point to WEBGL build directory

### For Testing:
the editor can connect to the local server!

But you need a second client to test!
For this: build the project as Windows Standalone (this is faster than a webgl build) and just start it!

It should connect to the local server and you can test your stuff! awesome!


## TODO
- move camera with wasd?
- action point system?
- add EntityWidget as Field to Entity Monobehaviour? or make an additional component for that? who knows..
- Hextile Properties (forest, etc.) -> HexNav has to consider these rules!
- Pushen und Pullen!!! => Shape editor für Unity!!
- Fog of War!

## Notes
- You can reduce your startup time if you configure your web server to host .unityweb files using gzip compression.

## NICE TA HAVE
- Give Orders over multiple turns (longer walk path)

## Games for Inspiration
- Endless Legend
- Shardbound
- For The King
- Northgard
- Bad North
