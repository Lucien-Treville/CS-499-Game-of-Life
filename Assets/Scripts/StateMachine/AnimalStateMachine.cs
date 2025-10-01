using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : StateMachine<AnimalStateMachine.AnimalState>
{
    public enum AnimalState
    {
        Idle,
        Sleep,
        FindMate,
        FindWater,
        FindFood,
        Chase,
        Flee,
        Attack,
        Breed,
        Drink,
        Eat,
    }

    void Awake()
    {
        CurrentState = States[AnimalState.Idle];
    }

}
