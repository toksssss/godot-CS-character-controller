using Godot;
using System;

[GlobalClass]
public partial class MovementAbility3D : Node3D
{
    // Movement abstract class
    //
    // It contains a flag to enable/disable movement skill, signals emitted when this was modified

    [Export] public bool Active { get; set; }
    public bool LastActive;

    /// <summary>
    /// <para>Emitted when ability has been active, is called when <b>SetActive()</b> is set to true</para>
    /// </summary>
    [Signal]
    public delegate void ActivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when ability has been active, is called when <b>SetActive()</b> is set to false</para>
    /// </summary>
    [Signal]
    public delegate void DeactivatedEventHandler();

    /// <summary>
    /// <para>Returns a speed modifier,</para>
    /// <para>Useful for abilities that when active can change the overall speed of the <b>CharacterController3D</b></para>
    /// </summary>
    public virtual float GetSpeedModifier()
    {
        return 1.0f;
    }

    /// <summary>
    /// <para>Returns true if ability is active</para>
    /// </summary>
    public bool IsActivated()
    {
        return Active;
    }

    /// <summary>
    /// <para>Defines when or not to activate the ability</para>
    /// </summary>
    public void SetActive(bool a)
    {
        LastActive = Active;
        Active = a;
        if (LastActive != Active)
        {
            if (Active)
            {
                EmitSignal(SignalName.Activated);
            }
            else
            {
                EmitSignal(SignalName.Deactivated);
            }
        }
    }

    /// <summary>
    /// <para>Change the current velocity of <b>CharacterController3D</b></para>
    /// <para>In this function abilities can change the way the character controller behaves based on speed and other
    /// params</para>
    /// </summary>
    public virtual Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        return velocity;
    }
}
