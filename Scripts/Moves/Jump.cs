using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Jump : Move
{
    // references
    
    public float JumpVelocity = 4.5f;

    public override void _Ready()
    {
    }

    public override string CheckRelevance(InputPackage input)
    {
        // funny one
        // SortListString(input.Actions);
        // return input.Actions[0];
        
        if (Player.IsOnFloor())
        {
            SortListString(input.Actions);
            return input.Actions[0];
        }
        return "okay";

        // if (Player.IsOnFloor())
        // {
        //     if (input.InputDir != Vector2.Zero)
        //     {
        //         return "run";
        //     }
        //
        //     return "idle";
        // }
        //
        // return "okay";
    }

    public override void Update(InputPackage input, double delta)
    {
        var velocity = Player.Velocity;
        velocity += Player.GetGravity() * (float)delta;
        Player.Velocity = velocity;

        Player.MoveAndSlide();
    }

    public override void OnEnterState()
    {
        var velocity = Player.Velocity;
        velocity.Y += JumpVelocity;
        Player.Velocity = velocity;
    }
}
