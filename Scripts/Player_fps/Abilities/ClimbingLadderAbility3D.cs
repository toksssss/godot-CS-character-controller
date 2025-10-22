using Godot;
using System;
using System.Diagnostics.Contracts;

[GlobalClass]
public partial class ClimbingLadderAbility3D : MovementAbility3D
{
    public Area3D CurrentLadderClimbing;
    public HeadMovement3D Head;

    private float _climbSpeed = 10.0f;

    public Vector3 LadderForwardMove;
    public Vector3 LadderSideMove;
    public Transform3D LadderGTransform;
    public Vector3 PosRelToLadder;
    public Vector3 WishDir;

    public float LadderStrafeVel;
    public float LadderClimbVel;

    public bool JumpedOfLadder;
    
    
    // Stores
    public bool WasClimbingLadder;

    public override Vector3 Apply(Vector3 velocity, float speed, bool isOnFloor, Vector3 direction, double delta)
    {
        if (!IsActivated())
        {
            return velocity;
        }
        
        // Strafe velocity is simple. Just take x component rel to ladder of both
        LadderStrafeVel = _climbSpeed * (LadderSideMove.X + LadderForwardMove.X);
        // For climb velocity, there are a few things to take into account:
        // If strafing directly into the ladder, go up, if strafing away, go down
        LadderClimbVel = _climbSpeed * -LadderSideMove.Z;
        // When pressing forward & facing the ladder, the player likely wants to move up. Vice versa with down.
        // So we will bias the direction (up/down) towards where we are looking by 45 degrees to give
        // a greater margin for up/down detect.
        var camForwardAmount = GetNode<Camera3D>("../Head/FirstPersonCameraReference/Camera3D").Basis.Z
            .Dot(CurrentLadderClimbing.Basis.Z);
        var upWish = Vector3.Up.Rotated(new Vector3(1, 0, 0), Mathf.DegToRad(-45 * camForwardAmount))
            .Dot(LadderForwardMove);
        LadderClimbVel +=  _climbSpeed * upWish;    

        // dismount
        // Only begin climbing ladders when moving towards them & prevent sticking to top of ladder when dismounting
        // Trying to best match the player's intention when climbing on ladder
        bool shouldDismount = false;
        if (!WasClimbingLadder)
        {
            var mountingOnTop = PosRelToLadder.Y > 
                                CurrentLadderClimbing.GetNode<Node3D>("TopOfLadder").Position.Y;
            if (mountingOnTop)
            {
                // They could be trying to get on from the top of the ladder, or trying to leave the ladder
                if (LadderClimbVel > 0)
                {
                    shouldDismount = true;
                }
            }
            else
            {
                // If not mounting top, they are either falling or on floor
                // In which case, only stick to ladder if intentionally moving forward
                if (LadderGTransform.Basis.Determinant() != 0)
                {
                    if ((LadderGTransform.AffineInverse().Basis * WishDir).Z < 0)
                    {
                        shouldDismount = true;
                    }
                }
            }
            // // Only stick to the ladder if very close. Helps make it easier to get off top & prevents camera jitter
            // if (Mathf.Abs(PosRelToLadder.Z) > 0.1f)
            // {
            //     shouldDismount = true;
            // }
        }
        
        // Let player step off onto floor
        if (isOnFloor && LadderClimbVel <= 0)
        {
            shouldDismount = true;
        }
        
        if (shouldDismount)
        {
            CurrentLadderClimbing = null;
            return velocity;
        }
        
        // jump
        
        
        GD.Print("Snap");
        
        velocity = LadderGTransform.Basis * new Vector3(LadderStrafeVel, LadderClimbVel, 0);
        
        // Snap player onto ladder
        PosRelToLadder.Z = 0;
        GlobalPosition = LadderGTransform * PosRelToLadder;
        
        return velocity;
    }
}
