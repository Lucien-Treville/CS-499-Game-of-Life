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
        States[AnimalState.Idle] = new AnimalIdle(this, AnimalState.Idle);
        States[AnimalState.Breed] = new AnimalBreed(this, AnimalState.Breed);
        States[AnimalState.Sleep] = new AnimalSleep(this, AnimalState.Sleep);
        States[AnimalState.FindMate] = new AnimalIdle(this, AnimalState.FindMate);
        States[AnimalState.FindWater] = new AnimalBreed(this, AnimalState.FindWater);
        States[AnimalState.FindFood] = new AnimalSleep(this, AnimalState.FindFood);
        States[AnimalState.Chase] = new AnimalIdle(this, AnimalState.Chase);
        States[AnimalState.Flee] = new AnimalBreed(this, AnimalState.Flee);
        States[AnimalState.Attack] = new AnimalSleep(this, AnimalState.Attack);
        States[AnimalState.Drink] = new AnimalIdle(this, AnimalState.Drink);
        States[AnimalState.Eat] = new AnimalBreed(this, AnimalState.Eat);


        CurrentState = States[AnimalState.Idle];
    }

}
