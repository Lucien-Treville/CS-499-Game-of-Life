using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalChase : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalChase(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Chase");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Chase");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if in range, attack
        // if no prey in range, find food
        return StateKey;
    }
}
