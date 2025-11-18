using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEat : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalEat(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Eat");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Eat");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if food is gone, but still hungry, findfood
        // if not hungry, idle
        return StateKey;
    }
}
