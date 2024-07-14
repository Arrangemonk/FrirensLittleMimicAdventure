using Godot;
using System;

public partial class Apply : Button
{
    private int busindex;
    private int sfxBusindex;
    private int musicBusindex;
    private HSlider volumeSlider;
    private HSlider sfxVolumeSlider;
    private HSlider musicVolumeSlider;

    private CheckBox fullscreenCheckbox;
    
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += IsPressed;
        busindex = AudioServer.GetBusIndex("Master");
        sfxBusindex = AudioServer.GetBusIndex("SFX");
        musicBusindex = AudioServer.GetBusIndex("MUSIC");
        volumeSlider = GetNode<HSlider>("../VolumeSlider");
        sfxVolumeSlider = GetNode<HSlider>("../SfxVolumeSlider");
        musicVolumeSlider = GetNode<HSlider>("../MusicVolumeSlider");
        fullscreenCheckbox = GetNode<CheckBox>("../FullscreenCheckbox");

        fullscreenCheckbox.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
        volumeSlider.Value = 100f * Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busindex));
        sfxVolumeSlider.Value = 100f * Mathf.DbToLinear(AudioServer.GetBusVolumeDb(sfxBusindex));
        musicVolumeSlider.Value = 100f * Mathf.DbToLinear(AudioServer.GetBusVolumeDb(musicBusindex));
    }


	private void IsPressed()
    {
        var slidervalue = (float)volumeSlider.Value / 100f;
        var sfxSlidervalue = (float)sfxVolumeSlider.Value / 100f;
        var musicSlidervalue = (float)musicVolumeSlider.Value / 100f;

        AudioServer.SetBusVolumeDb(busindex,Mathf.LinearToDb(slidervalue));
        AudioServer.SetBusMute(busindex, slidervalue == 0f);

        AudioServer.SetBusVolumeDb(sfxBusindex, Mathf.LinearToDb(sfxSlidervalue));
        AudioServer.SetBusMute(sfxBusindex, sfxSlidervalue == 0f);

        AudioServer.SetBusVolumeDb(musicBusindex, Mathf.LinearToDb(musicSlidervalue));
        AudioServer.SetBusMute(musicBusindex, musicSlidervalue == 0f);

        DisplayServer.WindowSetMode(fullscreenCheckbox.ButtonPressed?   DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);

        Global.TargetState = Gamestate.Title;
        Global.Fadestate = Fadestate.FadeOut;
    }
}
