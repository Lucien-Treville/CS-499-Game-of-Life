using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFlee : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalFlee(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        return StateKey;
    }
}
