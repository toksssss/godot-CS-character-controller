using Godot;
using System;

[GlobalClass]
public partial class Run : Move
{
	// References
	
	//Movement vars
	public float CurrentSpeed = 5.0f;

	public override void _Ready()
	{
	}

	public override string CheckRelevance(InputPackage input)
	{
		SortListString(input.Actions);
		if (input.Actions[0] == "run")
		{
			return "okay";
		}

		return input.Actions[0];

		// if (input.Actions.Contains("jump") && Player.IsOnFloor())
		// {
		// 	return "jump";
		// }
		// if (input.InputDir == Vector2.Zero)
		// {
		// 	return "idle";
		// }
		// return "okay";
	}

	public override void Update(InputPackage input, double delta)
	{
		Player.Velocity = VelocityByInput(input, delta);
		Player.MoveAndSlide();
	}
    
    public Vector3 VelocityByInput(InputPackage inputPackage, double delta)
    {
        var newVelocity = Player.Velocity;

        // Adding gravity
        if (!Player.IsOnFloor())
        {
	        newVelocity += Player.GetGravity() * (float)delta;
        }
        
        var direction = Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y).Normalized();
        direction.X = Mathf.Lerp(direction.X, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().X, (float)delta*LerpSpeed);
        direction.Y = Mathf.Lerp(direction.Y, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().Y, (float)delta*LerpSpeed);
        direction.Z = Mathf.Lerp(direction.Z, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().Z, (float)delta*LerpSpeed);
		  
        if (direction != Vector3.Zero)
        {
            newVelocity.X = direction.X * CurrentSpeed;
            newVelocity.Z = direction.Z * CurrentSpeed;
        }
        else
        {
            newVelocity.X = Mathf.MoveToward(Player.Velocity.X, 0, CurrentSpeed);
            newVelocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, CurrentSpeed);
        }
		  
        return newVelocity;
    }
}
