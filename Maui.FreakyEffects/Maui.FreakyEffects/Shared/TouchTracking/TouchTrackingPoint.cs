namespace Maui.FreakyEffects.TouchTracking;

public struct TouchTrackingPoint
{
    public TouchTrackingPoint(float x, float y) : this()
    {
        X = x;
        Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }
}

