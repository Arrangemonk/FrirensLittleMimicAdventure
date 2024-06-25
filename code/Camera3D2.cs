using Godot;
using System;

public partial class Camera3D2 : Godot.Camera3D
{
	float distance = 0.2f;
	float speed = 0.2f;
	float oldx = 0;
	float oldy = 0;
	float oldz = 0;
	float currx = 0;
	float curry = 0;
	float currz = 0;
	float timer = 0;

	Vector3 initialposition;
	Vector3 initiallookat;
	
	RandomNumberGenerator rng = new RandomNumberGenerator();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		oldx = rng.RandfRange(-distance, distance);
		oldy = rng.RandfRange(-distance, distance);
		oldz = rng.RandfRange(-distance, distance);
		currx = rng.RandfRange(-distance, distance);
		curry = rng.RandfRange(-distance, distance);
		currz = rng.RandfRange(-distance, distance);

		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f,5.0f,0.0f);
		initialposition = Position;
		initiallookat = new Vector3(0f,0f,0f);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	timer  += (float) delta * speed;
	if(timer > 1){
		timer = 0;
		oldx = currx;
		oldy = curry;
		oldz = currz;
		currx = rng.RandfRange(-distance, distance);
		curry = rng.RandfRange(-distance, distance);
		currz = rng.RandfRange(-distance, distance);
	}
	var tmpx = Global.SmoothStep(oldx,currx,timer);
	var tmpy = Global.SmoothStep(oldy,curry,timer);
	var tmpz = Global.SmoothStep(oldz,currz,timer);

	Position = initialposition + new Vector3(tmpx,tmpy,tmpz);
	LookAt(initiallookat);

	}
	
}
