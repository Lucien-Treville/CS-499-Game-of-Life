using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalIdle : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalIdle(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Idle");
        _animal.ClearTarget();
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");
    }

    public override void UpdateState()
    {
        // random roam or flocking pathfinding
        _animal.Wander();
        _animal.UpdateFear();
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if sleepy > threshold && fear = 0 sleep key

        // if fear > 40 flee key
        if (_animal.sleepLevel < _animal.sleepThreshold) return AnimalStateMachine.AnimalState.Sleep;

        // if fear > 40 flee key
        if (_animal.fearLevel > _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Flee;

        // if breed findmate key


        // if thirst < 40 findwater key

        if (_animal.thirstLevel < _animal.thirstThreshold) return AnimalStateMachine.AnimalState.FindWater;

        // if hunger < 40 findfood key
        if (_animal.hungerLevel < _animal.hungerThreshold) return AnimalStateMachine.AnimalState.FindFood;

        return StateKey;
    }

}