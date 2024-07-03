using Godot;
using System;
using System.Linq;

public partial class SlotmachineAnimation : AnimationPlayer
{
	// Called when the node enters the scene tree for the first time.

	MeshInstance3D cylinder1;
	MeshInstance3D cylinder2;
	MeshInstance3D cylinder3;

	Node3D Slotmachine;
	Vector3 startposition = new Vector3(0f,-5f,0f);
	Vector3 restingposition = Vector3.Zero;
	float timer = 0f;
	double timeroffset = 0f;
	float curr1;
	float curr2;
	float curr3;

	public float? Next1 {get;set;}
	public float? Next2 {get;set;}
	public float? Next3 {get;set;}
	Random rnd = new Random();
	bool animating = false;
	private CustomSignals signals;


	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		cylinder1 = GetNode<MeshInstance3D>("../Cylinder_001");
		cylinder2 = GetNode<MeshInstance3D>("../Cylinder_002");
		cylinder3 = GetNode<MeshInstance3D>("../Cylinder_003");
		Slotmachine = GetNode<Slot>("../../slotmachione");
		var mat1 = cylinder1.MaterialOverride as ShaderMaterial;
		mat1.SetShaderParameter("offset",curr1 = rnd.Next(0,7)*1.0f);
		var mat2 = cylinder2.MaterialOverride as ShaderMaterial;
		mat2.SetShaderParameter("offset",curr2 = rnd.Next(0,7)*1.0f);
		var mat3 = cylinder3.MaterialOverride as ShaderMaterial;
		mat3.SetShaderParameter("offset",curr3 = rnd.Next(0,7)*1.0f);

		signals.SlotmachineActivated += Reset;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeroffset += delta;
		if(!animating)
			return;
		if(timer > 5f){
			animating = false;
			return;
		}
		timer += (float)delta;
		var x = MathF.Max(0f,MathF.Min(1,MathF.Min(6.5f-1.3f*timer,timer*1.3f)));
		Slotmachine.Position = Global.OvershootSmoothStep(startposition,restingposition,x);
	}

	public void Reset()
	{
		GD.Print("slotmachine!!");
		var mat1 = cylinder1.MaterialOverride as ShaderMaterial;
		mat1.SetShaderParameter("resettime",timeroffset);
		mat1.SetShaderParameter("initialoffset",curr1);
		mat1.SetShaderParameter("offset",curr1 = Next1 ?? rnd.Next(0,8)*1.0f);

		var mat2 = cylinder2.MaterialOverride as ShaderMaterial;
		mat2.SetShaderParameter("resettime",timeroffset);
		mat2.SetShaderParameter("initialoffset",curr2);
		mat2.SetShaderParameter("offset",curr2 = Next2 ?? rnd.Next(0,8)*1.0f);

		var mat3 = cylinder3.MaterialOverride as ShaderMaterial;
		mat3.SetShaderParameter("resettime",timeroffset);
		mat3.SetShaderParameter("initialoffset",curr3);
		mat3.SetShaderParameter("offset",curr3 = Next3 ??  rnd.Next(0,8)*1.0f);
	
		timer = 0;
		animating = true;
		Play("Scene");
		Seek(0);
	}
}
