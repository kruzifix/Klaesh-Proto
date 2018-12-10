# Klaesh-Proto

[Build](https://kruzifix.github.io/Klaesh-Proto-Build/)

## Setup:
- Download & Install UnityHub
- Install Unity 2018.3.0b12
- Clone repo
- Start project with Unity version 2018.3.0b12

## Server:
To run the server locally:
- Install node.js
- go into Klaesh-Server directory
- install packages with `npm i`
- run server with `npm start`

If server is used to host build:
- adjust dir variable in index.js to point to build directory
when running project in unity editor this is not necessary!

## TODO
- You can reduce your startup time if you configure your web server to host .unityweb files using gzip compression.

- use Jobs and a JobManager to handle movement and combat
- MoveToJob, AttackJob, ...

- Navigator that builds 'Nav-Mesh/Map' from properties of Hextiles
- has methods like Flood(origin)
- and then Backtrack(target) -> returns path

- Fog of War!

## NICE TA HAVE
- Give Orders over multiple turns (longer walk path)