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
        Debug.Log($"{_animal.specieName} (ID: {_animal.instanceID}) is fleeing.");
        _animal.currentState = "Flee";

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Flee");

    }

    public override void UpdateState()
    {
        _animal.UpdateFear();
        Vector3 threat = _animal.GetThreat();
        _animal.Flee(threat);


    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if fear < threshold = idle key

        if (!_animal.isScared) return AnimalStateMachine.AnimalState.Idle;
        // else
        return StateKey;
    }
}