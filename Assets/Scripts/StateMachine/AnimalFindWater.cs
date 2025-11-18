using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindWater : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalFindWater(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindWater");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting FindWater");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if drink found, drink
        // if fear, flee
        return StateKey;
    }
}
