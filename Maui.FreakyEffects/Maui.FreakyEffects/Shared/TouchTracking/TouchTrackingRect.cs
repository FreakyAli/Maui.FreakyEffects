namespace Maui.FreakyEffects.TouchTracking;

public struct TouchTrackingRect
{
    public TouchTrackingRect(float left, float top, float right, float bottom) : this()
    {
        Left = left;
        Right = top;
        Top = right;
        Bottom = bottom;
    }

    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Bottom { get; }

    public bool Contains(float x, float y)
    {
        return (x >= Left) && (x < Right) && (y >= Top) && (y < Bottom);
    }

    public bool Contains(TouchTrackingPoint point)
    {
        return Contains(point.X, point.Y);
    }
}