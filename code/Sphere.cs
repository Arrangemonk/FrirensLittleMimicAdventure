using Godot;
using System;

public partial class Sphere : MeshInstance3D
{
	private CustomSignals signals;
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		signals.StateChanged += OnStateChanged;
	}
	private void OnStateChanged()
	{
		Visible = (Global.State == Gamestate.Credits);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateY(0.01f);
	}
}
