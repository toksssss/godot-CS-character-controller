using Godot;

[GlobalClass]
public partial class JumpAbility3D: MovementAbility3D
{
    [Export] public float Height { get; set; } = 10.0f;
    
    /// <summary>
    /// Applies jump height to the character
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="speed"></param>
    /// <param name="isOnFloor"></param>
    /// <param name="direction"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        if (IsActivated())
        {
            velocity.Y = Height;
        }
        return velocity;
    }
}
