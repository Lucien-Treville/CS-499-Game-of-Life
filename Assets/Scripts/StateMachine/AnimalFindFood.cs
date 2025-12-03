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
        // First: try to acquire a food target from visible entities
        _animal.FindFood();

        target = _animal.GetTarget();
        _target = _animal.GetTargetEntity();

        // If we still don't have a target, wander around to discover new things
        if (target == null)
        {
            _animal.Wander();
        }
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // Global checks
        if (_animal.isDead)
            return AnimalStateMachine.AnimalState.Dead;

        if (_animal.isScared)
            return AnimalStateMachine.AnimalState.Flee;

        target = _animal.GetTarget();
        _target = _animal.GetTargetEntity();

        // If we lost our target, either keep searching or stop if we're full
        if (target == null || _target == null)
        {
            // If we're no longer hungry, stop searching and idle
            if (_animal.hungerLevel >= _animal.hungerThreshold)
                return AnimalStateMachine.AnimalState.Idle;

            // Still hungry → stay in FindFood
            return StateKey;
        }

        // We have a valid target.
        // If it's a Plant, we can go straight to Eat once in range.
        float dist = _animal.GetTargetDistance();

        if (_target is Plant)
        {
            // Close enough to eat
            if (dist >= 0f && dist <= 2.0f)
                return AnimalStateMachine.AnimalState.Eat;

            // Not close yet → chase it
            return AnimalStateMachine.AnimalState.Chase;
        }
        else
        {
            // Non-plant food (predators, etc.) → let Chase/Attack handle details.
            return AnimalStateMachine.AnimalState.Chase;
        }
    }
}
