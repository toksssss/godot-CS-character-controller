// using Godot;
// using System;
//
// [GlobalClass]
// public partial class PlayerModel_old : Node
// {
// 	
// 	[ExportCategory("Speed")]
// 	[Export] public float WalkingSpeed = 5.0f;
// 	[Export] public float SprintingSpeed = 8.0f;
// 	[Export] public float CrouchingSpeed = 3.0f;
// 	[Export] public float JumpVelocity = 4.5f;
// 	
// 	[ExportCategory("Mouse")] 
// 	[Export] public float MouseSensitivity { get; set; } = 0.4f;
// 	
// 	//Important References
//     public Player Player;
//     
//     public Node3D Head;
//     public Node3D Nek;
// 	   
//     public Camera3D Camera;
// 	   
//     public CollisionShape3D StandingCollider;
//     public CollisionShape3D CrouchingCollider;
// 	   
//     public MeshInstance3D StandingMesh;
//     public MeshInstance3D CrouchingMesh;
// 	   
//     public RayCast3D RayCast;
//     
//     // Movement vars
//     public float CurrentSpeed = 5.0f;
//     public float LerpSpeed = 15.0f;
//     public float CrouchDepth = -0.5f;
//     public float FreeLookTiltAmount = 8.0f;
// 	
//     // States
//     public bool Walking;
//     public bool Sprinting;
//     public bool Crouching;
//     public bool FreeLooking;
//     public bool Sliding;
// 	
//     // Slide vars
//     public float SlideTimer = 0.0f;
//     public float SlideTimerMax = 1.0f;
//     public Vector2 SlideVector = Vector2.Zero;
//     public float SlideSpeed = 10.0f;
// 	
//     // Input vars
//     public Vector3 Direction = Vector3.Zero;
//
//     public override void _Ready()
//     {
//         Player = GetNode<Player>("..");
//         
//         Head = GetNode<Node3D>("../Nek/Head");
//         Nek = GetNode<Node3D>("../Nek");
// 		      
//         Camera = GetNode<Camera3D>("../Nek/Head/Camera3D");
// 		      
//         StandingCollider = GetNode<CollisionShape3D>("../StandingCollider");
//         CrouchingCollider = GetNode<CollisionShape3D>("../CrouchingCollider");
//         
//         StandingMesh = GetNode<MeshInstance3D>("../StandingMesh");
//         CrouchingMesh = GetNode<MeshInstance3D>("../CrouchingMesh");
//
//         RayCast = GetNode<RayCast3D>("../RayCast3D");
//         
//         Input.SetMouseMode(Input.MouseModeEnum.Captured); // model or input?
//     }
//     
    // public override void _Input(InputEvent @event)
    // {
	   //  HandleMouseMovement(@event);
    // }
    //
    // public void HandleMouseMovement(InputEvent @event)
    // {
	   //  if (@event is InputEventMouseMotion mouseEvent)
	   //  {
		  //   if (FreeLooking)
		  //   {
			 //    Nek.RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * MouseSensitivity));
				//
			 //    var nekRotation = Nek.Rotation;
			 //    nekRotation.Y = Mathf.Clamp(nekRotation.Y, Mathf.DegToRad(-110), Mathf.DegToRad(110));
			 //    Nek.Rotation = nekRotation;
		  //   }
		  //   else
		  //   {
			 //    Player.RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * MouseSensitivity));
			 //    Head.RotateX(Mathf.DegToRad(-mouseEvent.Relative.Y * MouseSensitivity));
		  //   }
			 //
		  //   var rotation = Head.Rotation;
		  //   rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
		  //   Head.Rotation = rotation;
	   //  }
    // }
    //
    // public Vector3 VelocityByInput(InputPackage inputPackage, double delta)
    // {
    //     var newVelocity = Player.Velocity;
		  //
    //     // Add the gravity.
    // if (!Player.IsOnFloor())
    // {
    //     newVelocity += Player.GetGravity() * (float)delta;
    // }
    //
    //     // Handle Jump.
    // if (inputPackage.IsJumping && Player.IsOnFloor())
    // {
    //     newVelocity.Y = JumpVelocity;
    //     Sliding = false;
    // }
		  //
    //     Direction.X = Mathf.Lerp(Direction.X, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().X, (float)delta*LerpSpeed);
    //     Direction.Y = Mathf.Lerp(Direction.Y, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().Y, (float)delta*LerpSpeed);
    //     Direction.Z = Mathf.Lerp(Direction.Z, (Player.Transform.Basis * new Vector3(inputPackage.InputDir.X, 0, inputPackage.InputDir.Y)).Normalized().Z, (float)delta*LerpSpeed);
    //
    //     if (Sliding)
    //     {
    //         Direction = (Player.Transform.Basis * new Vector3(SlideVector.X, 0, SlideVector.Y)).Normalized();
    //     }
		  //
    //     if (Direction != Vector3.Zero)
    //     {
    //         newVelocity.X = Direction.X * CurrentSpeed;
    //         newVelocity.Z = Direction.Z * CurrentSpeed;
    //
    //         if (Sliding)
    //         {
    //             newVelocity.X = Direction.X * (SlideTimer + 0.3f) * SlideSpeed;
    //             newVelocity.Z = Direction.Z * (SlideTimer + 0.3f) * SlideSpeed;
    //         }
    //     }
    //     else
    //     {
    //         newVelocity.X = Mathf.MoveToward(Player.Velocity.X, 0, CurrentSpeed);
    //         newVelocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, CurrentSpeed);
    //     }
		  //
    //     return newVelocity;
    // }
//     
//     public void ProcessStates(InputPackage inputPackage, double delta)
// 	{
// 		if (inputPackage.IsCrouching || Sliding)
// 		{
// 			// Crouching
// 			CurrentSpeed = CrouchingSpeed;
//
// 			var position = Head.Position;
// 			position.Y = Mathf.Lerp(position.Y, CrouchDepth, (float)delta * LerpSpeed);
// 			Head.Position = position;
//
// 			StandingCollider.Disabled = true;
// 			StandingMesh.Visible = false;
//
// 			CrouchingCollider.Disabled = false;
// 			CrouchingMesh.Visible = true;
//
// 			// Slide begin logic
// 			if (Sprinting && inputPackage.InputDir != Vector2.Zero)
// 			{
// 				Sliding = true;
// 				SlideTimer = SlideTimerMax;
// 				SlideVector = inputPackage.InputDir;
// 				FreeLooking = true;
// 			}
// 			
// 			Walking = false;
// 			Sprinting = false;
// 			Crouching = true;
// 		}
// 		else if (!RayCast.IsColliding())
// 		{
// 			// Standing
// 			var position = Head.Position;
// 			position.Y = Mathf.Lerp(position.Y, 0.0f, (float)delta * LerpSpeed);
// 			Head.Position = position;
// 			
// 			StandingCollider.Disabled = false;
// 			StandingMesh.Visible = true;
//
// 			CrouchingCollider.Disabled = true;
// 			CrouchingMesh.Visible = false;
// 			
// 			if (inputPackage.IsSprinting)
// 			{
// 				// Sprinting
// 				CurrentSpeed = SprintingSpeed;
// 				
// 				Walking = false;
// 				Sprinting = true;
// 				Crouching = false;
// 			}
// 			else
// 			{
// 				// Walking
// 				CurrentSpeed = WalkingSpeed;
// 				
// 				Walking = true;
// 				Sprinting = false;
// 				Crouching = false;
// 			}
// 		}
// 		
// 		// Handle FreeLooking
// 		if (inputPackage.IsFreeLooking || Sliding)
// 		{
// 			FreeLooking = true;
// 			
// 			var camRotation = Camera.Rotation;
// 			
// 			if (Sliding)
// 			{
// 				camRotation.Z = Mathf.Lerp(camRotation.Z, -Mathf.DegToRad(7.0f), (float)delta * LerpSpeed);
// 			}
// 			else
// 			{
// 				camRotation.Z = -Mathf.DegToRad(Nek.Rotation.Y * FreeLookTiltAmount);
// 			}
// 			
// 			Camera.Rotation = camRotation;
// 		}
// 		else
// 		{
// 			FreeLooking = false;
// 			var rotation = Nek.Rotation;
// 			rotation.Y = Mathf.Lerp(rotation.Y, 0.0f, (float)delta * LerpSpeed);
// 			Nek.Rotation = rotation;
// 			
// 			var camRotation = Camera.Rotation;
// 			camRotation.Z = Mathf.Lerp(camRotation.Y, 0.0f, (float)delta * LerpSpeed);
// 			Camera.Rotation = camRotation;
// 		}
// 		
// 		// Handle Sliding
// 		if (Sliding)
// 		{
// 			SlideTimer -= (float)delta;
// 			if (SlideTimer < 0)
// 			{
// 				Sliding = false;
// 				FreeLooking = false;
// 			}
// 			
// 		}
// 	}
// }
