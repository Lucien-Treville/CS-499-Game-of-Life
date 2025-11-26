using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalChase : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Transform target;
    public AnimalChase(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering Chase");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Chase");

    }

    public override void UpdateState()
    {
        target = _animal.GetTarget();

        _animal.PursueTargetTransform(target);

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if in range, attack
        // if no prey in range, find food

        if (target == null) return AnimalStateMachine.AnimalState.Idle;

        return StateKey;
    }
}
