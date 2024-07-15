using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public enum Audio
{
    Plop,
    Woosh,
    Flma,
    Start,
    Slotmachine,
    ChestOpen,
    Shuffle,
    Impact,
    BackgroundMusic,
    GameOver,
    Angry,
}

public partial class Flma : AudioStreamPlayer
{
    private readonly Dictionary<Audio, AudioStreamPlayer> sounds = new();
	private CustomSignals signals;

    public override void _Ready()
	{
		signals = Global.Signals(this);
		signals.PlaySound += Playsound;
        signals.StopSound += Stopsound;

        sounds.Add(Audio.Plop, GetNode<AudioStreamPlayer>(nameof(Audio.Plop)));
		sounds.Add(Audio.Woosh, GetNode<AudioStreamPlayer>(nameof(Audio.Woosh)));
		sounds.Add(Audio.Flma, this);
		sounds.Add(Audio.Start, GetNode<AudioStreamPlayer>(nameof(Audio.Start)));
		sounds.Add(Audio.Slotmachine, GetNode<AudioStreamPlayer>(nameof(Audio.Slotmachine)));
		sounds.Add(Audio.ChestOpen, GetNode<AudioStreamPlayer>(nameof(Audio.ChestOpen)));
		sounds.Add(Audio.Shuffle, GetNode<AudioStreamPlayer>(nameof(Audio.Shuffle)));
		sounds.Add(Audio.Impact, GetNode<AudioStreamPlayer>(nameof(Audio.Impact)));
		sounds.Add(Audio.BackgroundMusic, GetNode<AudioStreamPlayer>(nameof(Audio.BackgroundMusic)));
        sounds.Add(Audio.GameOver, GetNode<AudioStreamPlayer>(nameof(Audio.GameOver)));
        sounds.Add(Audio.Angry, GetNode<AudioStreamPlayer>(nameof(Audio.Angry)));

        Global.CreateOneshotTimer(this, 2.5, () => {
            signals.EmitSignal(nameof(CustomSignals.PlaySound), (int)Audio.BackgroundMusic);
            GD.Print("does?");
        });
        
    }

	private void Playsound (int sound)
	{
        sounds[(Audio)sound].Play();
	}


    private void Stopsound(int sound)
    {
        sounds[(Audio)sound].Stop();
    }

}
