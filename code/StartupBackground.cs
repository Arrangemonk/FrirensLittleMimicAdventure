using Godot;
using System;

public partial class StartupBackground : Godot.ColorRect
{
		float timer = MathF.PI * 0.25f;
	private CustomSignals signals;
	private bool canplayAudio = true;
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		GetViewport().SizeChanged += onViewportResize;
		onViewportResize();
		Color = new Color(0,0,0);
		signals.EmitSignal(nameof(CustomSignals.PlaySound),"flma");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Global.State != Gamestate.Title)
		return;
		timer += (float)delta;
		Modulate  = new Godot.Color(Modulate.R,Modulate.B,Modulate.B,Mathf.Sin(Mathf.Min(Math.Max(MathF.PI/2f,timer),MathF.PI)));
		if(timer >= MathF.PI * 1.05 && canplayAudio){
			canplayAudio = false;
			signals.EmitSignal(nameof(CustomSignals.ControlsUnlocked));
		}
	}

 	private void onViewportResize()
	{
		Size = GetViewport().GetVisibleRect().Size;
		Position = new Vector2(0,0);
	} 
}
