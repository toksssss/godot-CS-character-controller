using Godot;
using System;

public partial class Dev : Label
{
	private Player _player;
	
	public override void _Ready()
	{
		_player = GetNode<Player>("..");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Text = $"PLAYER\n" +
		       $"Velocity: {_player.Velocity}\n" +
		       $"Climbing: {_player.IsClimbing()}";
	}
}
