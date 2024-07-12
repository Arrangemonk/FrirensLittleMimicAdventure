using Godot;
using System;

public partial class Gameplay : Node3D
{

	private CustomSignals signals;
	private TextureProgressBar Shufflebar; 
	private TextureProgressBar Speedbar; 
	private TextureProgressBar Chestbar; 
	public override void _Ready()
	{
		Engine.MaxFps = 60;
		signals = Global.Signals(this);
		signals.StateChanged += OnStateChanged;
		signals.HandycapChanged +=  HandycapChanged;
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		Shufflebar = GetNode<TextureProgressBar>(nameof(Shufflebar));
		Speedbar = GetNode<TextureProgressBar>(nameof(Speedbar));
		Chestbar = GetNode<TextureProgressBar>(nameof(Chestbar));
		signals.EmitSignal(nameof(CustomSignals.StateChanged));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    private double timer = 0;
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
        if (Global.State == Gamestate.End)
        {
            Global.Speed = 1;
            Global.Shuffles = 3;
            Global.AmountChests = 3;
        }

        Shufflebar.Visible =
            Speedbar.Visible =
                Chestbar.Visible = (Global.State == Gamestate.Revolver || Global.State == Gamestate.Shuffle || Global.State == Gamestate.Result);

    }

	private void SetupTitle(){

		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f,5.0f,0.0f);
	}

	private void HandycapChanged()
	{
		Shufflebar.Value = Global.Shuffles;
		Speedbar.Value = Global.Speed;
		Chestbar.Value = Global.AmountChests;
	}
}
