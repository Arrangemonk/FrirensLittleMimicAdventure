using Godot;
using System;

public partial class Camera3D : Godot.Camera3D
{
    //constants
    private const float distance = 0.2f;
    private const float speed = 0.2f;
    private readonly Vector3 shakedelta = new(0, -0.2f, 0);
    private readonly RandomNumberGenerator rng = new();
    private CustomSignals signals;
    //vareables
    private Vector3 old = Vector3.Zero;
    private Vector3 curr = Vector3.Zero;
    private float timer = 0;
    private float shaketimer = 1f;

    public override void _Ready()
    {
        signals = Global.Signals(this);
        old = TmpDistance3();
        curr = TmpDistance3();

        Global.CameraPosition = new Vector3(0.0f, 1.5f, 6.0f);
        Global.CameraTarget = new Vector3(0.0f, 5.0f, 0.0f);

        signals.ShakeStart += BeginShaking;
    }

    private void BeginShaking()
    {
        shaketimer = 0;
    }

    private void Reset()
    {
        timer = 0;
        old = curr;
        curr = TmpDistance3();
    }

    public override void _Process(double delta)
    {
        var fdelta = (float)delta;
        shaketimer = MathF.Max(0, MathF.Min(1, shaketimer + fdelta * 1.5f));
        timer += fdelta * speed;
        if (timer > 1) Reset();
        Position = Global.SmoothStep(old, curr,timer) 
                 + Global.Bounce(Global.CameraPosition + shakedelta, Global.CameraPosition, shaketimer);
        LookAt(Global.CameraTarget);
    }

    private Vector3 TmpDistance3()
    {
        return new Vector3(
            rng.RandfRange(-distance, distance),
            rng.RandfRange(-distance, distance),
            rng.RandfRange(-distance, distance));
    }
}
