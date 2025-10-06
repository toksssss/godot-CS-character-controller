using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InputPackage : Node
{
    // public bool IsJumping;
    // public bool IsSprinting;
    // public bool IsCrouching;
    // public bool IsFreeLooking;

    public List<string> Actions = new();
    
    public Vector2 InputDir;
}
