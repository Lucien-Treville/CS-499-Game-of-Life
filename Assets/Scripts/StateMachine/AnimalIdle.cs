using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalIdle : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalIdle(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Idle");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");
    }

    public override void UpdateState()
    {
        // random roam or flocking pathfinding
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if sleepy > threshold && fear = 0 sleep key
        // if fear > 40 flee key
        // if breed findmate key
        // if thirst < 40 findwater key
        // if hunger < 40 findfood key
        return StateKey;
    }

}
