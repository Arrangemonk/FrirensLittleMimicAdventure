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
        signals.StateChanged += StateChanged;
        PlayerChanged();
        StateChanged();
    }

    private void PlayerChanged()
	{
		Texture = textures[(int)Global.Player];
	}

    private void StateChanged()
    {
        Visible = Global.State == Gamestate.Revolver
                  || Global.State == Gamestate.Result;
    }
}
