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

	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if(Global.State != Gamestate.Result)
			return;
		
		slotmachine._Input(@event);   
	}

	private void onViewportResize()
	{
		//if(Global.State != Gamestate.Result){
		//	Scale = Vector2.Zero;
		//	return;
		//}
		var screensize = GetViewport().GetVisibleRect().Size;
		var scale = MathF.Min(screensize.X,screensize.Y) / Texture.GetSize().Y * 1.4f;
		Position = new Vector2(screensize.X * 0.5f,screensize.Y *0.58f);
		Scale = new Vector2(scale,scale);
	}
}
