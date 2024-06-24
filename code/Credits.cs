using Godot;
using System;

public partial class Credits : Godot.RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	private Gamestate oldstate;
	public override void _Ready()
	{
		oldstate = Gamestate.Title;
		GetViewport().SizeChanged += onViewportResize;
		onViewportResize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = false;
		if (Global.State == Gamestate.Credits)
		{
			if(oldstate != Global.State)
			{
				oldstate = Global.State;
				SetupCredits();
			}
			Visible = true;
		}
	}

	private void SetupCredits(){

		Global.CameraPosition = new Vector3(0.0f,5.0f,6.0f);
		Global.CameraTarget = new Vector3(0.0f, 1.0f, 12f);
	}

	private void onViewportResize()
	{
		var screensize = GetViewport().GetVisibleRect().Size;
		var factor = screensize.X /  1280f;
		Size = screensize;
		AddThemeFontSizeOverride("normal_font_size",(int)(factor * 40f));
		AddThemeFontSizeOverride("bold_font_size",(int)(factor * 50f));
		Position = new Vector2(0,0);
	}
}
