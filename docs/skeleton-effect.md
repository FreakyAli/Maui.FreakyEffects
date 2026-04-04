# Skeleton Effect

The Skeleton Effect replaces content with animated placeholder shapes while data is loading, giving users immediate visual feedback instead of a blank screen.

## Platform Support

All platforms — iOS, Android, macOS Catalyst, Windows. The effect is implemented entirely in shared MAUI code with no platform-specific code required.

## XAML Namespace

```xml
xmlns:skeleton="clr-namespace:Maui.FreakyEffects.Skeleton;assembly=Maui.FreakyEffects"
```

## Attached Properties

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `SkeletonEffect.IsBusy` | `bool` | `false` | Activates or deactivates the skeleton on this view |
| `SkeletonEffect.IsParent` | `bool` | `false` | Marks a container as a parent — children will be hidden rather than individually skeletonised |
| `SkeletonEffect.Hide` | `bool` | `false` | Hides the view entirely instead of showing a skeleton |
| `SkeletonEffect.BackgroundColor` | `Color` | `default` | The color of the skeleton placeholder. Falls back to the view's existing color if not set |
| `SkeletonEffect.Animation` | `BaseAnimation` | `null` | The animation to run while loading. See [Animations](#animations) below |

## Animations

Set `SkeletonEffect.Animation` to one of the built-in animation types or omit it for no animation.

| Animation | Description | Key defaults |
| --- | --- | --- |
| `BeatAnimation` | Pulsing scale effect | Scale: 1.03, Interval: 750 ms |
| `FadeAnimation` | Opacity fade in/out | Opacity: 0.6, Interval: 500 ms |
| `VerticalShakeAnimation` | Shakes the view up and down | — |
| `HorizontalShakeAnimation` | Shakes the view left and right | — |

## Basic Usage

Apply `IsBusy` and `Animation` to any view. When `IsBusy` is `true` the view turns into a skeleton; when `false` the original content is restored.

```xml
<Label
    Text="User Name"
    skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
    skeleton:SkeletonEffect.BackgroundColor="#E0E0E0">
    <skeleton:SkeletonEffect.Animation>
        <skeleton:FadeAnimation />
    </skeleton:SkeletonEffect.Animation>
</Label>
```

## Animating a Layout

Mark the parent with `IsParent="True"` so the container itself gets the skeleton colour while its children are hidden. Child views then have `IsBusy` set individually.

```xml
<Grid
    skeleton:SkeletonEffect.IsParent="True"
    skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
    skeleton:SkeletonEffect.BackgroundColor="#E0E0E0">
    <Grid.RowDefinitions>
        <RowDefinition Height="60" />
        <RowDefinition Height="20" />
        <RowDefinition Height="20" />
    </Grid.RowDefinitions>

    <!-- Avatar -->
    <BoxView
        Grid.Row="0"
        CornerRadius="30"
        skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
        skeleton:SkeletonEffect.BackgroundColor="#D0D0D0">
        <skeleton:SkeletonEffect.Animation>
            <skeleton:BeatAnimation />
        </skeleton:SkeletonEffect.Animation>
    </BoxView>

    <!-- Name line -->
    <BoxView
        Grid.Row="1"
        skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
        skeleton:SkeletonEffect.BackgroundColor="#D0D0D0" />

    <!-- Subtitle line -->
    <BoxView
        Grid.Row="2"
        skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
        skeleton:SkeletonEffect.BackgroundColor="#D0D0D0" />
</Grid>
```

## Hiding Instead of Skeletonising

Use `Hide="True"` to make a view invisible while loading instead of showing a placeholder:

```xml
<Image
    Source="avatar.png"
    skeleton:SkeletonEffect.IsBusy="{Binding IsLoading}"
    skeleton:SkeletonEffect.Hide="True" />
```

## Controlling from Code-Behind

```csharp
SkeletonEffect.SetIsBusy(myLabel, true);
SkeletonEffect.SetAnimation(myLabel, new FadeAnimation());
SkeletonEffect.SetBackgroundColor(myLabel, Color.FromArgb("#E0E0E0"));

// Later, when data is ready:
SkeletonEffect.SetIsBusy(myLabel, false);
```

## Notes

- `BackgroundColor` overrides the view's existing background color while `IsBusy` is `true` and restores it when `false`.
- For `Label` and `Button`, the text color is set to `Transparent` during the skeleton state to hide the text content.
- Dynamic resources (e.g. `{DynamicResource MyColor}`) are detected automatically and restored correctly.
