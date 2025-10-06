using Godot;
using System;

public partial class PlayerProto : CharacterBody3D
{
    [Export] public bool CanMove { get; set; } = true;
    [Export] public bool HasGravity { get; set; } = true;
    [Export] public bool CanJump { get; set; } = true;
    [Export] public bool CanSprint { get; set; }
    [Export] public bool CanNoclip { get; set; }
    
    [ExportGroup("Speeds")]
    // Look around rotation speed
    [Export] public float LookSpeed { get; set; } = 0.002f;
    // Normal speed
    [Export] public float BaseSpeed { get; set; } = 7.0f;
    // Speed of jump
    [Export] public float JumpVelocity { get; set; } = 4.5f;
    // Running speed
    [Export] public float SprintSpeed { get; set; } = 10.0f;
    // Noclip speed
    [Export] public float NoclipSpeed { get; set; } = 25.0f;
    
    [ExportGroup("Input Actions")]
    [Export] public string InputLeft { get; set; } = "move_left";
    [Export] public string InputRight { get; set; } = "move_right";
    [Export] public string InputForward { get; set; } = "move_forward";
    [Export] public string InputBack { get; set; } = "move_back";
    [Export] public string InputJump { get; set; } = "jump";
    [Export] public string InputSprint { get; set; } = "sprint";
    [Export] public string InputNoclip { get; set; } = "noclip";
    

    public bool MouseCaptured;
    public Vector2 LookRotation;
    public float MoveSpeed;
    public bool Nocliping;
    
    // IMPORTANT REFERENCES
    public Node3D Head;
    public CollisionShape3D Collider;

    public override void _Ready()
    {
        CheckInputMappings();
        
        Head = GetNode<Node3D>("Head");
        Collider = GetNode<CollisionShape3D>("Collider");

        LookRotation.Y = Rotation.Y;
        LookRotation.X = Head.Rotation.X;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Mouse capturing
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            CaptureMouse();
        }

        if (Input.IsKeyPressed(Key.Escape))
        {
            ReleaseMouse();
        }

        // Look around
        if (MouseCaptured && @event is InputEventMouseMotion myEvent)
        {
            RotateLook(myEvent.Relative);
        }
        
        // Toggle noclip
        if (CanNoclip && Input.IsActionJustPressed(InputNoclip))
        {
            if (!Nocliping)
            {
                EnableNoclip();
            }
            else
            {
                DisableNoclip();
            }
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        // If nocliping, handle noclip and nothing else
        if (CanNoclip && Nocliping)
        {
            var inputDir = Input.GetVector(
                InputLeft,
                InputRight,
                InputForward,
                InputBack
            );
            var motion = (Head.GlobalBasis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            motion.X *= NoclipSpeed * (float)delta;
            motion.Y *= NoclipSpeed * (float)delta;
            motion.Z *= NoclipSpeed * (float)delta;

            MoveAndCollide(motion);
            return;
        }
        
        // Apply gravity to velocity
        if (HasGravity)
        {
            if (!IsOnFloor())
            {
                Velocity += GetGravity() * (float)delta;
            }
        }
        
        // Apply jumping
        if (CanJump)
        {
            if (Input.IsActionJustPressed(InputJump) && IsOnFloor())
            {
                var velocity = Velocity;
                velocity.Y = JumpVelocity;
                Velocity = velocity;
            }
        }
        
        // Modify speed based on spriting
        if (CanSprint && Input.IsActionPressed(InputSprint))
        {
            MoveSpeed = SprintSpeed;
        }
        else
        {
            MoveSpeed = BaseSpeed;
        }
        
        // Apply desired movement to velocity
        if (CanMove)
        {
            var inputDir = Input.GetVector(
                InputLeft, 
                InputRight, 
                InputForward, 
                InputBack
                );
            var moveDir = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (moveDir != Vector3.Zero)
            {
                var velocity = Velocity;
                velocity.X = moveDir.X * MoveSpeed;
                velocity.Z = moveDir.Z * MoveSpeed;
                Velocity = velocity;
            }
            else
            {
                var velocity = Velocity;
                velocity.X = Mathf.MoveToward(velocity.X, 0, MoveSpeed);
                velocity.Z = Mathf.MoveToward(velocity.Z, 0, MoveSpeed);
                Velocity = velocity;
            }
        }
        else
        {
            var velocity = Velocity;
            velocity.X = 0;
            velocity.Y = 0;
            Velocity = velocity;
        }
        
        // GD.Print(Velocity);

        MoveAndSlide();
    }
    
    
// ## Rotate us to look around.
// ## Base of controller rotates around y (left/right). Head rotates around x (up/down).
// ## Modifies look_rotation based on rot_input, then resets basis and rotates by look_rotation.
    public void RotateLook(Vector2 rotInput)
    {
        LookRotation.X -= rotInput.Y * LookSpeed;
        LookRotation.X = Mathf.Clamp(LookRotation.X, Mathf.DegToRad(-85), Mathf.DegToRad(85));
        LookRotation.Y -= rotInput.X * LookSpeed;

        var transform = Transform;
        transform.Basis = Basis.Identity;
        Transform = transform;
        
        RotateY(LookRotation.Y);
        var headTransform = Head.Transform;
        headTransform.Basis = Basis.Identity;
        Head.Transform = headTransform;
        Head.RotateX(LookRotation.X);
    }

    public void EnableNoclip()
    {
        Collider.Disabled = true;
        Nocliping = true;
        Velocity = Vector3.Zero;
    }

    public void DisableNoclip()
    {
        Collider.Disabled = false;
        Nocliping = false;
    }

    public void CaptureMouse()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
        MouseCaptured = true;
    }

    public void ReleaseMouse()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        MouseCaptured = false;
    }
    
    // Checks if some Input Actions haven't been created
    // Disables functionality accordingly
    public void CheckInputMappings()
    {
        
        if (CanMove && !InputMap.HasAction(InputLeft))
        {
            GD.PushError("Movement disabled. No InputAction found for InputLeft: " + InputLeft);
            CanMove = false;
        }
        if (CanMove && !InputMap.HasAction(InputRight))
        {
            GD.PushError("Movement disabled. No InputAction found for InputRight: " + InputRight);
            CanMove = false;
        }
        if (CanMove && !InputMap.HasAction(InputForward))
        {
            GD.PushError("Movement disabled. No InputAction found for InputForward: " + InputForward);
            CanMove = false;
        }
        if (CanMove && !InputMap.HasAction(InputBack))
        {
            GD.PushError("Movement disabled. No InputAction found for InputBack: " + InputBack);
            CanMove = false;
        }
        if (CanJump && !InputMap.HasAction(InputJump))
        {
            GD.PushError("Movement disabled. No InputAction found for InputJump: " + InputJump);
            CanJump = false;
        }
        if (CanSprint && !InputMap.HasAction(InputSprint))
        {
            GD.PushError("Movement disabled. No InputAction found for InputSprint: " + InputSprint);
            CanSprint = false;
        }
        if (CanNoclip && !InputMap.HasAction(InputNoclip))
        {
            GD.PushError("Movement disabled. No InputAction found for InputNoclip: " + InputNoclip);
            CanNoclip = false;
        }
    }
    
}
