using Godot;
using System;

public partial class Licence : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	private CustomSignals signals;
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		GetViewport().SizeChanged += onViewportResize;
		signals.StateChanged += onViewportResize;
		onViewportResize();
	}

	private void onViewportResize()
	{
		if(Global.State != Gamestate.Title){
			Scale = Vector2.Zero;
			return;
		}
		var screensize = GetViewport().GetVisibleRect().Size;
		var scale = MathF.Min(screensize.X,screensize.Y)  * 0.2f / MathF.Min(Texture.GetSize().X,Texture.GetSize().Y)*0.25f ;
		Position = new Vector2(screensize.X - scale * Texture.GetSize().X * 0.5f,screensize.Y *0.98f);
		Scale = new Vector2(scale,scale);
	}

	float timer = -2f;
	public override void _Process(double delta)
	{
		timer += (float)delta;
		Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(MathF.PI*0.5f,Math.Max(0,timer))));
	}
}
