using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class PlayerSoulsLike : CharacterBody3D
{
	//Important references
	public PlayerModel Model;
	public InputGatherer InputGatherer;
	
	public override void _Ready()
	{
		Model = GetNode<PlayerModel>("Model");
		InputGatherer = GetNode<InputGatherer>("InputGatherer");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var input = InputGatherer.GatherInput();
		Model.Update(input, delta);
	}
}
