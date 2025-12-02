using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDead : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalDead(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Dead");
        _animal.ClearTarget();
        _animal.ClearMate();
        _animal.currentState = "Dead";
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");
    }

    public override void UpdateState()
    {
        // no updates
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // no state changes


        return StateKey;
    }

}