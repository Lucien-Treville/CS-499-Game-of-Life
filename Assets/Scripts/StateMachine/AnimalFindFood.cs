using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindFood : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalFindFood(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindFood");

    }

    public override void ExitState()
    {
        Debug.Log("Exit FindFood");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if food found, eat
        // if fear, flee
        return StateKey;
    }
}
