using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindMate : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Animal _mate;
    public AnimalFindMate(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        //Debug.Log("Entering FindMate");
        _mate = null;
        _animal.currentState = "FindMate";


    }

    public override void ExitState()
    {
        //Debug.Log("Exiting FindMate");


    }

    public override void UpdateState()
    {

      //  _animal.UpdateFear();
        _mate = _animal.FindBreedTarget();
        if (_mate == null) { _animal.Wander(); }
        if (_mate != null)
        {
            _animal.MoveTo(_mate.transform.position);
        }

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        //Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) [FindMateCheck] hunger={_animal.hungerLevel:F1} thr={_animal.hungerThreshold:F1}");

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if fear, flee

        if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;

        if (_animal.isHungry) return AnimalStateMachine.AnimalState.FindFood;

        if (_animal.isThirsty) return AnimalStateMachine.AnimalState.FindWater;

        // if hungry or thirsty, idle
        if (!_animal.isBreedable) return AnimalStateMachine.AnimalState.Idle;
        
        // if mate found, breed
        if (_animal.mate != null) return AnimalStateMachine.AnimalState.Breed;



        return StateKey;
    }
}