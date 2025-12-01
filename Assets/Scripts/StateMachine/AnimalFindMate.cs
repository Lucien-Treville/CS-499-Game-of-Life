using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindMate : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalFindMate(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindMate");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting FindMate");

    }

    public override void UpdateState()
    {
        _animal.Wander();
        _animal.UpdateFear();
        Animal mate = _animal.FindBreedTarget();
        

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if fear, flee

        if (_animal.fearLevel > _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Flee;

        // if mate found, breed
        if (_animal.mate != null) return AnimalStateMachine.AnimalState.Breed; 

        // if hungry or thirsty, idle
        if (_animal.thirstLevel < _animal.thirstThreshold) return AnimalStateMachine.AnimalState.Idle;
        if (_animal.hungerLevel < _animal.hungerThreshold) return AnimalStateMachine.AnimalState.Idle;


        return StateKey;
    }
}