using Godot;
using System;

public partial class chest_grimoir : Node3D
{
	
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
