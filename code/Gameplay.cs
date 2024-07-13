using Godot;
using System;

public partial class Gameplay : Node3D
{

	private CustomSignals signals;
	private NinePatchRect shufflebar; 
	private NinePatchRect speedbar; 
	private NinePatchRect chestbar;
    private Sprite2D charball;
    private Texture2D[] textures = new[]{
        GD.Load<Texture2D>("res://images/frieren_ball.png"),
        GD.Load<Texture2D>("res://images//fern_ball.png"),
        GD.Load<Texture2D>("res://images//stark_ball.png")
    };
public override void _Ready()
	{
		Engine.MaxFps = 60;
		signals = Global.Signals(this);
		signals.StateChanged += OnStateChanged;
		signals.HandycapChanged += HandycapChanged;
        signals.PlayerChanged += PlayerChanged;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
		shufflebar = GetNode<NinePatchRect>("Hud//ShuffleBar");
		speedbar = GetNode<NinePatchRect>("Hud//SpeedBar");
		chestbar = GetNode<NinePatchRect>("Hud//ChestBar");
        charball = GetNode<Sprite2D>("Hud//CharBall");
        HandycapChanged();
        OnStateChanged();
        PlayerChanged();
    }

    private void PlayerChanged()
    {
        charball.Texture = textures[(int)Global.Player];
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsAction("Escape") || Global.Fadestate != Fadestate.None) 
            return;
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
            HandycapChanged();
        }
        charball.Visible =
        shufflebar.Visible =
        speedbar.Visible =
        chestbar.Visible = (Global.State == Gamestate.Revolver || Global.State == Gamestate.Shuffle || Global.State == Gamestate.Result);

    }

	private void SetupTitle(){

		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f,5.0f,0.0f);
	}

	private void HandycapChanged()
	{
        shufflebar.SetSize(new Vector2(GetWith(Global.Shuffles, Global.MaxShuffles),shufflebar.Size.Y));
        speedbar.SetSize(new Vector2(GetWith(Global.Speed, Global.MaxSpeed), speedbar.Size.Y));
        chestbar.SetSize(new Vector2(GetWith(Global.AmountChests, Global.MaxChests), chestbar.Size.Y));
    }

    private float GetWith(float value,float maxvalue)
    {
        var max = GetViewport().GetVisibleRect().Size.X * 0.5f - 156f;
        return (MathF.Floor(Global.Lerp(16, max, value / maxvalue)));
    }
}
