using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEat : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private LivingEntity _target;
    public AnimalEat(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Eat");
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) is eating.");
        _animal.currentState = "Eat";


    }

    public override void ExitState()
    {
        Debug.Log("Exiting Eat");

    }

    public override void UpdateState()
    {
        _target = _animal.GetTargetEntity();
        if (_target != null) _animal.Eat(_target);
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        _target = _animal.GetTargetEntity();

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        if (_animal.isScared) return AnimalStateMachine.AnimalState.Flee;


        // if food is gone, but still hungry, findfood

        if (_animal.isHungry && _target == null) return AnimalStateMachine.AnimalState.FindFood;
        // if not hungry, idle

        if (!_animal.isHungry) return AnimalStateMachine.AnimalState.Idle;

        return StateKey;
    }
}