using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public partial class AudioStreamPlayer : Godot.AudioStreamPlayer
{
	//get a dictionary here

	private Dictionary<string,Godot.AudioStreamPlayer> Sounds = new Dictionary<string,Godot.AudioStreamPlayer>();
	private CustomSignals signals;
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		signals.PlaySound += Playsound;

		Sounds.Add("plop",GetNode<Godot.AudioStreamPlayer>("../plop"));
		Sounds.Add("woosh",GetNode<Godot.AudioStreamPlayer>("../woosh"));
		Sounds.Add("flma",GetNode<Godot.AudioStreamPlayer>("../flma"));
		Sounds.Add("start",GetNode<Godot.AudioStreamPlayer>("../start"));
		Sounds.Add("slotmachine",GetNode<Godot.AudioStreamPlayer>("../slotmachine"));
		Sounds.Add("chestopen",GetNode<Godot.AudioStreamPlayer>("../chestopen"));
		Sounds.Add("shuffle",GetNode<Godot.AudioStreamPlayer>("../shuffle"));
		Sounds.Add("impact",GetNode<Godot.AudioStreamPlayer>("../impact"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void Playsound (string sound)
	{
		var current = Sounds[sound];

		current.Play();
	}

}
