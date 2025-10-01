using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<Estate> : MonoBehaviour where Estate : enum
{
    protected Dictionary<Estate, BaseState<Estate>> States = new Dictionary<Estate, BasteState<Estate>>();

    protected BaseState<Estate> Current State;

    protected bool IsTransitioningState = false;
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        Estate nextStateKey = CurrentState.GetNextState();

        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        } else if (!IsTransitioningState)
        {
            TransitionToState(nextStateKey)
        }
    }

    public void TransitionToState(Estate key)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[key];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }
}