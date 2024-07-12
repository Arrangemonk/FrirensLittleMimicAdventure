using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RotatingChests : MultiMeshInstance3D
{
    private int amount;
    private const float Distance = 3f;
    private float speed = 1.0f;
    private int shuffles;
    private int currentshuffle;
    private float timer = 0f;
    private bool success = true;
    private Random rnd;
    private Gamestate LastState { get; set; }
    private CustomSignals signals;
    private bool left;
    private bool right;
    private bool slotmachinestaring;
    private bool mimistarting;
    private byte? action = null;
    private Node3D grimoir;
    private Node3D mimic;
    private GpuParticles3D smokepuffReplacer;
    private GpuParticles3D smokepuffReplacee;
    private int current = 0;
    private int selected = 0;
    private int oldselected = 0;
    private int replacer;
    private Vector2 replacerInit;
    private int replacee;
    private Vector2 replaceeInit;
    private bool replacing = false;
    private bool showlabels = false;
    private bool impacted = false;
    private int[] valids;
    private int[] lablenames;
    private int[] oldlablenames;
    private static readonly Vector3 Hover = new Vector3(0, 2, 0);
    private float rotation = 0f;
    private float oldRotation;
    private float currentRotation;
    private readonly List<Label3D> labels = new List<Label3D>();
    private static readonly (float x, float y)[] Fall = { (0, 0), (0.1f, 0.6f), (0.2f, 0.8f), (0.3f, 1f), (0.4f, 1f), (0.5f, 0.9f), (0.6f, 0.7f), (0.8f, 0f), (0.8f, 0f), (0.9f, 0.05f), (1f, 0f) };
    private static readonly (float x, float y)[] Move = { (0, 0), (0.1f, 0f), (0.5f, 0.5f), (0.9f, 1f), (1f, 1f) };
    private static readonly (float x, float y)[] Stretch = { (0, 1f), (0.4f, 1.15f), (0.6f, 1.15f), (0.8f, 0.85f), (0.9f, 1.05f), (0.95f, 0.95f), (1f, 1f) };
    private static readonly (float x, float y)[] Squish = { (0, 1f), (0.3f, 0.9f), (0.6f, 0.875f), (0.8f, 1.1f), (0.9f, 0.9f), (1f, 1f) };
    private static int[] ValidPositions(int count)
        => count switch
        {
            3 => new[] { 3, 4, 5 },
            4 => new[] { 1, 3, 5, 7 },
            5 => new[] { 0, 2, 4, 6, 8 },
            6 => new[] { 0, 1, 2, 6, 7, 8 },
            7 => new[] { 0, 1, 2, 4, 6, 7, 8 },
            8 => new[] { 0, 1, 2, 3, 5, 6, 7, 8 },
            _ => new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
        };
    private static int Starter(int count)
    => count switch
    {
        4 => 1,
        6 => 1,
        8 => 1,
        _ => 4,
    };
    public override void _Ready()
    {
        signals = Global.Signals(this);
        grimoir = GetNode<Node3D>("../ChestGrimoir");
        mimic = GetNode<Node3D>("../ChestMimic");
        smokepuffReplacer = GetNode<GpuParticles3D>("SmokepuffReplacer");
        smokepuffReplacee = GetNode<GpuParticles3D>("SmokepuffReplacee");
        for (var i = 0; i < Global.MaxChests; i++)
        {
            var label = new Label3D
            {
                Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
                FontSize = 52,
            };
            labels.Add(label);
            AddChild(label);
        }

        signals.AmountChecksChanged += Setup;
        rnd = new Random();
    }
    private void Setup()
    {
        switch (Global.State)
        {
            case Gamestate.Revolver:
                SetupRevolver();
                break;
            case Gamestate.Shuffle:
                SetupShuffle();
                break;
            case Gamestate.Result:
                SetupResult();
                break;
            default:
                SetupHidden(true);
                break;
        }
    }
    public override void _Process(double delta)
    {
        if (Global.State != LastState)
        {
            LastState = Global.State;
            smokepuffReplacer.Emitting = false;
            smokepuffReplacee.Emitting = false;
            Setup();
        }
        switch (Global.State)
        {
            case Gamestate.Revolver:
                Revolver(delta);
                timer += (float)delta;
                break;
            case Gamestate.Shuffle:
                Shuffle(delta);
                timer += (float)delta * (1f + speed/10f);
                break;
            case Gamestate.Result:
                Result(delta);
                timer += (float)delta;
                break;
            default:
                break;
        }
    }
    private void SetupHidden(bool all = false)
    {
        mimic.Visible = false;
        grimoir.Visible = false;
        var start = all ? 0 : amount;
        for (var i = start; i < Global.MaxChests; i++)
        {
            Multimesh.SetInstanceTransform(i, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));
            labels[i].Scale = Vector3.Zero;
        }
    }
    private void Replace(int replacer, int replacee)
    {
        if (current == replacer)
            current = replacee;
        else if (current == replacee)
            current = replacer;

        //valids?
        var v1 = Array.IndexOf(valids, replacer);
        var v2 = Array.IndexOf(valids, replacee);
        (lablenames[v1], lablenames[v2]) = (lablenames[v2], lablenames[v1]);
    }
    private void SetupShuffle()
    {
        speed = Global.Speed;
        shuffles = Global.Shuffles;
        currentshuffle = 0;
        GD.Print("shuffle");
        Global.CameraPosition = new Vector3(0.0f, 5.0f, 6.0f);
        Global.CameraTarget = new Vector3(0.0f, 1.0f, 0.0f);
        amount = Global.AmountChests;
        valids = ValidPositions(amount);
        lablenames = new int[amount];
        oldlablenames = new int[amount];
        for (var i = 0; i < amount; i++)
        {
            lablenames[i] = i + 1;
            oldlablenames[i] = i + 1;
        }
        replacing = false;
        current = Starter(amount);
        showlabels = true;
        timer = 0;
        grimoir.Visible = true;
        grimoir.Position = new Vector3((current % 3) * 3f - 3f, 0f, -1 * ((current - (current % 3)) - 3f));
        var player = grimoir.GetNode<AnimationPlayer>("GrimoirPlayer");
        player.Seek(0.5);
        Global.Playsound(this,Audio.ChestOpen);
    }
    private void Shuffle(double delta)
    {
        //predefine all as invalid
        SetupHidden(true);
        //place valid ones
        foreach (var i in valids)
        {
            var position = new Vector3((i % 3) * 3f - 3f, 0f, -1 * ((i - (i % 3)) - 3f));
            Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, position));
            if (!showlabels)
                continue;
            labels[i].Position = position + Hover;
            labels[i].Scale = Vector3.One;
            labels[i].Text = (oldlablenames[Array.IndexOf(valids, i)]).ToString();
        }
        //we dont want to confuse people with animation during fadestate
        if (Global.Fadestate == Fadestate.FadeOut)
            return;
        //determine current pair
        if (!replacing && !showlabels)
        {
            if (currentshuffle == shuffles)
            {
                Global.Fadestate = Fadestate.FadeOut;
                Global.TargetState = Gamestate.Revolver;
                return;
            }
            currentshuffle++;
            Global.Playsound(this,Audio.Shuffle);
            impacted = true;
            replacing = true;
            timer = 0;
            replacer = valids[rnd.Next(0, valids.Length)];
            do
            {
                replacee = valids[rnd.Next(0, valids.Length)];
            } while (replacer == replacee);
            for (var i = 0; i < lablenames.Length; i++)
            {
                oldlablenames[i] = lablenames[i];
            }
            Replace(replacer, replacee);
            GD.Print("current:",Array.IndexOf(valids,current)+1);

            replacerInit = new Vector2((replacer % 3) * 3f - 3f, (replacer - (replacer % 3)) - 3f);
            replaceeInit = new Vector2((replacee % 3) * 3f - 3f, (replacee - (replacee % 3)) - 3f);
            smokepuffReplacer.Position = new Vector3(replacerInit.X, 0, -replacerInit.Y);
            smokepuffReplacee.Position = new Vector3(replaceeInit.X, 0, -replaceeInit.Y);
        }
        if (showlabels)
        {
            if (timer >= 2f)
            {
                showlabels = false;
                foreach (var i in valids)
                {
                    //labels[i].Scale = Vector3.Zero;
                }
            }
            grimoir.Visible = true;
            Multimesh.SetInstanceTransform(current, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));

            return;
        }

        var progress = Global.Lerp(0f, 1f, timer);

        //var scaley = 1f;//1f + Mathf.Sin((progress + 0.25f) * 2 * Mathf.Pi) * 0.2f;
        //var scalex = 1f;//1.05f + Mathf.Sin((progress - 0.25f) * 2 * Mathf.Pi) * 0.025f;
        //var func = Mathf.Sin(progress*  MathF.PI);

        var scaley = Global.Spine1d(Stretch, Mathf.Clamp(progress, 0, 1));
        var scalex = Global.Spine1d(Squish, Mathf.Clamp(progress, 0, 1));
        var func = Global.Spine1d(Fall, Mathf.Clamp(progress, 0, 1));
        var pt = Global.Spine1d(Move, Mathf.Clamp(progress, 0, 1));

        //animate replacement
        var currentreplacer = Global.Lerp(replacerInit, replaceeInit, pt);
        var currentreplacee = Global.Lerp(replaceeInit, replacerInit, pt);

        var replacerpos = new Vector3(currentreplacer.X, func * 4f, -currentreplacer.Y);
        Multimesh.SetInstanceTransform(replacer,
        new Transform3D(new Vector3(scalex, 0, 0), new Vector3(0, scaley, 0), new Vector3(0, 0, 1), replacerpos));
        labels[replacer].Position = replacerpos + Hover;

        var replaceepos = new Vector3(currentreplacee.X, func * 2f, -currentreplacee.Y);
        Multimesh.SetInstanceTransform(replacee,
         new Transform3D(new Vector3(scalex, 0, 0), new Vector3(0, scaley, 0), new Vector3(0, 0, 1), replaceepos));
        labels[replacee].Position = replaceepos + Hover;

        if (impacted && timer >= 0.75f)
        {
            smokepuffReplacer.Emitting = true;
            smokepuffReplacee.Emitting = true;
        }
        if (impacted && timer >= 0.8f)
        {
            impacted = false;
            signals.EmitSignal(nameof(CustomSignals.ShakeStart));
            Global.Playsound(this,Audio.Impact);
        }
        if (timer >= 0.95f)
        {
            smokepuffReplacer.Emitting = false;
            smokepuffReplacee.Emitting = false;
        }

        if (timer >= 1f)
            replacing = false;
    }
    private void SetupRevolver()
    {
        GD.Print("revolver");
        Global.CameraPosition = new Vector3(0.0f, 1.5f, 6.0f);
        Global.CameraTarget = new Vector3(0.0f, 1.0f, -0.0f);
        amount = Global.AmountChests;
        oldRotation = 0;
        currentRotation = 0;
        SetupHidden(true);
        timer = 1;
        oldselected = valids[0];
        selected = valids[0];
    }
    private void Revolver(double delta)
    {
        if (timer > 1)
        {
            ProcessActions();
            return;
        }
        var tmprotation = Global.OvershootSmoothStep(oldRotation, currentRotation, timer);
        var selectedindex = Array.IndexOf(valids, oldselected);
        for (var i = 0; i < amount; i++)
        {
            var angle = 2.0f * MathF.PI * (i - selectedindex) / amount + tmprotation;
            var position = new Vector3(MathF.Sin(angle) * Distance, 0f, MathF.Cos(angle) * Distance);
            labels[i].Position = position + Hover;
            labels[i].Scale = Vector3.One;
            labels[i].Text = (i + 1).ToString();
            Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, position));
        }
    }
    private void ResetRevolver(Direction direction)
    {
        timer = 0;
        rotation = (direction == Direction.Left ? 1 : -1) * 2.0f * MathF.PI / amount;
        oldRotation = 0f;
        currentRotation = rotation;
        SetupHidden();
    }
    private void SetupResult()
    {
        timer = 0;
        SetupHidden(true);
        //we dont use the 0th, it is replaced by mimic or grimoir
        for (var i = 1; i < amount; i++)
        {
            var angle = 2.0f * MathF.PI * i / amount;
            Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(MathF.Sin(angle) * Distance, 0f, MathF.Cos(angle) * Distance)));
        }

        if (selected == current)
        {
            grimoir.Visible = true;
            grimoir.Position = new Vector3(0f, 0f, Distance);
            var player = grimoir.GetNode<AnimationPlayer>("GrimoirPlayer");
            player.Seek(0.5);
            slotmachinestaring = true;
        }
        else
        {
            mimistarting = true;
            mimic.Visible = true;
            mimic.Position = new Vector3(0f, 0f, Distance);
            var player = mimic.GetNode<AnimationPlayer>("MimicPlayer");
            player.Seek(0.5);
        }
    }
    private void Result(double delta)
    {
        if (timer < 0.7f)
            return;
        if (slotmachinestaring)
        {
            slotmachinestaring = false;
            signals.EmitSignal(nameof(CustomSignals.SlotmachineActivated));
            Global.Playsound(this,Audio.Slotmachine);
        }

        if (mimistarting)
        {
            mimistarting = false;
            Global.Fadestate = Fadestate.FadeOut;
            Global.TargetState = Gamestate.End;
        }

    }
    public override void _Input(InputEvent @event)
    {
        if (Global.State != Gamestate.Revolver)
            return;
        if (timer < 0.25)
            return;
        if (@event.IsAction("Left"))
        {
            action = 1;
        }
        if (@event.IsAction("Right"))
        {
            action = 2;
        }
        if (@event.IsAction("Return"))
        {
            action = 3;
        }
    }
    private void ProcessActions()
    {
        switch (action)
        {
            case 3:
                //Global.Fadestate = Fadestate.FadeOut;
                Global.State = Gamestate.Result;
                Global.Playsound(this,Audio.ChestOpen);
                break;
            case 1:
                ResetRevolver(Direction.Left);
                oldselected = selected;
                selected = valids[(valids.ToList().IndexOf(selected) - 1 + amount) % amount];
                Global.Playsound(this,Audio.Plop);
                GD.Print("selected:", Array.IndexOf(valids, selected) + 1);
                break;
            case 2:
                ResetRevolver(Direction.Right);
                oldselected = selected;
                selected = valids[(valids.ToList().IndexOf(selected) + 1 + amount) % amount];
                Global.Playsound(this,Audio.Plop);
                GD.Print("selected:", Array.IndexOf(valids, selected) + 1);
                break;
        }
        action = null;
    }

}
