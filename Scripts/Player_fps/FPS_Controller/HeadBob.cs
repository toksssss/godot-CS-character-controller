using Godot;
using System;

[GlobalClass]
public partial class HeadBob : Node
{
    // Head bob effect for the camera
    
    // Node that will receive the headbob effect
    [Export] public Node3D Head;

    [ExportGroup("Step Bob")]

    // Enables the headbob effect for the steps taken
    [Export] public bool StepBobEnabled { get; set; } = true;
    
    // Maximum range value of headbob
    [Export] public Vector2 BobRange { get; set; } = new(0.07f, 0.07f);
    
    // Curve where bob happens
    [Export] public Curve BobCurve { get; set; }
    
    // Curve Multiplier
    [Export] public Vector2 CurveMultiplier { get; set; } = new(2, 2);
    
    // Difference of step headbob movement between vertical and horizontal angle
    [Export] public int VerticalHorizontalRatio { get; set; } = 2;

    [ExportGroup("Jump Bob")]

    // Enables bob for made jumps
    [Export] public bool JumpBobEnabled { get; set; } = true;
    
    // Resource that stores information from bob lerp jump
    [Export] public TimedBobCurve TimedBobCurve { get; set; }

    [ExportGroup("Rotation To Move (Quake Like)")]

    // Enables camera angle for the direction the character controller moves
    [Export] public bool RotationToMove = true;
    
    // Speed at which the camera angle moves
    [Export] public float SpeedRotation { get; set; }= 4.0f;
    
    // Rotation angle limit per move
    [Export] public float AngleLimitForRotation { get; set; } = 0.1f;
    
    // Actual speed of headbob
    public float Speed;
    
    // Store original position of head for headbob reference
    public Vector3 OriginalPosition;
    
    // Store original rotation of head for headbob reference
    public Quaternion OriginalRotation;
    
    // Actual cycle x of step headbob
    public float CyclePositionX;
    
    // Actual cycle Y of step headbob
    public float CyclePositionY;
    
    // Actual interval of step headbob
    public float StepInterval;

    public override void _Ready()
    {
        OriginalPosition = Head.Position;
        OriginalRotation = Head.Quaternion;
    }

    public void SetupStepBob(float stepInterval)
    {
        StepInterval = stepInterval;
    }
    
    // Applies step headbob and rotation headbob (quake style)
    public void HeadBobProcess(Vector3 horizontalVelocity, Vector2 inputAxis, bool isOnFloor, double delta)
    {
        TimedBobCurve?.BobProcess(delta);

        var newPosition = OriginalPosition;
        var newRotation = OriginalRotation;
        if (StepBobEnabled)
        {
            var headPos = DoHeadBob(horizontalVelocity.Length(), delta);
            if (isOnFloor)
            {
                newPosition += headPos;
            }
        }

        // ?????????????????
        // if (TimedBobCurve != null)
        // {
        //     TimedBobCurve.Y -= TimedBobCurve.GetOffset();
        // }

        if (RotationToMove)
        {
            newRotation = HeadBobRotation(inputAxis.Y, inputAxis.X, delta);
        }

        Head.Position = newPosition;
        Head.Quaternion = newRotation;
    }

    private Vector3 DoHeadBob(float speed, double delta)
    {
        var xPos = BobCurve.Sample(CyclePositionX) * CurveMultiplier.X * BobRange.X;
        var yPos = BobCurve.Sample(CyclePositionY) * CurveMultiplier.Y * BobRange.Y;

        var tickSpeed = (speed * (float)delta) / StepInterval;
        CyclePositionX += tickSpeed;
        CyclePositionY += tickSpeed * VerticalHorizontalRatio;

        if (CyclePositionX > 1)
        {
            CyclePositionX -= 1;
        }

        if (CyclePositionY > 1)
        {
            CyclePositionY -= 1;
        }

        return new Vector3(xPos, yPos, 0);
    }

    private Quaternion HeadBobRotation(float x, float z, double delta)
    {
        var targetRotation =
            Quaternion.FromEuler(new Vector3(x * AngleLimitForRotation, 0, -z * AngleLimitForRotation));
        return Head.Quaternion.Slerp(targetRotation, SpeedRotation * (float)delta);
    }
    
    // Resets head bob step cycles
    public void ResetCycles()
    {
        CyclePositionX = 0;
        CyclePositionY = 0;
    }

    public void DoBobJump()
    {
        TimedBobCurve?.DoBobCycle();
    }
}
