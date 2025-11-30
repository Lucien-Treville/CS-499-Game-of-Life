using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindWater : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Transform target;
    public AnimalFindWater(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindWater");
        _animal.ClearTarget();
        _animal.FindWaterSource();
        target = _animal.GetTarget();

    }

    public override void ExitState()
    {
        Debug.Log("Exiting FindWater");

    }

    public override void UpdateState()
    {
        if (target != null) {_animal.PursueTargetTransform(target);}
        _animal.UpdateFear();

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if drink found, drink
        float distanceToTarget = _animal.GetTargetDistance();
        if (distanceToTarget < 2f && distanceToTarget > -1f) // Adjust threshold as needed
        {
            return AnimalStateMachine.AnimalState.Drink;
        }

        if (target == null | _animal.thirstLevel > _animal.thirstThreshold) return AnimalStateMachine.AnimalState.Idle;

        // if fear, flee

        if (_animal.fearLevel > _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Flee;
        return StateKey;
    }
}
