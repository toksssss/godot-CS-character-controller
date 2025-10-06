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
    /// <para> Called at the end of the <b>Move()</b> function when a move accumulator for a step has ended. </para>
    /// </summary>
    [Signal] 
    public delegate void SteppedEventHandler();

    /// <summary>
    /// <para> Emitted when touching the ground after being airborne </para>
    /// <para> Is called in the <b>Move()</b> function </para>
    /// </summary>
    [Signal]
    public delegate void LandedEventHandler();

    /// <summary>
    /// <para> Emitted when a jump is processed </para>
    /// <para> Is called when <b>JumpAbility3D</b> is active</para>
    /// </summary>
    [Signal]
    public delegate void JumpedEventHandler();

    /// <summary>
    /// <para>Emitted when a crouch is started </para>
    /// <para>Is called when <b>CrouchAbility3D</b> is active</para>
    /// </summary>
    [Signal]
    public delegate void CrouchedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a crouch is finished </para>
    /// <para> Is called when <b>CrouchAbility3D</b> is deactive</para>
    /// </summary>
    [Signal]
    public delegate void UncrouchedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a sprint started </para>
    /// <para> Is called when <b>SprintAbility3D</b> is active</para>
    /// </summary>
    [Signal]
    public delegate void SprintedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a fly mode is started </para>
    /// <para> Is called when <b>FlyModeAbility3D</b> is active</para>
    /// </summary>
    [Signal]
    public delegate void FlyModeActivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when a fly mode is finished, called when <b>FlyModeAbility3D</b> is deactive</para>
    /// </summary>
    [Signal]
    public delegate void FlyModeDeactivatedEventHandler();
    
    /// <summary>
    /// <para>Emitted when emerged in water. Called when the height of the water depth returned from the
    /// <b>GetDepthOnWater()</b> function of <b>SwimAbility3D</b> is greater than the minimum height defined in
    /// <b>SubmergedHeight</b>.</para>
    /// </summary>
    [Signal]
    public delegate void EmergedEventHandler();
    
    /// <summary>
    /// <para>Emitted when emerged in water. Called when the height of the water depth returned from the
    /// <b>GetDepthOnWater()</b> function of <b>SwimAbility3D</b> is less than the minimum height defined in
    /// <b>SubmergedHeight</b>.</para>
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
    /// <para>Called when the height of the water depth returned from the <b>GetDepthOnWater()</b> function on <b>SwimAbility3D</b> is greater than minimum height defined in <b>FloatingHeight</b></para>
    /// </summary>
    [Signal]
    public delegate void StartedFloatingEventHandler();
    
    /// <summary>
    /// <para>Emitted when water starts to float.</para>
    /// <para>Called when the height of the water depth returned from the <b>GetDepthOnWater()</b> function on <b>SwimAbility3D</b> is less than minimum height defined in <b>FloatingHeight</b></para>
    /// </summary>
    [Signal]
    public delegate void StoppedFloatingEventHandler();

    [ExportGroup("Movement")] 
    [Export] public float GravityMultiplier { get; set; } = 3.0f;

    [Export] public float Speed { get; set; } = 10.0f;
    
    // Time for the character to reach full speed 
    [Export] public float Acceleration { get; set; } = 8.0f;
    
    // Time for the character to stop walking
    [Export] public float Deceleration { get; set; } = 10.0f;

    [Export] public float AirControl { get; set; } = 0.3f;

    [ExportGroup("Sprint")] 
    [Export] public float SprintSpeedMultiplier { get; set; } = 1.6f;

    [ExportGroup("Footsteps")]
    // Maximum counter value to be computed one step
    [Export] public float StepLengthen { get; set; } = 0.7f;
    
    // Value to be added to compute a step, each frame that the character is walking this value
    // is added to a counter
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

    // List of movement skills to be used in processing this class.
    private Array<MovementAbility3D> _abilities;

    // Result direction of inputs sent to Move()
    private Vector3 _direction = Vector3.Zero;

    // Current counter used to calculate next step.
    private float _stepCycle;
    // Maximum value for _step_cycle to compute a step.
    private float _nextStep;
    
    // Character controller horizontal speed.
    public Vector3 HorizontalVelocity = Vector3.Zero;
    
    // Base transform node to direct player movement
    // Used to diferentiate flu mode/swim moves from regular character movement.
    public Node3D DirectionBaseNode;

    // References
    
    public Vector3 Gravity;
    public CollisionShape3D Collision;
    // Above head collision checker, used for crouching and jumping
    public ShapeCast3D HeadCheck;
    
    // Abilities

    private WalkAbility3D WalkAbility;
    private CrouchAbility3D CrouchAbility;
    private JumpAbility3D JumpAbility;
    
    // Store

    private float _normalSpeed;
    private bool _lastIsOnFloor;
    private float _defaultHeight;

    public override void _Ready()
    {
        Gravity = GetGravity() * GravityMultiplier;
        Collision = GetNode<CollisionShape3D>("Collision");
        HeadCheck = GetNode<ShapeCast3D>("HeadCheck");

        WalkAbility = GetNode<WalkAbility3D>("WalkAbility");
        CrouchAbility = GetNode<CrouchAbility3D>("CrouchAbility");
        JumpAbility = GetNode<JumpAbility3D>("JumpAbility");

        _normalSpeed = Speed;
    }

    public virtual void Setup()
    {
        DirectionBaseNode = this;
        _abilities = Abilities;
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

        if (!JumpAbility.IsActivated())
        {
            Velocity += GetGravity() * GravityMultiplier * (float)delta;
        }
        
        // Abilities activation conditions
        JumpAbility.SetActive(inputJump && IsOnFloor() && !HeadCheck.IsColliding());
        WalkAbility.SetActive(true);
        CrouchAbility.SetActive(inputCrouch && IsOnFloor());

        var multiplier = 1.0f;
        foreach (var ability in _abilities)
        {
            multiplier *= ability.GetSpeedModifier();
        }

        Speed = _normalSpeed * multiplier;

        foreach (var ability in _abilities)
        {
            Velocity = ability.Apply(Velocity, Speed, IsOnFloor(), direction, delta);
        }

        MoveAndSlide();
        HorizontalVelocity = new Vector3(Velocity.X, 0, Velocity.Z);

        CheckStep(delta);

    }

    private List<MovementAbility3D> LoadNodes(Array<NodePath> nodePaths)
    {
        List<MovementAbility3D> nodes = new();
        
        foreach (var nodePath in nodePaths)
        {
            var node = GetNode(nodePath);
            if (node != null)
            {
                var ability = node as MovementAbility3D;
                nodes.Add(ability);
            }
        }

        return nodes;
    }

    private void ConnectSignals()
    {
        CrouchAbility.Activated += OnCrouched;
        CrouchAbility.Deactivated += OnUncrouched;
        JumpAbility.Activated += OnJumped;
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
        WalkAbility.Acceleration = Acceleration;
        WalkAbility.Deceleration = Deceleration;
        WalkAbility.AirControl = AirControl;
        CrouchAbility.SpeedMultiplier = CrouchSpeedMultiplier;
        CrouchAbility.DefaultHeight = _defaultHeight;
        CrouchAbility.HeightInCrouch = HeightInCrouch;
        CrouchAbility.Collision = Collision;
        CrouchAbility.HeadCheck = HeadCheck;
        JumpAbility.Height = JumpHeight;
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
        // if (isFlyMode() || IsFloating())
        // {
        //     if (inputUp)
        //     {
        //         _direction.Y += 1.0f;
        //     }
        //     else if (inputDown)
        //     {
        //         _direction.Y -= 1.0f;
        //     }
        // }
        // else
        // {
        //     _direction.Y = 0;
        // }
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
