using SkiaSharp;

namespace Maui.FreakyEffects.SkiaScene;

public interface ISKScene
{
    void Render(SKCanvas canvas);
    void Zoom(SKPoint point, float scale);
    void ZoomByScaleFactor(SKPoint point, float scaleFactor);
    void MoveToPoint(SKPoint point);
    void MoveByVector(SKPoint vector);
    void Rotate(SKPoint point, float radians);
    void RotateByRadiansDelta(SKPoint point, float radiansDelta);
    SKPoint GetCanvasPointFromViewPoint(SKPoint viewPoint);
    SKPoint GetCenter();
    float GetAngleInRadians();
    float GetScale();
    float MaxScale { get; set; }
    float MinScale { get; set; }
    SKPoint ScreenCenter { get; set; }
    SKRect CenterBoundary { get; set; }
}