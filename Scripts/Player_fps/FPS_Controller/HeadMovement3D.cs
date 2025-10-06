using Godot;
using System;

[GlobalClass]
public partial class HeadMovement3D : Marker3D
{
    // Node that moves the character's head
    // To move just call the function RotateCamera
    
    /// Mouse sensitivity of rotation move
    public float MouseSensitivity { get; set; } = 2.0f;
    
    /// Vertical angle limit of rotation move
    public float VerticalAngleLimit { get; set; } = 90.0f;
    
    /// Actual rotation of movement
    public Vector3 ActualRotation = Vector3.Zero;

    public override void _Ready()
    {
        ActualRotation.Y = GetOwner<Player>().Rotation.Y;
    }
    
    /// <summary>
    /// Defines mouse sensitivity
    /// </summary>
    /// <param name="sens">Mouse sensitivity</param>
    public void SetMouseSensitivity(float sens)
    {
        MouseSensitivity = sens;
    }
    
    /// <summary>
    /// Defines vertical angle limit for rotation movement of head
    /// </summary>
    /// <param name="limit">angle limit</param>
    public void SetVerticalAngleLimit(float limit)
    {
        VerticalAngleLimit = Mathf.DegToRad(limit);
    }
    
    /// <summary>
    /// <para>Rotates the head of the character that contains camera used by <see cref="FPSController3D"/></para>
    /// <para>Vector2 is sent with reference to the input of a mouse as an example</para>
    /// </summary>
    /// <param name="mouseAxis">Vector2</param>
    public void RotateCamera(Vector2 mouseAxis)
    {
        // Horizontal mouse look
        ActualRotation.Y -= mouseAxis.X * (MouseSensitivity / 1000);
        // Vertical mouse look
        ActualRotation.X = Mathf.Clamp(ActualRotation.X - mouseAxis.Y * (MouseSensitivity / 1000), -VerticalAngleLimit,
            VerticalAngleLimit);

        var rotation = GetOwner<Player>().Rotation;
        rotation.Y = ActualRotation.Y;
        GetOwner<Player>().Rotation = rotation;

        rotation = Rotation;
        rotation.X = ActualRotation.X;
        Rotation = rotation;
    }
}
