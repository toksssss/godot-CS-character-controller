using Godot;
using System;

[GlobalClass]
public partial class CrouchAbility3D : MovementAbility3D
{
    [Export] public float SpeedMultiplier { get; set; } = 0.7f;
    
    [Export] public CollisionShape3D Collision { get; set; }
    
    [Export] public ShapeCast3D HeadCheck { get; set; }

    [Export] public float HeightInCrouch = 1.0f;
    [Export] public float DefaultHeight = 2.0f;

    public float CrouchFactor;

    // applies slow if crouch is enabled
    public override float GetSpeedModifier()
    {
        return IsActivated() ? SpeedMultiplier : base.GetSpeedModifier();
    }
    
    // Set collision height
    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        var shape = (CapsuleShape3D)Collision.Shape;
        
        if (IsActivated())
        {
            shape.Height -= (float)delta * 8; // Smooth crouch. Any number is accepted 
        }
        else if (!HeadCheck.IsColliding())
        {
            shape.Height += (float)delta * 8;
        }

        shape.Height = Mathf.Clamp(shape.Height, HeightInCrouch, DefaultHeight);

        // Crouch state from 0 to 1. 0 - standing, 1 - full crouch
        CrouchFactor = (DefaultHeight - HeightInCrouch) -
                       (shape.Height - HeightInCrouch) / (DefaultHeight - HeightInCrouch); 

        Collision.Shape = shape;
        
        return velocity;
    }
} 
