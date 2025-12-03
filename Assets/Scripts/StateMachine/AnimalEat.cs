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

        // If we somehow lost the target, do nothing this frame;
        // GetNextState will push us out of Eat.
        if (_target == null)
            return;

        _animal.Eat(_target);
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        _target = _animal.GetTargetEntity();

        // Global death/fear checks
        if (_animal.isDead)
            return AnimalStateMachine.AnimalState.Dead;

        if (_animal.isScared)
            return AnimalStateMachine.AnimalState.Flee;

        // If food disappeared (fully eaten or destroyed)
        if (_target == null)
        {
            // Still hungry? Go find more food.
            if (_animal.hungerLevel < 80.0)
                return AnimalStateMachine.AnimalState.FindFood;

            // Otherwise, we’re satisfied.
            return AnimalStateMachine.AnimalState.Idle;
        }

        // If we've reached a high enough hunger level, stop eating and idle.
        if (_animal.hungerLevel >= 80.0)
            return AnimalStateMachine.AnimalState.Idle;

        // Otherwise stay in Eat and keep taking bites on cooldown.
        return StateKey;
    }
}
