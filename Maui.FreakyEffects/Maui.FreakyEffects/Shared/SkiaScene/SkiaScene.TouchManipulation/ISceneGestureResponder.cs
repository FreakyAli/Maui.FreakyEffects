namespace Maui.FreakyEffects.SkiaScene.TouchManipulation;

public interface ISceneGestureResponder
{
    TouchManipulationMode TouchManipulationMode { get; set; }

    void StartResponding();

    void StopResponding();
}

public delegate void PanEventHandler(object sender, PanEventArgs args);

public delegate void PinchEventHandler(object sender, PinchEventArgs args);

public delegate void TapEventHandler(object sender, TapEventArgs args);