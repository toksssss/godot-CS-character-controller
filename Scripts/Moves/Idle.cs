using Godot;
using System;

[GlobalClass]
public partial class Idle : Move
{
    public override string CheckRelevance(InputPackage input)
    {
        SortListString(input.Actions);
        return input.Actions[0];


        // if (input.Actions.Contains("jump"))
        // {
        //     return "jump";
        // }
        // if (input.InputDir != Vector2.Zero)
        // {
        //     return "run";
        // }
        //
        // return "okay";
    }
}
