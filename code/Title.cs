using Godot;
using System;
using System.Drawing;

public partial class Title : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    private float timer = -2f;
	public override void _Process(double delta)
	{
		if(Global.State != Gamestate.Title){
			Scale = Vector2.Zero;
			return;
		}
		var screensize = GetViewport().GetVisibleRect().Size;
		Position = new Vector2(0,screensize.Y *0.8f);
		var scale = MathF.Min(screensize.X,screensize.Y)  * 0.2f / Texture.GetSize().Y ;
		Scale = new Vector2(scale,scale);
		timer += (float)delta;
		Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(MathF.PI*0.5f,Math.Max(0,timer))));
	}
}
