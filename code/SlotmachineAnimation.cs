using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class SlotmachineAnimation : AnimationPlayer
{
    // Called when the node enters the scene tree for the first time.

    private MeshInstance3D cylinder1;
    private MeshInstance3D cylinder2;
    private MeshInstance3D cylinder3;

    private Node3D slotmachine;
    private Vector3 startposition = new Vector3(0f, -5f, 0f);
    private Vector3 restingposition = Vector3.Zero;
    private float timer = 0f;
    private double timeroffset = 0f;
    private float curr1;
    private float curr2;
    private float curr3;

    public float? Next1 { get; set; }
    public float? Next2 { get; set; }
    public float? Next3 { get; set; }
    private readonly Random rnd = new Random();
    private bool animating = false;
    private CustomSignals signals;


    public override void _Ready()
    {
        signals = Global.Signals(this);
        cylinder1 = GetNode<MeshInstance3D>("../Cylinder_001");
        cylinder2 = GetNode<MeshInstance3D>("../Cylinder_002");
        cylinder3 = GetNode<MeshInstance3D>("../Cylinder_003");
        slotmachine = GetNode<Slot>("../../Slotmachine");
        var mat1 = cylinder1.MaterialOverride as ShaderMaterial;
        mat1.SetShaderParameter("offset", curr1 = rnd.Next(0, 4) * 1.0f);
        var mat2 = cylinder2.MaterialOverride as ShaderMaterial;
        mat2.SetShaderParameter("offset", curr2 = rnd.Next(0, 4) * 1.0f);
        var mat3 = cylinder3.MaterialOverride as ShaderMaterial;
        mat3.SetShaderParameter("offset", curr3 = rnd.Next(0, 4) * 1.0f);

        signals.SlotmachineActivated += Reset;

        GD.Print("speed:", Global.Speed);
        GD.Print("Shuffles:", Global.Shuffles);
        GD.Print("AmountChests:", Global.AmountChests);
        signals.EmitSignal(nameof(CustomSignals.HandycapChanged));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timeroffset += delta;
        if (!animating || Global.State != Gamestate.Result)
            return;
        if (timer > 5f)
        {
            Global.Fadestate = Fadestate.FadeOut;
            Global.TargetState = Gamestate.Shuffle;
            animating = false;
            signals.EmitSignal(nameof(CustomSignals.HandycapChanged));
            return;
        }
        timer += (float)delta;
        var x = MathF.Max(0f, MathF.Min(1, MathF.Min(6.5f - 1.3f * timer, timer * 1.3f)));
        slotmachine.Position = Global.OvershootSmoothStep(startposition, restingposition, x);
    }

    public void Reset()
    {
        var mat1 = cylinder1.MaterialOverride as ShaderMaterial;
        mat1.SetShaderParameter("resettime", timeroffset);
        mat1.SetShaderParameter("initialoffset", curr1);
        mat1.SetShaderParameter("offset", curr1 = Next1 ?? rnd.Next(0, 4) * 1.0f);

        var mat2 = cylinder2.MaterialOverride as ShaderMaterial;
        mat2.SetShaderParameter("resettime", timeroffset);
        mat2.SetShaderParameter("initialoffset", curr2);
        mat2.SetShaderParameter("offset", curr2 = Next2 ?? rnd.Next(0, 4) * 1.0f);

        var mat3 = cylinder3.MaterialOverride as ShaderMaterial;
        mat3.SetShaderParameter("resettime", timeroffset);
        mat3.SetShaderParameter("initialoffset", curr3);
        mat3.SetShaderParameter("offset", curr3 = Next3 ?? rnd.Next(0, 4) * 1.0f);

        Global.SlotResult = CountEquals((int)curr1 % 4, (int)curr2 % 4, (int)curr3 % 4);
        GD.Print("slotvalues:", curr1, curr2, curr3);
        GD.Print("slotresult", Global.SlotResult);

        Global.Speed = Math.Min(Global.MaxSpeed, Global.Speed + (Global.SlotResult == 1 ? 1 : 0));
        Global.Shuffles = Math.Min(Global.MaxShuffles, Global.Shuffles + (Global.SlotResult == 2 ? 1 : 0));
        Global.AmountChests = Math.Min(Global.MaxChests, Global.AmountChests + (Global.SlotResult == 3 ? 1 : 0));

        GD.Print("speed:", Global.Speed);
        GD.Print("Shuffles:", Global.Shuffles);
        GD.Print("AmountChests:", Global.AmountChests);

        timer = 0;
        animating = true;
        Play("Scene");
        Seek(0);
    }

    private static int CountEquals(params int[] input)
    {
        var stuff = new Dictionary<int,int>();

        foreach (var i in input)
        {
            if(!stuff.Keys.Contains(i))
                    stuff.Add(i, 0);
            stuff[i] += 1;
        }

        return stuff.Values.Max();
    }
}
