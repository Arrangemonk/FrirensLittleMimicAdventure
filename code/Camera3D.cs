using Godot;
using System;

public partial class Camera3D : Godot.Camera3D
{
	float slowdistance = 0.2f;
	float slowspeed = 0.2f;
	float fastdistance = 0.1f;
	float fastspeed = 30f;
	float oldx = 0;
	float oldy = 0;
	float oldz = 0;
	float currx = 0;
	float curry = 0;
	float currz = 0;
	float timer = 0;
	float violenceTimer = 1f;


	private CustomSignals signals;
	
	RandomNumberGenerator rng = new RandomNumberGenerator();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		oldx = TmpDistance(	slowdistance);
		oldy = TmpDistance( slowdistance);
		oldz = TmpDistance( slowdistance);
		currx = TmpDistance( slowdistance);
		curry = TmpDistance( slowdistance);
		currz = TmpDistance( slowdistance);

		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f,5.0f,0.0f);

		signals.ViolentShakeStart += BeginViolentShaking;
	}


    private void BeginViolentShaking()
    {
     	violenceTimer = 0;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	var violentposition = Global.CameraPosition + new Vector3(0,-0.2f,0);
	violenceTimer = Math.Max(0,Math.Min(1,violenceTimer + (float)delta * 1.5f));
	var tmpspeed = Global.SmoothStep(fastspeed,slowspeed,1);
	var tmpdistance = Global.SmoothStep(fastdistance,slowdistance,1);
	timer  += (float) delta * tmpspeed;
	if(timer > 1){
		timer = 0;
		oldx = currx;
		oldy = curry;
		oldz = currz;
		currx = TmpDistance(tmpdistance);
		curry = TmpDistance(tmpdistance);
		currz = TmpDistance(tmpdistance);
	}
	var tmpx = Global.SmoothStep(oldx,currx,timer);
	var tmpy = Global.SmoothStep(oldy,curry,timer);
	var tmpz = Global.SmoothStep(oldz,currz,timer);

	var tmpPos = Global.bounce(violentposition,Global.CameraPosition,violenceTimer);

	Position = tmpPos + new Vector3(tmpx,tmpy,tmpz);
	LookAt(Global.CameraTarget);

	}

	private float TmpDistance(float distance)
	{
		return rng.RandfRange(-distance, distance);
	}


	
}
