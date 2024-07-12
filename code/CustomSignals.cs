using Godot;
using System;

public partial class CustomSignals : Node
{
    [Signal]
    public delegate void PlayerChangedEventHandler();

    [Signal]
    public delegate void StateChangedEventHandler();

    [Signal]
    public delegate void PlaySoundEventHandler(int sound);

    [Signal]
    public delegate void AmountChecksChangedEventHandler();

    [Signal]
    public delegate void ControlsUnlockedEventHandler();

    [Signal]
    public delegate void SlotmachineActivatedEventHandler();

    [Signal]
    public delegate void ShakeStartEventHandler();

    [Signal]
    public delegate void HandycapChangedEventHandler();

}
