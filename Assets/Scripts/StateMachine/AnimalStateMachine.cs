using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : StateMachine<AnimalStateMachine.AnimalState>
{
    private Animal _animal;
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

    public void setAnimal(Animal animal)
    {
        _animal = animal;
    }

    void Awake()
    {
        States[AnimalState.Idle] = new AnimalIdle(this, AnimalState.Idle, _animal);
        States[AnimalState.Breed] = new AnimalBreed(this, AnimalState.Breed, _animal);
        States[AnimalState.Sleep] = new AnimalSleep(this, AnimalState.Sleep, _animal);
        States[AnimalState.FindMate] = new AnimalFindMate(this, AnimalState.FindMate, _animal);
        States[AnimalState.FindWater] = new AnimalFindWater(this, AnimalState.FindWater, _animal);
        States[AnimalState.FindFood] = new AnimalFindFood(this, AnimalState.FindFood, _animal);
        States[AnimalState.Chase] = new AnimalChase(this, AnimalState.Chase, _animal);
        States[AnimalState.Flee] = new AnimalFlee(this, AnimalState.Flee, _animal);
        States[AnimalState.Attack] = new AnimalAttack(this, AnimalState.Attack, _animal);
        States[AnimalState.Drink] = new AnimalDrink(this, AnimalState.Drink, _animal);
        States[AnimalState.Eat] = new AnimalEat(this, AnimalState.Eat, _animal);



        CurrentState = States[AnimalState.Idle];
    }

}
