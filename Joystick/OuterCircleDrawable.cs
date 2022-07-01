namespace MauiControls;

class JoystickDrawable : IDrawable
{
    private const int CircleJoystickSizeFactor = Joystick.CircleJoystickSizeFactor;
    private float _radius;
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            Center = new PointF(_radius * CircleJoystickSizeFactor + Offset, _radius * CircleJoystickSizeFactor + Offset);
            Position = Center;
        }
    }
    public PointF Center { get; private set; }

    public PointF Position { get; set; }

    public float Offset { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Blue;
        canvas.FillCircle(Position, Radius);
    }

    public void Reset()
    {
        Position = Center;
    }
}