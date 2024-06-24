using Godot;
using System;
using System.Drawing;

public partial class AriproPresentsMaze : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	float timer = 0;
	public override void _Ready()
	{
		GetViewport().SizeChanged += onViewportResize;
		onViewportResize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		if(Global.State != Gamestate.Title){
			Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,0f);
		return;
		}
		timer += (float)delta;
		Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(MathF.PI,timer)));
	}

	private void onViewportResize()
	{
		var screensize = GetViewport().GetVisibleRect().Size;
		var scale = screensize.X / Texture.GetSize().X *0.5f;
		Position = new Vector2(screensize.X/2f,screensize.Y/2f);
		Scale = new Vector2(scale,scale);
	}
}
