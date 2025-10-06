using Godot;
using System;

[GlobalClass]
public partial class CrouchAbility3D : MovementAbility3D
{
    public float SpeedMultiplier { get; set; } = 0.7f;
    
    public CollisionShape3D Collision { get; set; }
    
    public ShapeCast3D HeadCheck { get; set; }

    public float HeightInCrouch = 1.0f;
    public float DefaultHeight = 2.0f;

    /// <summary>
    ///  Used to determine the crouch state
    /// </summary>
    private float _crouchFactor;

    /// Applies slow if crouch is <b>enabled</b>
    public override float GetSpeedModifier()
    {
        return IsActivated() ? SpeedMultiplier : base.GetSpeedModifier();
    }
    
    /// <summary>
    /// Changes players shape according to the <see cref="CrouchAbility3D._crouchFactor"/> 
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="speed"></param>
    /// <param name="isOnFloor"></param>
    /// <param name="direction"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
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
        _crouchFactor = (DefaultHeight - HeightInCrouch) -
                        (shape.Height - HeightInCrouch) / (DefaultHeight - HeightInCrouch); 

        Collision.Shape = shape;
        
        return velocity;
    }
} 
