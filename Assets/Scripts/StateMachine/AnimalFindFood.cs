using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindFood : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Transform target;
    private LivingEntity _target;
    

    public AnimalFindFood(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindFood");
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) is finding food.");
        _animal.currentState = "FindFood";


    }

    public override void ExitState()
    {
        Debug.Log("Exit FindFood");

    }

    public override void UpdateState()
    {
        _animal.Wander();
        
       // _animal.UpdateFear();
        

        
        
        _animal.FindFood();
        target = _animal.GetTarget();
        _target = _animal.GetTargetEntity();



    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        target = _animal.GetTarget();
        _target = _animal.GetTargetEntity();

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if food found, eat
        // if fear, flee

        if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;

        if (target != null && _target.isDead) return AnimalStateMachine.AnimalState.Eat;

        if (target != null) return AnimalStateMachine.AnimalState.Chase;


        

        return StateKey;
    }
}