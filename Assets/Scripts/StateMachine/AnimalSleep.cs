using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSleep : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    public AnimalSleep(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Sleep");

    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle");

    }

    public override void UpdateState()
    {
        // increment sleepy
    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if dead, DIE
        if (_animal.isDead) return AnimalStateMachine.AnimalState.Dead;

        // if sleepy = 100 or fear > 10 go to idle
        if (_animal.sleepLevel == 100) return AnimalStateMachine.AnimalState.Sleep;

        return StateKey;
    }
}
