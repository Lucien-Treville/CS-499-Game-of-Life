using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAttack : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private LivingEntity _target;
    private Transform target;
    public AnimalAttack(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        // Debug.Log("Entering Attack");
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) is attacking.");
        _target = _animal.GetTargetEntity();
        target = _animal.GetTarget();
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Attack");

    }

    public override void UpdateState()
    {
        _target = _animal.GetTargetEntity();
        target = _animal.GetTarget();


        _animal.AttackAnimal(_target);
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {

        _target = _animal.GetTargetEntity();
        target = _animal.GetTarget();

        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if no target in range, go to chase

        if (_target == null || target == null) return AnimalStateMachine.AnimalState.Idle;

        float distanceToTarget = _animal.GetTargetDistance();

        // do NOT transition to Eat � clear the target and return to Idle.
        if (_target is Animal targetAnimal && targetAnimal.isDead && !_animal.isPredator)
        {
            return AnimalStateMachine.AnimalState.Idle;
        }
        if (_target.isCorpse && distanceToTarget < 2f && distanceToTarget != -1f) return AnimalStateMachine.AnimalState.Eat;



        if (distanceToTarget > 2f && distanceToTarget != -1f) // Adjust threshold as needed
        {
            return AnimalStateMachine.AnimalState.Chase;
        }



        return StateKey;
    }
}