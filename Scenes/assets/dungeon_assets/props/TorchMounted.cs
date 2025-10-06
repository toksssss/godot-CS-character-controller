using Godot;
using System;

public partial class TorchMounted : StaticBody3D
{
	[Export] public bool DisableLight { get; set; }
	
	public OmniLight3D Light;
	
	public override void _Ready()
	{
		Light = GetNode<OmniLight3D>("Light");
		
		if (DisableLight)
		{
			Light.LightEnergy = 0;
		}
	}

}
