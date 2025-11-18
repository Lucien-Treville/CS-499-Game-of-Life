using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindMate : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalFindMate(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindMate");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting FindMate");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if mate found, breed
        // if fear, flee
        // if hungry or thirsty, idle
        return StateKey;
    }
}
