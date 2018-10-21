# Klaesh-Proto

## Setup:
- Download UnityHub
- Install Unity 2018.2.12f1
- Clone repo
- Start project with Unity version 2018.2.12f1 

## Notes:
- Make Entities configurable through Scriptable objects
- Entity saves coordinate
- HexTile has reference to current occupying entity
- EntityManager spawns entities (all Scriptable objects in specific folder can be loaded) and keeps track of them
- EntityManager needs reference to Hexmap, maybe make Hexmap available through ServiceLocator?
- HexTile needs IsBlocked() method (or can be walked on)
