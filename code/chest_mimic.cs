using Godot;
using System;


public partial class chest_mimic : Node3D
{
	/*
	double timer = 0.0f;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	timer += 1.0 * delta;
	Position = new Vector3((float)Math.Sin(timer)*3.0f,0f,(float)Math.Cos(timer)*3.0f);
	}
	*/
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector3(0f,0f,3f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
