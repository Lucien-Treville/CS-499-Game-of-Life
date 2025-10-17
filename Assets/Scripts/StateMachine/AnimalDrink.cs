using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDrink : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    public AnimalDrink(AnimalStateMachine machine, AnimalStateMachine.AnimalState key)
        : base(key)
    {
        _machine = machine;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Drink");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Drink");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if drink is gone, but still thirsty, go to findwater
        // if not thirsty, go idle
        return StateKey;
    }
}
