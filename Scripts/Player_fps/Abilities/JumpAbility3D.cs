using Godot;

[GlobalClass]
public partial class JumpAbility3D: MovementAbility3D
{
    [Export] public float Height { get; set; } = 10.0f;
    
    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        if (IsActivated())
        {
            velocity.Y = Height;
        }
        return velocity;
    }
}
