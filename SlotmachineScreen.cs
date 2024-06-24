using Godot;
using System;

public partial class SlotmachineScreen : Sprite2D
{
// Called when the node enters the scene tree for the first time.
	SubViewport svp;
	Node3D slotmachine;
	
	private CustomSignals signals;

	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		svp = GetNode<SubViewport>("SlotmachineViewport");	 
		slotmachine = svp.GetNode<slotmachine>("Slotmachine");
		Texture = svp.GetTexture();
		GetViewport().SizeChanged += onViewportResize;
		signals.StateChanged += onViewportResize;
		onViewportResize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	float timer = -2f;
	public override void _Process(double delta)
	{
		timer += (float)delta;
		//Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(MathF.PI*0.5f,Math.Max(0,timer))));
	}

    public override void _Input(InputEvent @event)
    {
		if(Global.State != Gamestate.Title)
			return;
		
		slotmachine._Input(@event);   
	}

	private void onViewportResize()
	{
		if(Global.State != Gamestate.Title){
			Scale = Vector2.Zero;
			return;
		}
		var screensize = GetViewport().GetVisibleRect().Size;
		var scale = MathF.Min(screensize.X,screensize.Y) / Texture.GetSize().Y * 1.2f;
		Position = new Vector2(screensize.X * 0.5f,screensize.Y *0.5f);
		Scale = new Vector2(scale,scale);
	}
}
