using Godot;
using System;

public partial class StateFader : ColorRect
{
	private const float speed = 3f;
	private const float duration = Mathf.Pi /speed;
	private float timer = 0f;
	private CustomSignals signals;
	private Fadestate oldFadestate = Fadestate.None;
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		GetViewport().SizeChanged += onViewportResize;
		onViewportResize();
		Color = new Color(0, 0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Global.Fadestate != oldFadestate)
		{
			oldFadestate =Global.Fadestate;
			timer = 0f;
		}
		if (Global.Fadestate == Fadestate.FadeOut){
		if (timer > duration)
		{
			Modulate = new Color(Modulate.R, Modulate.B, Modulate.B, 0f);
			Global.Fadestate = Fadestate.None;
			GD.Print((int)Global.Player);
		}
		else if (timer > duration * 0.5 )
		{
			if (Global.State != Global.TargetState)
			{
				Global.State = Global.TargetState;
				signals.EmitSignal(nameof(CustomSignals.StateChanged));
				GD.Print(Global.State);
			}
			if (Global.Player != Global.TargetPlayer)
			{
				Global.Player = Global.TargetPlayer;
				signals.EmitSignal(nameof(CustomSignals.PlayerChanged));
				GD.Print(Global.Player);
			}
		}
		Color = new Color(0, 0, 0);
		Modulate = new Color(Modulate.R, Modulate.B, Modulate.B, Mathf.Sin(speed * Mathf.Min(timer, duration)));
		}
		timer += (float)delta;
	}

	public override void _Input(InputEvent @event)
	{
		if (Global.State != Gamestate.Title || Global.Fadestate != Fadestate.None)
			return;
		if (@event.IsAction("Left"))
		{
			Global.Fadestate = Fadestate.FadeOut;
			Global.TargetPlayer = (Player)Global.Mod((int)Global.Player - 1,3);
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"woosh");
		}
		if (@event.IsAction("Right"))
		{
			Global.Fadestate = Fadestate.FadeOut;
			Global.TargetPlayer = (Player)Global.Mod((int)Global.Player + 1,3);
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"woosh");
		}
	}

	private void onViewportResize()
	{
		Size = GetViewport().GetVisibleRect().Size;
		Position = new Vector2(0, 0);
	}
}
