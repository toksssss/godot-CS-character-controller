using Godot;
using System;

[GlobalClass]
public partial class ClimbingLadderAbility3D : MovementAbility3D
{
    public Area3D CurrentLadderClimbing;
    public HeadMovement3D Head;
    
    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        if (!IsActivated())
        {
            return velocity;
        }
        
        

        return velocity;
    }
}
