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
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) is idle.");
        _animal.ClearTarget();
        _animal.ClearMate();
        _animal.currentState = "Idle";
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");
    }

    public override void UpdateState()
    {
        // random roam or flocking pathfinding
        _animal.Wander();
       // _animal.UpdateFear();
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) [IdleCheck] hunger={_animal.hungerLevel:F1} thr={_animal.hungerThreshold:F1}");

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if sleepy > threshold && fear = 0 sleep key

        // if fear > 40 flee key
        // if (_animal.sleepLevel < _animal.sleepThreshold) return AnimalStateMachine.AnimalState.Sleep;

        // if fear > 40 flee key
       if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;

        if (_animal.isAggro) return AnimalStateMachine.AnimalState.Chase;


        // if hunger < 40 findfood key
        if (_animal.isHungry) return AnimalStateMachine.AnimalState.FindFood;

        // if thirst < 40 findwater key

        if (_animal.isThirsty) return AnimalStateMachine.AnimalState.FindWater;

        

        // if breed findmate key
        if (_animal.isBreedable) return AnimalStateMachine.AnimalState.FindMate;

        return StateKey;
    }

}