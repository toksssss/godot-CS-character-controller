using Godot;
using System;

[GlobalClass]
public partial class FPSController3D : CharacterController3D
{
    // Character Controller 3D specialized in FPS
    //
    // Contains information:
    // - FOV
    // - HeadBob
    // - Rotation limits
    // - Inputs for camera rotation

    [ExportGroup("FOV")]
    
    // speed at which the FOV changes
    [Export] public float FovChangeSpeed { get; set; } = 4;

    // FOV to be multiplied when active the sprint
    [Export] public float SprintFovMultiplier { get; set; } = 1.1f;
    
    // FOV to be multiplied when active the crouch
    [Export] public float CrouchFovMultiplier { get; set; } = 0.95f;

    [ExportGroup("Mouse")]

    // Mouse sensitivity
    private float _mouseSensitivity = 2.0f;

    [Export]
    public float MouseSensitivity
    {
        get => _mouseSensitivity;
        set
        {
            _mouseSensitivity = value;
            Head.MouseSensitivity = _mouseSensitivity; 
        }
    }

    [Export] public float VerticalAngleLimit { get; set; } = 90.0f;

    [ExportGroup("Head Bob - Steps")]

    // Enables bob for made steps
    [Export] public bool StepBobEnabled { get; set; } = true;

    // Difference of step bob movement between vertical and horizontal angle
    [Export] public int VerticalHorizontalRatio { get; set; } = 2;

    [ExportGroup("Head Bob - Jump")]

    // Enables bob for made jumps
    [Export] public bool JumpBobEnabled { get; set; } = true;

    [ExportGroup("Head Bob - Move rotation (Quake Like)")]

    // Enables camera angle for the direction the character moves
    [Export] public bool RotationToMove { get; set; } = true;
    
    // Speed at which camera angle moves
    [Export] public float SpeedRotation { get; set; } = 4.0f;
    
    // Rotation angle limit per move
    [Export] public float AngleLimitForRotation { get; set; } = 0.1f;
    
    // References

    public HeadMovement3D Head;
    public Marker3D FirstPersonCameraReference;
    public Marker3D ThirdPersonCameraReference;
    public HeadBob HeadBob;

    public override void _Ready()
    {
        base._Ready();
        
        Head = GetNode<HeadMovement3D>("Head");
        FirstPersonCameraReference = GetNode<Marker3D>("Head/FirstPersonCameraReference");
        ThirdPersonCameraReference = GetNode<Marker3D>("Head/ThirdPersonCameraReference");
        HeadBob = GetNode<HeadBob>("Head/HeadBob");
    }

    public override void Setup()
    {
        base.Setup();
        
        // TODO: rewrite this piece of crap
        Head.SetMouseSensitivity(MouseSensitivity);
        Head.SetVerticalAngleLimit(VerticalAngleLimit);
        HeadBob.StepBobEnabled = StepBobEnabled;
        HeadBob.JumpBobEnabled = JumpBobEnabled;
        HeadBob.RotationToMove = RotationToMove;
        HeadBob.SpeedRotation = SpeedRotation;
        HeadBob.AngleLimitForRotation = AngleLimitForRotation;
        HeadBob.VerticalHorizontalRatio = VerticalHorizontalRatio;
        HeadBob.SetupStepBob(StepInterval * 2); // why 2?
    }
    
    // Rotate head based on mouse axis parameter.
    // This function call Head.RotateCamera()
    public void RotateHead(Vector2 mouseAxis)
    {
        Head.RotateCamera(mouseAxis);
    }
    
    // Call to move the character
    // First it is defined what the direction of movement will be, whether it is vertically or not
    // based on whether swim or fly mode is active
    // After, the Move() of the base class [CharacterMovement3D] is called
    // It is then called functions responsible for head bob if necessary.
    public void Move(double delta, Vector2 inputAxis, bool inputJump, bool inputCrouch, bool inputSprint = false, bool inputSwimDown = false, bool inputSwimUp = false)
    {
        DirectionBaseNode = this;
        base.Move(delta, inputAxis, inputJump, inputCrouch);
        CheckHeadBob(delta, inputAxis);
        
    }

    private void CheckHeadBob(double delta, Vector2 inputAxis)
    {
        HeadBob.HeadBobProcess(HorizontalVelocity, inputAxis, IsOnFloor(), delta);
        
    }

    public override void OnJumped()
    {
        base.OnJumped();
        HeadBob.DoBobJump();
        HeadBob.ResetCycles();
    }


}
