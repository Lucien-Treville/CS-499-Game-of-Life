using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAttack : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
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

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if no target in range, go to chase
        return StateKey;
    }
}
