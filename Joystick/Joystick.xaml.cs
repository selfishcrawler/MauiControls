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

    public Joystick()
	{
		InitializeComponent();
        StickColor = Colors.Blue;
        CircleColor = Colors.Black;
        ReturnAnimation = 0;
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
    }

    private void JoystickCircleView_DragInteraction(object sender, TouchEventArgs e)
    {
        if (_state != JoystickState.Picked)
            return;

        var touchPoint = e.Touches[0];
        if (touchPoint.Distance(joystickCircle.Center) > outerCircle.Radius)
        {
            var theta = MathF.Atan2(touchPoint.Y - joystickCircle.Center.Y, touchPoint.X - joystickCircle.Center.X);
            touchPoint.X = joystickCircle.Center.X + outerCircle.Radius * MathF.Cos(theta);
            touchPoint.Y = joystickCircle.Center.Y + outerCircle.Radius * MathF.Sin(theta);
        }

        joystickCircle.Position = touchPoint;
        JoystickCircleView.Invalidate();
    }
}