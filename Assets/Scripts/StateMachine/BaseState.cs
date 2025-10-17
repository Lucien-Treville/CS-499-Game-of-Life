using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<Estate> where Estate : struct, System.Enum
{
    public Estate StateKey { get; private set; }
    public BaseState(Estate key)
    {
        StateKey = key;
    }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract Estate GetNextState();




}
