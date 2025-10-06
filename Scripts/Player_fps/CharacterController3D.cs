using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class CharacterController3D : CharacterBody3D
{
    // Main class of the movement system, contains abilities array for character movements.

    /// <summary>
    /// <para> Emitted when the character performs a step </para>
    /// <para> Called at the end of the <see cref="CharacterController3D.Move"/> function when a move accumulator for a step has ended. </para>
    /// </summary>
    [Signal] 
    public delegate void SteppedEventHandler();

    /// <summary>
    /// <para> Emitted when touching the ground after being airborne </para>
    /// <para> Is called in the <see cref="CharacterController3D.Move"/> function </para>
    /// </summary>
    [Signal]
    public delegate void LandedEventHandler();

    /// <summary>
    /// <para> Emitted when a jump is processed </para>
    /// <para> Is called when <see cref="JumpAbility3D"/> is active</para>
    /// </summary>
    [Signal]
    public delegate void JumpedEventHandler();

    /// <summary>
    /// <para>Emitted when a crouch is started </para>
    /// <para>Is called when <see cref="CrouchAbility3D"/> is active</para>
    /// </summary>
    [Signal]
    public delegate void CrouchedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a crouch is finished </para>
    /// <para> Is called when <see cref="CrouchAbility3D"/> is deactivated</para>
    /// </summary>
    [Signal]
    public delegate void UncrouchedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a sprint started </para>
    /// <para> Is called when <see cref="SprintAbility3D"/> is active</para>
    /// </summary>
    [Signal]
    public delegate void SprintedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a fly mode is started </para>
    /// <para> Is called when <see cref="FlyModeAbility3D"/> is active</para>
    /// </summary>
    [Signal]
    public delegate void FlyModeActivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a fly mode is finished, called when <see cref="FlyModeAbility3D"/> is deactivated</para>
    /// </summary>
    [Signal]
    public delegate void FlyModeDeactivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when emerged in water. Called when the height of the water depth returned from the
    /// <see cref="SwimAbility3D.GetDepthOnWater"/> function of <see cref="SwimAbility3D"/> is greater than the minimum height defined in
    /// <see cref="SwimAbility3D.SubmergedHeigh"/>.</para>
    /// </summary>
    [Signal]
    public delegate void EmergedEventHandler();
    
    /// <summary>
    /// <para>Emitted when emerged in water. Called when the height of the water depth returned from the
    /// <see cref="SwimAbility3D.GetDepthOnWater"/> function of <see cref="SwimAbility3D"/> is less than the minimum height defined in
    /// <see cref="SwimAbility3D.SubmergedHeigh"/>.</para>
    /// </summary>
    [Signal]
    public delegate void SubmergedEventHandler();
    
    /// <summary>
    /// <para>Emitted when it starts to touch the water</para>
    /// </summary>
    [Signal]
    public delegate void EnteredTheWaterEventHandler();
    
    /// <summary>
    /// <para>Emitted when it stops touching the water</para>
    /// </summary>
    [Signal]
    public delegate void ExitTheWaterEventHandler();
    
    /// <summary>
    /// <para>Emitted when water starts to float.</para>
    /// <para>Called when the height of the water depth returned from the <see cref="SwimAbility3D.GetDepthOnWater"/> function
    /// on <see cref="SwimAbility3D"/> is greater than minimum height defined in <see cref="SwimAbility3D.FloatingHeight"/></para>
    /// </summary>
    [Signal]
    public delegate void StartedFloatingEventHandler();
    
    /// <summary>
    /// <para>Emitted when water starts to float.</para>
    /// <para>Called when the height of the water depth returned from the <see cref="SwimAbility3D.GetDepthOnWater"/> function
    /// on <see cref="SwimAbility3D"/> is less than minimum height defined in <see cref="SwimAbility3D.FloatingHeight"/></para>
    /// </summary>
    [Signal]
    public delegate void StoppedFloatingEventHandler();

    [ExportGroup("Movement")] 
    [Export] public float GravityMultiplier { get; set; } = 3.0f;

    [Export] public float Speed { get; set; } = 10.0f;
    
    /// <summary>
    /// Time for the character to reach full speed
    /// </summary>
    [Export] public float Acceleration { get; set; } = 8.0f;
    
    /// <summary>
    /// Time for the character to stop walking
    /// </summary>
    [Export] public float Deceleration { get; set; } = 10.0f;

    [Export] public float AirControl { get; set; } = 0.3f;

    [ExportGroup("Sprint")] 
    [Export] public float SprintSpeedMultiplier { get; set; } = 1.6f;

    [ExportGroup("Footsteps")]
    
    [Export] public float StepLengthen { get; set; } = 0.7f;
    
    /// <summary>
    /// Value to be added to compute a step, each frame that the character is walking this value is added to a counter
    /// </summary>
    [Export] public float StepInterval { get; set; } = 6.0f;

    [ExportGroup("Crouch")] 
    [Export] public float HeightInCrouch { get; set; } = 1.0f;

    [Export] public float CrouchSpeedMultiplier { get; set; } = 0.7f;

    [ExportGroup("Jump")] 
    [Export] public float JumpHeight { get; set; } = 10.0f;

    [ExportGroup("Fly")] 
    [Export] public float FlyModeSpeedMultiplier { get; set; } = 2.0f;

    [ExportGroup("Swim")] 
    [Export] public float SubmergedHeight { get; set; } = 0.36f;
    [Export] public float FloatingHeight { get; set; } = 0.75f;
    [Export] public float OnWaterSpeedMultiplier { get; set; } = 0.75f;
    [Export] public float SubmergedSpeedMultiplier { get; set; } = 0.5f;
    
    [ExportGroup("Abilities")]
    
    // List of movement skills to be used in processing this class.
    [Export] public Array<MovementAbility3D> Abilities { get; set; }

    /// <summary>
    /// Result direction of inputs sent to <see cref="CharacterController3D.Move"/>
    /// </summary>
    private Vector3 _direction = Vector3.Zero;

    /// Current counter used to calculate next step.
    private float _stepCycle;
    /// Maximum value for _step_cycle to compute a step.
    private float _nextStep;
    
    /// Character controller horizontal speed.
    public Vector3 HorizontalVelocity = Vector3.Zero;
    
    /// <summary>
    /// <para>Base transform node to direct player movement</para>
    /// <para>Used to differentiate fly mode/swim moves from regular character movement.</para>
    /// </summary>
    public Node3D DirectionBaseNode;

    // References
    
    public Vector3 Gravity;
    public CollisionShape3D Collision;
    
    /// Above head collision checker, used for crouching and jumping
    private ShapeCast3D _headCheck;
    
    // Abilities

    private WalkAbility3D _walkAbility;
    private CrouchAbility3D _crouchAbility;
    private JumpAbility3D _jumpAbility;
    
    // Store

    private float _normalSpeed;
    private bool _lastIsOnFloor;
    private float _defaultHeight;

    public override void _Ready()
    {
        Collision = GetNode<CollisionShape3D>("Collision");
        _headCheck = GetNode<ShapeCast3D>("HeadCheck");

        _walkAbility = GetNode<WalkAbility3D>("WalkAbility");
        _crouchAbility = GetNode<CrouchAbility3D>("CrouchAbility");
        _jumpAbility = GetNode<JumpAbility3D>("JumpAbility");

        _normalSpeed = Speed;
    }

    public virtual void Setup()
    {
        DirectionBaseNode = this;
        var shape = (CapsuleShape3D)Collision.Shape;
        _defaultHeight = shape.Height;
        
        ConnectSignals();
        StartVariables();
    }

    public void Move(double delta, Vector2 inputAxis = new Vector2(), bool inputJump = false, bool inputCrouch = false,
        bool inputSwimDown = false, bool inputSwimUp = false)
    {
        var direction = DirectionInput(inputAxis, inputSwimDown, inputSwimUp, DirectionBaseNode);
        CheckLanded();

        if (!_jumpAbility.IsActivated())
        {
            Velocity += GetGravity() * GravityMultiplier * (float)delta;
        }
        
        // Abilities activation conditions
        _jumpAbility.SetActive(inputJump && IsOnFloor() && !_headCheck.IsColliding());
        _walkAbility.SetActive(true);
        _crouchAbility.SetActive(inputCrouch && IsOnFloor());

        var multiplier = 1.0f;
        foreach (var ability in Abilities)
        {
            multiplier *= ability.GetSpeedModifier();
        }

        Speed = _normalSpeed * multiplier;

        foreach (var ability in Abilities)
        {
            Velocity = ability.Apply(Velocity, Speed, IsOnFloor(), direction, delta);
        }

        MoveAndSlide();
        HorizontalVelocity = new Vector3(Velocity.X, 0, Velocity.Z);

        CheckStep(delta);
    }

    private void ConnectSignals()
    {
        _crouchAbility.Activated += OnCrouched;
        _crouchAbility.Deactivated += OnUncrouched;
        _jumpAbility.Activated += OnJumped;
    }

    private void OnCrouched()
    {
        EmitSignal(SignalName.Crouched);
    }
    
    private void OnUncrouched()
    {
        EmitSignal(SignalName.Uncrouched);
    }
    
    public virtual void OnJumped()
    {
        EmitSignal(SignalName.Jumped);
    }

    private void StartVariables()
    {
        // TODO: rewrite base variables
        _walkAbility.Acceleration = Acceleration;
        _walkAbility.Deceleration = Deceleration;
        _walkAbility.AirControl = AirControl;
        _crouchAbility.SpeedMultiplier = CrouchSpeedMultiplier;
        _crouchAbility.DefaultHeight = _defaultHeight;
        _crouchAbility.HeightInCrouch = HeightInCrouch;
        _crouchAbility.Collision = Collision;
        _crouchAbility.HeadCheck = _headCheck;
        _jumpAbility.Height = JumpHeight;
    }

    private Vector3 DirectionInput(Vector2 input, bool inputDown, bool inputUp, Node3D aimNode)
    {
        _direction = Vector3.Zero;
        
        var aim = aimNode.GetGlobalTransform().Basis;
        if (input.Y >= 0.5)
        {
            _direction -= aim.Z;
        }

        if (input.Y <= -0.5)
        {
            _direction += aim.Z;
        }

        if (input.X >= 0.5)
        {
            _direction += aim.X;
        }

        if (input.X <= -0.5)
        {
            _direction -= aim.X;
        }

        // NOTE: For free-flying and swimming movements
        
        _direction.Y = 0;
        
        return _direction.Normalized();
    }

    private void CheckLanded()
    {
        if (IsOnFloor() && !_lastIsOnFloor)
        {
            OnLanded();
            ResetStep();
        }

        _lastIsOnFloor = IsOnFloor();
    }

    private void OnLanded()
    {
        EmitSignal(SignalName.Landed);
    }

    private void ResetStep()
    {
        _nextStep = _stepCycle + StepInterval;
    }

    private void CheckStep(double delta)
    {
        if (IsStep(HorizontalVelocity.Length(), delta))
        {
            Step(IsOnFloor());
        }
    }

    private bool IsStep(float velocity, double delta)
    {
        if (Mathf.Abs(velocity) < 0.1f)
        {
            return false;
        }

        _stepCycle += (velocity + StepLengthen) * (float)delta;

        if (_stepCycle <= _nextStep)
        {
            return false;
        }

        return true;
    }

    private bool Step(bool isOnFloor)
    {
        ResetStep();
        if (isOnFloor)
        {
            EmitSignal(SignalName.Stepped);
            return true;
        }

        return false;
    }
    
    






}
