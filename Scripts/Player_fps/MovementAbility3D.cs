using Godot;
using System;

[GlobalClass]
public partial class MovementAbility3D : Node3D
{
    // Movement abstract class
    //
    // It contains a flag to enable/disable movement skill, signals emitted when this was modified

    [Export] public bool Active { get; set; }
    private bool _lastActive;

    /// <summary>
    /// <para>Emitted when ability has been active, is called when <see cref="MovementAbility3D.SetActive"/> is set to true</para>
    /// </summary>
    [Signal]
    public delegate void ActivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when ability has been active, is called when <see cref="MovementAbility3D.SetActive"/> is set to true</para>
    /// </summary>
    [Signal]
    public delegate void DeactivatedEventHandler();

    /// <summary>
    /// <para>Returns a speed modifier (default is <b>1.0</b>)</para>
    /// <para>Useful for abilities that when active can change the overall speed of the <see cref="CharacterController3D"/></para>
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
        _lastActive = Active;
        Active = a;
        if (_lastActive != Active)
        {
            var signalName = Active 
                ? SignalName.Activated
                : SignalName.Deactivated;
            EmitSignal(signalName);
        }
    }

    /// <summary>
    /// <para>Change the current velocity of <see cref="CharacterController3D"/></para>
    /// <para>In this function abilities can change the way the character controller behaves based on speed and other
    /// params</para>
    /// </summary>
    public virtual Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        return velocity;
    }
}
