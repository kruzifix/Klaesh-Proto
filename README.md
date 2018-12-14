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
- You can reduce your startup time if you configure your web server to host .unityweb files using gzip compression.

- on hover entity/tile (in inputstate)
- action point system?
- floating/scrolling text in screen and worldspace (i.e. above unit)

- Navigator that builds 'Nav-Mesh/Map' from properties of Hextiles
- has methods like Flood(origin)
- should then consider tile properties (bigger weight for forest, etc)
- and then Backtrack(target) -> returns path

- Fog of War!

## NICE TA HAVE
- Give Orders over multiple turns (longer walk path)

## Games for Inspiration
- Endless Legend
- Shardbound
- For The King
- Northgard
- Bad North