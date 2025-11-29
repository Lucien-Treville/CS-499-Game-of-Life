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
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if drink is gone, but still thirsty, go to findwater

        if (_animal.thirstLevel < _animal.thirstThreshold) return AnimalStateMachine.AnimalState.FindWater;
        // if not thirsty, go idle
        if (_animal.thirstLevel > _animal.thirstThreshold) return AnimalStateMachine.AnimalState.Idle;
        return StateKey;
    }
}