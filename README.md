# Maui.FreakyEffects

<div>
   <a href="https://www.nuget.org/packages/FreakyEffects"><img src="https://img.shields.io/nuget/v/FreakyEffects?color=blue&logo=nuget"></a>
   <a href="https://www.nuget.org/packages/FreakyEffects"><img src="https://img.shields.io/nuget/dt/FreakyEffects.svg"></a>
<a href="https://app.fossa.com/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects?ref=badge_shield" alt="FOSSA Status"><img src="https://app.fossa.com/api/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects.svg?type=shield"/></a>
   <a href="./LICENSE"><img src="https://img.shields.io/github/license/freakyali/maui.freakyeffects"></a>
   <a href="https://www.codefactor.io/repository/github/freakyali/maui.freakyeffects"><img src="https://www.codefactor.io/repository/github/freakyali/maui.freakyeffects/badge"></a>
</div>

Maui.FreakyEffects is a collection of lightweight, visually stunning, and unique effects for .NET MAUI. It contains a variety of effects that you can use to add creativity and polish to your applications across all platforms.

The code for each effect is well-documented and easy to understand, making it accessible for both experienced MAUI developers and those just starting out.

## Platforms

| Platform | Version | Status |
| --- | --- | --- |
| iOS | 15.0+ | Full support |
| Android | API 23+ (Marshmallow) | Full support |
| macOS Catalyst | 15.0+ | Full support |
| Windows | 10.0.17763+ (1809) | Full support |

## Effects

| Effect | Description | Platforms |
| --- | --- | --- |
| [Skeleton Effect](docs/skeleton-effect.md) | Animated loading placeholders with Beat, Fade, and Shake animations | All |
| [Touch Effects](docs/touch-effects.md) | Visual ripple/highlight on touch with configurable color and tap/long-tap commands | All |
| [Touch Tracking](docs/touch-tracking.md) | Low-level multi-touch point tracking with Pressed, Moved, Released, Entered, Exited, and Cancelled events | All |
| [SkiaScene](docs/skia-scene.md) | Hardware-accelerated 2D scene with pinch-to-zoom, pan, tap, and double-tap gesture recognition | All |

## Previews

| iOS | Android |
| --- | --- |
| <img src="https://user-images.githubusercontent.com/31090457/225077296-a87ef6ea-7ef4-4ab6-a904-960bcd13654b.gif" width="250" height="550"/>| <img src="https://user-images.githubusercontent.com/31090457/225077308-994f0265-463a-4109-bca1-8abc0cf634c4.gif" width="250" height="550"/> |
| <img src="https://github.com/FreakyAli/Maui.FreakyEffects/assets/31090457/72eb3dac-b716-47ff-be96-73341ca2b1ac" width="250" height="550"/>| <img src="https://github.com/FreakyAli/Maui.FreakyEffects/assets/31090457/691abb5b-42cf-4619-9c0d-91f76d814d16" width="250" height="550"/> |

Skeleton Effect's Profile Design Inspired from [here](https://www.behance.net/gallery/155366855/Daily-UI-Day-6-User-Profile?tracking_source=search_projects%7Cuser+profile)

## Installation

Add our [NuGet](https://www.nuget.org/packages/FreakyEffects) package or run the following command in your .NET MAUI project:

```
Install-Package FreakyEffects -Version xx.xx.xx
```

Then register the effects in your `MauiProgram.cs`:

```csharp
builder
    .UseMauiApp<App>()
    .ConfigureEffects(effects =>
    {
        effects.InitFreakyEffects();
    });
```

## Documentation

Full documentation lives in the [docs/](docs/) folder:

- [Getting Started](docs/getting-started.md)
- [Skeleton Effect](docs/skeleton-effect.md)
- [Touch Effects](docs/touch-effects.md)
- [Touch Tracking](docs/touch-tracking.md)
- [SkiaScene](docs/skia-scene.md)

You can also check the [Wiki](https://github.com/FreakyAli/Maui.FreakyEffects/wiki) for additional information.

### Like what you saw? Want to keep this repo alive?
[![](https://miro.medium.com/max/600/0*wrBJU05A3BULKcWA.gif)](https://www.buymeacoffee.com/FreakyAli)

## License

The license for this project can be found [here](https://github.com/FreakyAli/Maui.FreakyEffects/blob/master/LICENSE)

## Activity

Fossa:

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects?ref=badge_large)

Sparkline:

[![Sparkline](https://stars.medv.io/FreakyAli/Maui.FreakyEffects.svg)](https://stars.medv.io/FreakyAli/Maui.FreakyEffects)

RepoBeats:

![RepoBeats](https://repobeats.axiom.co/api/embed/d14386d457d5790da527b4b00dfddcd8dad8e2dc.svg "Repobeats analytics image")
