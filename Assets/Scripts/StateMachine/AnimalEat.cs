using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEat : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalEat(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Eat");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Eat");

    }

    public override void UpdateState()
    {

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if food is gone, but still hungry, findfood
        // if not hungry, idle

        if (_animal.hungerLevel > _animal.hungerThreshold) return AnimalStateMachine.AnimalState.Idle;

        return StateKey;
    }
}