using Godot;
using System;

public partial class Goons : Sprite2D
{
    private Texture2D[] textures = new[]{
		GD.Load<Texture2D>("res://images//Goons_0.png"),
		GD.Load<Texture2D>("res://images//Goons_1.png"),
		GD.Load<Texture2D>("res://images//Goons_2.png")
		};

	private CustomSignals signals;
    private float timer = -2f;

	public override void _Ready()
	{
		signals = Global.Signals(this);
		signals.PlayerChanged += OnPlayerChange;
		signals.StateChanged += OnStateChange;
		GetViewport().SizeChanged += OnViewportResize;
		Texture = textures[(int)Global.Player];
		OnViewportResize();
		OnPlayerChange();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += (float)delta;
		Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(MathF.PI*0.5f,Math.Max(0,timer))));
	}

	private void OnViewportResize()
	{
		if(Global.State != Gamestate.Title){
			Scale = Vector2.Zero;
			return;
		}
		var screensize = GetViewport().GetVisibleRect().Size;
		var scale = screensize.X / Texture.GetSize().X;
		Position = new Vector2(screensize.X/2f,screensize.Y/2f);
		Scale = new Vector2(scale,scale);
	}

	private void OnPlayerChange()
	{
		GD.Print(Global.Player);
		Texture = textures[(int)Global.Player];
	}

	private void OnStateChange()
	{
		if(Global.State != Gamestate.Title)
			Scale = new Vector2(0,0);
		else
			OnViewportResize();
	}
}
