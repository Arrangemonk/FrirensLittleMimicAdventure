using Godot;
using System;

public partial class GodRays : Node3D
{
	private CustomSignals signals;
	private MeshInstance3D rays;
	public override void _Ready()
	{
		signals = Global.Signals(this);
		rays = GetNode<MeshInstance3D>("rays");
		signals.StateChanged += OnStateChanged;
		rays.Visible = true;
	}
	private void OnStateChanged()
	{
		rays.Visible = Global.State == Gamestate.Credits || Global.State == Gamestate.Title;
		rays.Scale = Global.State == Gamestate.Title ? new Vector3(10f,10f,10f) : new Vector3(0.35f,0.35f,0.35f);
		GD.Print(rays.Scale,rays.Visible);
	}

		public override void _Process(double delta)
	{
		RotateY(-0.0025f);
	}

}
