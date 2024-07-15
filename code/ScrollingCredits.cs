using Godot;
using System;
using System.IO;
using System.Linq;

public partial class ScrollingCredits : Label3D
{
    // Called when the node enters the scene tree for the first time.

    private Vector3 initialPosition;
    private Vector3 offset = new(0, 0, 8f);
    private string[] credits;
    private int currentLine = -1;
    private CustomSignals signals;
    private float timer;
    public override void _Ready()
    {
        signals = Global.Signals(this);
        initialPosition = Position;
        credits = File.ReadAllText("Credits.txt").Split('\r','\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        foreach(var entry in credits)
            GD.Print(entry);
        signals.StateChanged += StateChanged;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Global.State != Gamestate.Credits) return;
        timer += (float)delta;
        Position = initialPosition + offset * timer;
        if (timer > 3.8f)
        {
            ChangeLine();
        }
        GD.Print(timer);
    }

    private void StateChanged()
    {
        Visible = Global.State == Gamestate.Credits;
        if (Global.State != Gamestate.Credits) return;
        Global.CameraPosition = new Vector3(0.0f, 5.0f, 6.0f);
        Global.CameraTarget = new Vector3(0.0f, 6.0f, 10f);
        currentLine = -1;
        ChangeLine();
    }

    private void ChangeLine()
    {
        currentLine = (currentLine + 1) % credits.Length;
        Text = credits[currentLine];
        timer = 0;
        GD.Print("line changed - ", credits[currentLine]);
    }
}
