# Klaesh-Proto

[Build](https://kruzifix.github.io/Klaesh-Proto-Build/)

## Setup:
- Download & Install UnityHub
- Install Unity 2018.2.17f1
- Clone repo
- Start project with Unity version 2018.2.17f1 

## Notes:
- InputSystem (InputMode, MoveInput, AttackInput, ...)
- Preview System for Inputs
- Turn based Game Simulator


ignore this:

### Game Simulator
reference to map and entities/manager

Execute(IEnumerable<IAction>)
? should Actions be simulated sequentially/parallel


enum ActionExecutionState
    Idle
    Executing
    Finished

IAction
    target: IEntity

    state : ActionExecutionState

    StartExecution() : Coroutine

    Animations?

    ??
    waitFor/executeAfter : IAction