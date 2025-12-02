using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDrink : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalDrink(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Drink");
        _animal.currentState = "Drink";

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Drink");

    }

    public override void UpdateState()
    {
        _animal.Drink();
        _animal.UpdateFear();

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if fear, flee
        if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;

        // if not thirsty, go idle
        if (!_animal.isThirsty) return AnimalStateMachine.AnimalState.Idle;
        return StateKey;
    }
}