using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerModel : Node
{
    // References
    public Player Player;
    
    // Moves
    public Idle Idle;
    public Run Run;
    public Jump Jump;
    // public Crouch Crouch;
    // public FreeLook FreeLook;
    // public Sliding Sliding;

    public Move CurrentMove;

    public Dictionary<string, Move> Moves;
    
        
    public override void _Ready()
    {
        Player = GetNode<Player>("..");
        Idle = GetNode<Idle>("Idle");
        Run = GetNode<Run>("Run");
        Jump = GetNode<Jump>("Jump");
        // Crouch = GetNode<Crouch>("Crouch");
        // FreeLook = GetNode<FreeLook>("FreeLook");

        Moves = new Dictionary<string, Move>()
        {
            {"idle", Idle},
            {"run", Run},
            {"jump", Jump}
            // {"crouch", Crouch},
            // {"free_look", FreeLook},
            // {"sliding", Sliding}
        };

        CurrentMove = Moves["idle"];
        foreach (var move in Moves.Values)
        {
            move.Player = Player;
        }
    }

    public void Update(InputPackage input, double delta)
    {
        var relevance = CurrentMove.CheckRelevance(input);
        if (relevance != "okay")
        {
            SwitchTo(relevance);
        }
        CurrentMove.Update(input, delta);
    }

    public void SwitchTo(string state)
    {
        CurrentMove.OnExitState();
        CurrentMove = Moves[state];
        CurrentMove.OnEnterState();
    }
}
