using Godot;
using System;

public partial class MenuRevolver : MeshInstance3D
{
	private int rollerstate = 0;
	private int oldrollerstate = 0;
	private float currentAngle = 0f;
	private float oldAngle = 0f;
	private float timer = 0;
	private bool canstart = false;

	CustomSignals signals;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		signals.ControlsUnlocked += ControlsUnlocked;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += (float)delta * 2f;;
		timer = Math.Min(timer,1f);
		if(rollerstate != oldrollerstate)
		{
			timer = 0;
			oldAngle = 0.25f * MathF.PI * oldrollerstate;
			currentAngle = 0.25f * MathF.PI * rollerstate;
			oldrollerstate = rollerstate;
		}
		Rotation = new Vector3(0,0,Global.OvershootSmoothStep(oldAngle,currentAngle,timer));
	}
	
	private void ControlsUnlocked()
	{
		canstart = true;
		GD.Print("controls unlocked");
	}

	public override void _Input(InputEvent @event)
	{
		// GD.Print("ROLLER ",@event.AsText());
		if (timer < 1)
			return;
		if (@event.IsAction("Down"))
		{
			rollerstate += 1;
			rollerstate = ((rollerstate % 8) + 8) % 8;
			FixRotation();
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"plop");
		}
		if (@event.IsAction("Up"))
		{
			rollerstate -= 1;
			rollerstate = ((rollerstate % 8) + 8) % 8;
			FixRotation();
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"plop");
		}
		if (@event.IsActionReleased("Return") && canstart)
		{
			Global.Fadestate = Fadestate.FadeOut;
			Global.TargetState = GameStateFromRollerState(rollerstate);
			GD.Print("ROLLER ",Global.TargetState.ToString());
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"start");
		}
	//OvershootSmoothStep	
	}

	private Gamestate GameStateFromRollerState(int roller)
	{
		roller %= 4;
		return roller switch
		{
			0 => Gamestate.Shuffle,
			1 => Gamestate.Setting,
			2 => Gamestate.Credits,
			3 => Gamestate.Quit,
			_ => Global.State,
		};
	}

	private void FixRotation()
	{
		if (rollerstate == 0 && oldrollerstate == 7)
		{
			oldrollerstate = 3;
			rollerstate = 4;
		}

		if (rollerstate == 7 && oldrollerstate == 0)
		{
			oldrollerstate = 4;
			rollerstate = 3;
		}
	}

}
