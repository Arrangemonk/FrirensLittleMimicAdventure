using Godot;
using System;

public partial class Gameplay : Node3D
{

	private CustomSignals signals;
	public override void _Ready()
	{
		Engine.MaxFps = 60;
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		signals.StateChanged += OnStateChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	double timer = 0;
	public override void _Process(double delta)
	{
		/*timer+= delta;
		if(3 < timer )
		{
			timer = 0d;
			Global.AmountChests = Math.Max(3,(Global.AmountChests +1) % (Global.MaxChests +1));
			signals.EmitSignal(nameof(CustomSignals.AmountChecksChanged));
		}
		*/
	}

		public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("Escape") && Global.Fadestate == Fadestate.None)
		if (Global.State != Gamestate.Title)
		{
			Global.Fadestate = Fadestate.FadeOut;
			Global.TargetState = Gamestate.Title;
		}
		else if (Global.State == Gamestate.Title){
			Global.Fadestate = Fadestate.FadeOut;
			Global.TargetState = Gamestate.Quit;
		}
	}
	private void OnStateChanged()
	{
		if (Global.State == Gamestate.Quit)
			GetTree().Quit();
		if (Global.State == Gamestate.Title)
			SetupTitle();
	}

	private void SetupTitle(){

		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f,5.0f,0.0f);
	}
}
