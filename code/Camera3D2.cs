using Godot;
using System;

public partial class Camera3D2 : Godot.Camera3D
{
    private float distance = 0.2f;
    private float speed = 0.2f;
    private float oldx = 0;
    private float oldy = 0;
    private float oldz = 0;
    private float currx = 0;
    private float curry = 0;
    private float currz = 0;
    private float timer = 0;

    private Vector3 initialposition;
    private Vector3 initiallookat;

    private RandomNumberGenerator rng = new RandomNumberGenerator();
	
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
