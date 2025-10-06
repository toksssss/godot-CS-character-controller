using Godot;
using System;
using Godot.Collections;

public partial class StateMachine : Node
{
    [Export] public State InitialState { get; set; }
	
    public State CurrentState;
    public Dictionary<string, State> States = new();

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is State state)
            {
                States[state.Name.ToString().ToLower()] = state;
                state.Transitioned += OnChildTransition;
            }
        }

        if (InitialState != null)
        {
            InitialState.Enter();
            CurrentState = InitialState;
        }
    }

    public override void _Process(double delta)
    {
        CurrentState?.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }

    public void OnChildTransition(State oldState, string newStateName)
    {
        if (oldState != CurrentState)
        {
            return;
        }

        var newState = States[newStateName.ToLower()];
        if (newState == null)
        {
            return;
        }
		
        CurrentState?.Exit();
        newState.Enter();
        
        CurrentState = newState;
    }
}
