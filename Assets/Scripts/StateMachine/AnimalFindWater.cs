using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindWater : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalFindWater(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindWater");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting FindWater");

    }

    public override void UpdateState()
    {
        _animal.Wander();
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if drink found, drink
        // if fear, flee

        if (_animal.fearLevel > _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Flee;
        return StateKey;
    }
}
