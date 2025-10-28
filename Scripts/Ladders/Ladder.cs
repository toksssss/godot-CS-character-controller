using Godot;
using System;

[GlobalClass]
public partial class Ladder : Area3D
{
	[Signal]
	public delegate void BodyEnteredLadderEventHandler(Node3D body, Ladder ladder);
	
	[Signal]
	public delegate void BodyExitedLadderEventHandler(Node3D body, Ladder ladder);
	
	public override void _Ready()
	{
		BodyEntered += body => OnBodyEntered(body, this);
		BodyExited += body => OnBodyExited(body, this);
	}

	private void OnBodyEntered(Node3D body, Ladder ladder)
	{
		EmitSignal(SignalName.BodyEnteredLadder, body, ladder);
	}
	
	private void OnBodyExited(Node3D body, Ladder ladder)
	{
		EmitSignal(SignalName.BodyExitedLadder, body, ladder);
	}
}
