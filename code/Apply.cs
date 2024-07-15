using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Apply : Button
{
    private int busindex;
    private int sfxBusindex;
    private int musicBusindex;
    private HSlider volumeSlider;
    private HSlider sfxVolumeSlider;
    private HSlider musicVolumeSlider;

    private CheckBox fullscreenCheckbox;
    private ConfigFile config;
    private const string Filename = "settings.cfg";
    private const string Section = "SETTINGS_MENU_SECTION";
    private const string Volume = "Volume";
    private const string SfxVolume = "SfxVolume";
    private const string MusicVolume = "MusicVolume";
    private const string Fullscreen = "Fullscreen";
    private const string Score = "Score";



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

       config = new ConfigFile();
       var result = config.Load(Filename);

        var volume = config.GetValue(Section, Volume, 100).AsSingle();
        var sfxVolume = config.GetValue(Section, SfxVolume, 100).AsSingle();
        var musicVolume = config.GetValue(Section, MusicVolume, 100).AsSingle();
        var fullscreen = config.GetValue(Section, Fullscreen, true).AsBool();
        Global.Highscore = config.GetValue(Section, Score, 0).AsInt32();

        UpdateUi(volume, sfxVolume, musicVolume, fullscreen);
        ApplySettings(volume, sfxVolume, musicVolume, fullscreen);
        if (result != Error.Ok)
        {
            SaveSettings(volume, sfxVolume, musicVolume, fullscreen);
        }
    }

    private void ApplySettings(float volume, float sfxVolume, float musicVolume, bool fullscreen)
    {
        AudioServer.SetBusVolumeDb(busindex, Mathf.LinearToDb(volume * 0.01f));
        AudioServer.SetBusMute(busindex, volume == 0f);

        AudioServer.SetBusVolumeDb(sfxBusindex, Mathf.LinearToDb(sfxVolume * 0.01f));
        AudioServer.SetBusMute(sfxBusindex, sfxVolume == 0f);

        AudioServer.SetBusVolumeDb(musicBusindex, Mathf.LinearToDb(musicVolume * 0.01f));
        AudioServer.SetBusMute(musicBusindex, musicVolume == 0f);

        DisplayServer.WindowSetMode(fullscreen
            ? DisplayServer.WindowMode.ExclusiveFullscreen
            : DisplayServer.WindowMode.Windowed);
    }

    private void SaveSettings(float volume, float sfxVolume, float musicVolume, bool fullscreen)
    {
        config.SetValue(Section, Volume, volume);
        config.SetValue(Section, SfxVolume, sfxVolume);
        config.SetValue(Section, MusicVolume, musicVolume);
        config.SetValue(Section, Fullscreen, fullscreen);
        config.SetValue(Section, Score, Global.Highscore);
        config.Save(Filename);
    }

    private void UpdateUi(float volume, float sfxVolume, float musicVolume, bool fullscreen)
    {
        fullscreenCheckbox.ButtonPressed = fullscreen;
        volumeSlider.Value = volume;
        sfxVolumeSlider.Value = sfxVolume;
        musicVolumeSlider.Value = musicVolume;
    }
    private void IsPressed()
    {
        var volume = (float)volumeSlider.Value;
        var sfxVolume = (float)sfxVolumeSlider.Value;
        var musicVolume = (float)musicVolumeSlider.Value;
        var fullscreen = fullscreenCheckbox.ButtonPressed;

        ApplySettings(volume, sfxVolume, musicVolume, fullscreen);
        SaveSettings(volume, sfxVolume, musicVolume, fullscreen);

        Global.TargetState = Gamestate.Title;
        Global.Fadestate = Fadestate.FadeOut;
    }
}
