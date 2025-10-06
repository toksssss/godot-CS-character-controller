using Godot;

[GlobalClass]
public partial class WalkAbility3D : MovementAbility3D
{
    // Basic movement ability
    
    // Time to reach full speed
    [Export] public float Acceleration { get; set; } = 8.0f;
    // Time to stop walking
    [Export] public float Deceleration { get; set; } = 10.0f;

    [Export(PropertyHint.Range, "0.0, 1.0, 0.05")]
    public float AirControl { get; set; } = 0.3f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="speed"></param>
    /// <param name="isOnFloor"></param>
    /// <param name="direction"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        // Check when ability is active. Return current velocity if ability is not active
        if (!IsActivated())
        {
            return velocity;
        }
        
        // Using only the horizontal velocity, interpolate towards the input
        var tempVel = velocity;
        tempVel.Y = 0;
        
        float tempAccel = direction.Dot(tempVel) > 0 ? Acceleration : Deceleration;

        if (!isOnFloor)
        {
            tempAccel *= AirControl;
        }
        
        Vector3 target = direction * speed;
        tempVel = tempVel.Lerp(target, tempAccel * (float)delta);

        velocity.X = tempVel.X;
        velocity.Z = tempVel.Z;

        return velocity;
    }
}
