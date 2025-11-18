using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSleep : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalSleep(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Sleep");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");

    }

    public override void UpdateState()
    {
        // decrement sleepy
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if sleepy = 0 or fear > 10 go to idle
        return StateKey;
    }
}
