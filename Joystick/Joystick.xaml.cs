namespace MauiControls;

public enum JoystickState
{
    Ready,
    Picked
}

public partial class Joystick : ContentView
{
    private const int SizeFactor = 2;
    public const int CircleJoystickSizeFactor = 10;
    private JoystickState _state;

    private float _offset;
    public float Offset
    {
        get => _offset;
        set
        {
            _offset = value;
            joystickCircle.Offset = value;
            outerCircle.Offset = value;
        }
    }

    public float Size
    {
        get => (float)OuterCircleView.WidthRequest;
        set
        {
            var calculatedSize = value + Offset * 2;

            OuterCircleView.WidthRequest = calculatedSize;
            OuterCircleView.HeightRequest = calculatedSize;
            JoystickCircleView.WidthRequest = calculatedSize;
            JoystickCircleView.HeightRequest = calculatedSize;

            outerCircle.Radius = value / SizeFactor;
            joystickCircle.Radius = value / (SizeFactor * CircleJoystickSizeFactor);

            OuterCircleView.Invalidate();
            JoystickCircleView.Invalidate();
        }
    }

    private Color _stickColor;
    public Color StickColor
    {
        get => _stickColor;
        set
        {
            _stickColor = value;
            joystickCircle.Color = value;
        }
    }

    private Color _circleColor;
    public Color CircleColor
    {
        get => _circleColor;
        set
        {
            _circleColor = value;
            outerCircle.Color = value;
        }
    }

    public uint ReturnAnimation { get; set; }

    public event Action<object, JoystickActionEventArgs> JoystickMoved;
    public event Action<object, JoystickActionEventArgs> JoystickInteractionStopped;

    private float _unitConvertionFactor;
    private AngleMeasure _angleMeasureUnits;
    public AngleMeasure AngleMeasureUnits
    {
        get => _angleMeasureUnits;
        set
        {
            _angleMeasureUnits = value;
            _unitConvertionFactor = value == AngleMeasure.Radians ? 1 : 180 / MathF.PI;
        }
    }

    public Joystick()
	{
		InitializeComponent();
        StickColor = Colors.Blue;
        CircleColor = Colors.Black;
        ReturnAnimation = 0;
        AngleMeasureUnits = AngleMeasure.Radians;
        _state = JoystickState.Ready;
    }

    private void JoystickCircleView_StartInteraction(object sender, TouchEventArgs e)
    {
        _state = e.Touches[0].Distance(joystickCircle.Center) > joystickCircle.Radius ? JoystickState.Ready : JoystickState.Picked;
    }

    private void JoystickCircleView_EndInteraction(object sender, TouchEventArgs e)
    {
        if (_state != JoystickState.Picked)
            return;

        var point = joystickCircle.Position;

        joystickCircle.Reset();
        JoystickCircleView.Invalidate();

        if (ReturnAnimation > 0)
        {
            JoystickCircleView.TranslationX = point.X - joystickCircle.Center.X;
            JoystickCircleView.TranslationY = point.Y - joystickCircle.Center.Y;
            JoystickCircleView.TranslateTo(0, 0, ReturnAnimation);
        }
        _state = JoystickState.Ready;
        var eventArgs = new JoystickActionEventArgs();
        JoystickMoved?.Invoke(this, eventArgs);
        JoystickInteractionStopped?.Invoke(this, eventArgs);
    }

    private void JoystickCircleView_DragInteraction(object sender, TouchEventArgs e)
    {
        if (_state != JoystickState.Picked)
            return;

        var touchPoint = e.Touches[0];
        var distance = touchPoint.Distance(joystickCircle.Center);
        var angle = MathF.Atan2(touchPoint.Y - joystickCircle.Center.Y, touchPoint.X - joystickCircle.Center.X);
        if (distance > outerCircle.Radius)
        {
            touchPoint.X = joystickCircle.Center.X + outerCircle.Radius * MathF.Cos(angle);
            touchPoint.Y = joystickCircle.Center.Y + outerCircle.Radius * MathF.Sin(angle);
            distance = distance > joystickCircle.Radius ? joystickCircle.Radius : distance;
        }

        joystickCircle.Position = touchPoint;
        JoystickCircleView.Invalidate();

        var eventArgs = new JoystickActionEventArgs()
        {
            Angle = NormalizeAngle(angle) * _unitConvertionFactor,
            Deviation = distance,
        };
        JoystickMoved?.Invoke(this, eventArgs);
    }

    private const float HalfPI = MathF.PI / 2.0f;
    private float NormalizeAngle(float angle) => angle switch
    {
        >= 0 and <= MathF.PI => angle + HalfPI,
        < 0 and >= -HalfPI => angle + HalfPI,
        < -HalfPI and > -MathF.PI => angle + MathF.PI * 2.5f,
        _ => 0,
    };
}