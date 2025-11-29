using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFindFood : BaseState<AnimalStateMachine.AnimalState>
{
    private AnimalStateMachine _machine;
    private Animal _animal;
    private Transform target;
    private string targetTag; 

    public AnimalFindFood(AnimalStateMachine machine, AnimalStateMachine.AnimalState key, Animal animal)
        : base(key)
    {
        _machine = machine;
        _animal = animal;
    }
    public override void EnterState()
    {
        Debug.Log("Entering FindFood");

      
    }

    public override void ExitState()
    {
        Debug.Log("Exit FindFood");

    }

    public override void UpdateState()
    {
        _animal.Wander();
        _animal.FindFood();
        _animal.UpdateFear();

    }

    public override AnimalStateMachine.AnimalState GetNextState()
    {
        // if food found, eat
        // if fear, flee

        if (_animal.fearLevel > _animal.fleeThreshold) return AnimalStateMachine.AnimalState.Flee;

        if (target != null && !_animal.isPredator) return AnimalStateMachine.AnimalState.Eat;

        if (target != null && _animal.isPredator) return AnimalStateMachine.AnimalState.Chase;


        

        return StateKey;
    }
}