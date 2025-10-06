using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class InputGatherer : Node
{

	public InputPackage GatherInput()
	{
		var newInput = new InputPackage();

		if (Input.IsActionJustPressed("jump"))
		{
			newInput.Actions.Add("jump");
		}

		// if (Input.IsActionPressed("crouch"))
		// {
		// 	newInput.Actions.Add("crouch");
		// }
		//
		// if (Input.IsActionPressed("free_look"))
		// {
		// 	newInput.Actions.Add("free_look");
		// }
		//
		// if (Input.IsActionPressed("sprint"))
		// {
		// 	newInput.Actions.Add("sprint");
		// }

		newInput.InputDir = Input.GetVector(
			"move_left",
			"move_right",
			"move_forward", 
			"move_back");

		if (newInput.InputDir != Vector2.Zero)
		{
			newInput.Actions.Add("run");
		}

		if (newInput.Actions.Count == 0)
		{
			newInput.Actions.Add("idle");
		}

		return newInput;
	}
}
