namespace MauiControls;

class OuterCircleDrawable : IDrawable
{
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            _center = new PointF(_radius, _radius);
        }
    }
    public float Offset { get; set; }
    private float _radius;
    private PointF _center;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.DrawCircle(_center.X + Offset, _center.Y + Offset, Radius);
    }
}