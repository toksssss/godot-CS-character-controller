using Godot;
using System;

[GlobalClass]
public partial class Player : FPSController3D
{
    // Example script that extends [CharacterController3D] through
    // [FPSController3D]
    //
    // This is just an example, and should be used as a basis for creating your
    // own version using the controller's Move() function
    //
    // This player contains the inputs that will be used if the function
    // Move() in _PhysicsProcess()
    // The input process only happens when mouse is in capture mode.
    // This script also adds submerged and merged signals to change
    // [Environment] when we are in the water.

    [Export] public string InputBackActionName { get; set; } = "move_backward";
    [Export] public string InputForwardActionName { get; set; } = "move_forward";
    [Export] public string InputLeftActionName { get; set; } = "move_left";
    [Export] public string InputRightActionName { get; set; } = "move_right";
    [Export] public string InputJumpActionName { get; set; } = "move_jump";
    [Export] public string InputCrouchActionName { get; set; } = "move_crouch";
    [Export] public string InputChangeMouseModeActionName { get; set; } = "m_button2";

    public override void _Ready()
    {
        base._Ready();
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
        Setup();
    }

    public override void _PhysicsProcess(double delta)
    {
        var isValidInput = Input.GetMouseMode() == Input.MouseModeEnum.Captured;

        if (isValidInput)
        {
            var inputAxis = Input.GetVector(InputLeftActionName, InputRightActionName, InputBackActionName,
                InputForwardActionName);
            var inputJump = Input.IsActionJustPressed(InputJumpActionName);
            var inputCrouch = Input.IsActionPressed(InputCrouchActionName);
            Move(delta, inputAxis, inputJump, inputCrouch);
        }
        else
        {
            Move(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.GetMouseMode() == Input.MouseModeEnum.Captured)
        {
            RotateHead(mouseMotion.ScreenRelative);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(InputChangeMouseModeActionName))
        {
            var mouseMode = Input.GetMouseMode() == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
            Input.SetMouseMode(mouseMode);
        }
    }
}
