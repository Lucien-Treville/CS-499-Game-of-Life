using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBreed : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Animal _mate;
    public AnimalBreed(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Breed");
        _mate = _animal.mate;
        _animal.currentState = "Breed";
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Breed");
        _animal.ClearMate();
        _animal.isBreedable = false;
        _mate.isBreedable = false;


    }

    public override void UpdateState()
    {
        if (_mate != null)
            _animal.StartBreed(_mate);
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        if (_mate.isDead) return AnimalStateMachine.AnimalState.Idle;

        // Stay in Breed while the animal is performing the mating coroutine
        if (_animal.isBreeding) return StateKey;

        if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;

        if (_animal.isHungry) return AnimalStateMachine.AnimalState.FindFood;

        if (_animal.isThirsty) return AnimalStateMachine.AnimalState.FindWater;


        // If mating handshake hasn't started but mate still present, stay (or attempt again)
        if (_animal.mate != null) return StateKey;

        // Otherwise no mate -> go Idle
        return AnimalStateMachine.AnimalState.Idle;
    }
}