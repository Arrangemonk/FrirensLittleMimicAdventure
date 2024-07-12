using Godot;
using System;

public partial class BgMusic : AudioStreamPlayer2D
{
    private float timer = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += (float)delta;
		if(!Playing && 2.5f < timer)
			Play();
	}
}
