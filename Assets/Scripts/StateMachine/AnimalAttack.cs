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
        Debug.Log("Entering Attack");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Attack");

    }

    public override void UpdateState()
    {
        _target = _animal.GetTargetEntity();

        _animal.AttackAnimal(_target);
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {

        // if dead, DIE
        if(_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if no target in range, go to chase
        _target = _animal.GetTargetEntity();
        target = _animal.GetTarget();
        float distanceToTarget = _animal.GetTargetDistance();

        if (_target.isCorpse && distanceToTarget < 2f && distanceToTarget != -1f) return AnimalStateMachine.AnimalState.Eat;

        if (_target == null | target == null) return AnimalStateMachine.AnimalState.Idle;

        if (distanceToTarget > 2f && distanceToTarget != -1f) // Adjust threshold as needed
        {
            return AnimalStateMachine.AnimalState.Chase;
        }



        return StateKey;
    }
}
