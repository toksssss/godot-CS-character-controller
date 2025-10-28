using Godot;
using System;
using System.Diagnostics.Contracts;

[GlobalClass]
public partial class ClimbingLadderAbility3D : MovementAbility3D
{
    public Area3D CurrentLadderClimbing;
    public HeadMovement3D Head;

    private float _climbSpeed = 10.0f;
    
    public Vector3 WishDir;

    public bool JumpedOfLadder;
    
    
    // Stores
    public bool WasClimbingLadder;

    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        if (!IsActivated())
        {
            return velocity;
        }
        
        var ladderGTransform = CurrentLadderClimbing.GlobalTransform;
        
        Vector3 posRelToLadder;
        if (ladderGTransform.Basis.Determinant() != 0)
        {
            posRelToLadder = CurrentLadderClimbing.GlobalTransform.AffineInverse() * GlobalPosition;
        }
        else
        {
            posRelToLadder = CurrentLadderClimbing.GlobalTransform.Inverse() * GlobalPosition;
        }

        var camera = GetNode<Camera3D>("../Head/FirstPersonCameraReference/Camera3D");

        var forwardMove = Input.GetActionStrength("move_forward") - Input.GetActionStrength("move_back");
        var sideMove = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        var ladderForwardMove = ladderGTransform.AffineInverse().Basis *
                                GetNode<Camera3D>("../Head/FirstPersonCameraReference/Camera3D")
                                    .GlobalTransform.Basis * new Vector3(0, 0, -forwardMove);
        var ladderSideMove = ladderGTransform.AffineInverse().Basis *
                             GetNode<Camera3D>("../Head/FirstPersonCameraReference/Camera3D")
                                 .GlobalTransform.Basis * new Vector3(sideMove, 0, 0);

        var ladderStrafeVel = _climbSpeed * (ladderSideMove.X + ladderForwardMove.X);
        var ladderClimbVel = _climbSpeed * -ladderSideMove.Z;

        var camForwardAmount = GetNode<Camera3D>("../Head/FirstPersonCameraReference/Camera3D").Basis.Z
            .Dot(CurrentLadderClimbing.Basis.Z);
        var upWish = Vector3.Up.Rotated(new Vector3(1, 0, 0), Mathf.DegToRad(-45 * camForwardAmount))
            .Dot(ladderForwardMove);
        ladderClimbVel +=  _climbSpeed * upWish;    
        
        var shouldDismount = isOnFloor && ladderClimbVel <= 0;

        if (shouldDismount)
        {
            CurrentLadderClimbing = null;
            return velocity;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            velocity = CurrentLadderClimbing.GlobalTransform.Basis.Z * 15;
            CurrentLadderClimbing = null;
            return velocity;
        }

        velocity = ladderGTransform.Basis * new Vector3(ladderStrafeVel, ladderClimbVel, 0);

        posRelToLadder.Z = 0;
        GlobalPosition = ladderGTransform * posRelToLadder;

        return velocity;
    }
}
