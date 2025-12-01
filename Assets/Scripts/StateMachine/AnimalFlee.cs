using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFlee : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalFlee(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        _animal.UpdateFear();
        Transform threat = _animal.GetThreat();
        _animal.Flee(threat);

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if fear < threshold = idle key

        if (_animal.fearLevel < _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Idle;
        // else
        return StateKey;
    }
}