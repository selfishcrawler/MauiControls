namespace MauiControls;

public enum AngleMeasure
{
    Degrees,
    Radians,
}

public class JoystickActionEventArgs : EventArgs
{
    public float Angle { get; set; }
    public float Deviation { get; set; }
}