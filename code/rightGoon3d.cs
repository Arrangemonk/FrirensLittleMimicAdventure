using Godot;
using System;

public partial class RightGoon3d : Sprite3D
{
private CustomSignals signals;


private Texture2D[] textures = new[]{
		GD.Load<Texture2D>("res://images//fern.png"),
		GD.Load<Texture2D>("res://images//stark.png"),
		GD.Load<Texture2D>("res://images//friren.png")
		};

	public override void _Ready()
	{
		signals = Global.Signals(this);
		signals.PlayerChanged += PlayerChanged;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = Global.State == Gamestate.Revolver
		|| Global.State == Gamestate.Result;
	}

	private void PlayerChanged()
	{
		Texture = textures[(int)Global.Player];
	}
}
