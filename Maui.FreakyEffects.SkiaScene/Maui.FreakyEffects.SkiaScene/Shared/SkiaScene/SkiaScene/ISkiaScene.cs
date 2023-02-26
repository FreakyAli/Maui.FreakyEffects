using System;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using static Microsoft.Maui.Controls.Internals.GIFBitmap;

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

public interface ISKSceneRenderer
{
    void Render(SKCanvas canvas, float angleInRadians, SKPoint center, float scale);
}

public static class SKPointExtensions
{
    public static float GetMagnitude(this SKPoint point)
    {
        return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
    }
}

public class SKScene : ISKScene
{
    protected readonly ISKSceneRenderer _sceneRenderer;
    protected SKMatrix Matrix = SKMatrix.CreateIdentity();

    protected const float DefaultMaxScale = 8;
    protected const float DefaultMinScale = 1f / 8;

    public SKScene(ISKSceneRenderer sceneRenderer)
    {
        _sceneRenderer = sceneRenderer;
    }

    public void Render(SKCanvas canvas)
    {
        canvas.Concat(ref Matrix);
        var center = GetCenter();
        var angleInRadians = GetAngleInRadians();
        var scale = GetScale();
        _sceneRenderer.Render(canvas, angleInRadians, center, scale);
    }

    public float MaxScale { get; set; } = DefaultMaxScale;
    public float MinScale { get; set; } = DefaultMinScale;
    public SKRect CenterBoundary { get; set; }
    public SKPoint ScreenCenter { get; set; }

    public void MoveByVector(SKPoint vector)
    {
        SKMatrix invertedMatrix;
        if (!Matrix.TryInvert(out invertedMatrix))
        {
            return;
        }
        var resultPoint = invertedMatrix.MapVector(vector.X, vector.Y);
        Matrix.PreConcat(SKMatrix.CreateTranslation(resultPoint.X, resultPoint.Y));
        if (CenterBoundary.IsEmpty)
        {
            return;
        }
        var center = GetCenter();
        if (!CenterBoundary.Contains(center))
        {
            //rollback
            Matrix.PreConcat(SKMatrix.CreateTranslation(-resultPoint.X, -resultPoint.Y));
        }
    }

    public void MoveToPoint(SKPoint point)
    {
        var center = GetCenter();
        SKPoint diff = center - point;
        Matrix.PreConcat(SKMatrix.CreateTranslation(diff.X, diff.Y));
    }

    public void Rotate(SKPoint point, float radians)
    {
        var currentAngle = GetAngleInRadians();
        var angleDiff = radians - currentAngle;
        Matrix.PreConcat(SKMatrix.CreateRotation(angleDiff, point.X, point.Y));
    }

    public void RotateByRadiansDelta(SKPoint point, float radiansDelta)
    {
        Matrix.PreConcat(SKMatrix.CreateRotation(radiansDelta, point.X, point.Y));
    }

    public void Zoom(SKPoint point, float scale)
    {
        var currentScale = GetScale();
        var scaleFactor = scale / currentScale;
        Matrix.PreConcat(SKMatrix.CreateScale(scaleFactor, scaleFactor, point.X, point.Y));
    }

    public void ZoomByScaleFactor(SKPoint point, float scaleFactor)
    {
        var currentScale = GetScale();
        currentScale *= scaleFactor;
        if (currentScale < MinScale || currentScale > MaxScale)
        {
            return;
        }
        if (!CenterBoundary.IsEmpty)
        {
            var center = GetCenter();
            if (!CenterBoundary.Contains(center))
            {
                return;
            }
        }
        Matrix.PreConcat(SKMatrix.CreateScale(scaleFactor, scaleFactor, point.X, point.Y));
    }


    public SKPoint GetCanvasPointFromViewPoint(SKPoint viewPoint)
    {

        SKMatrix invertedMatrix;
        if (!Matrix.TryInvert(out invertedMatrix))
        {
            return SKPoint.Empty;
        }
        return invertedMatrix.MapPoint(viewPoint);
    }

    public SKPoint GetCenter()
    {
        return GetCanvasPointFromViewPoint(ScreenCenter);
    }

    /// <summary>
    /// Returns number from interval [-pi to pi]
    /// </summary>
    /// <returns></returns>
    public float GetAngleInRadians()
    {
        //https://stackoverflow.com/questions/4361242/extract-rotation-scale-values-from-2d-transformation-matrix
        var skewY = Matrix.SkewY;
        var scaleY = Matrix.ScaleY;
        var result = Math.Atan2(skewY, scaleY);
        return (float)result;
    }

    public float GetScale()
    {
        //https://stackoverflow.com/questions/4361242/extract-rotation-scale-values-from-2d-transformation-matrix
        var scaleX = Matrix.ScaleX;
        var skewY = Matrix.SkewY;
        var result = Math.Sqrt(scaleX * scaleX + skewY * skewY);
        return (float)result;
    }
}