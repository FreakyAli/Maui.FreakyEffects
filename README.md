<div align="center">

# Maui.FreakyEffects

**A lightweight, open-source effects library for .NET MAUI**

<a href="https://www.nuget.org/packages/FreakyEffects"><img src="https://img.shields.io/nuget/v/FreakyEffects?color=blue&logo=nuget&style=for-the-badge" alt="FreakyEffects NuGet version"></a>
<a href="https://www.nuget.org/packages/FreakyEffects"><img src="https://img.shields.io/nuget/dt/FreakyEffects?style=for-the-badge" alt="FreakyEffects download count"></a>
<a href="./LICENSE"><img src="https://img.shields.io/github/license/freakyali/maui.freakyeffects?style=for-the-badge" alt="Repository license"></a>
<a href="https://www.codefactor.io/repository/github/freakyali/maui.freakyeffects"><img src="https://img.shields.io/codefactor/grade/github/freakyali/maui.freakyeffects?style=for-the-badge" alt="CodeFactor grade"></a>

<br/>

| iOS | macOS | Android | Windows |
| :---: | :---: | :---: | :---: |
| 15.0+ | 15.0+ | API 23+ | 10.0.17763+ |

<br/>

</div>

---

## Previews

> GIFs do not represent actual performance — clone the repo and run the Samples app for a real feel.

<div align="center">

| iOS | Android |
| :---: | :---: |
| <img src="https://user-images.githubusercontent.com/31090457/225077296-a87ef6ea-7ef4-4ab6-a904-960bcd13654b.gif" width="220" alt="Touch ripple effect demo on iOS"/> | <img src="https://user-images.githubusercontent.com/31090457/225077308-994f0265-463a-4109-bca1-8abc0cf634c4.gif" width="220" alt="Touch ripple effect demo on Android"/> |
| <img src="https://github.com/FreakyAli/Maui.FreakyEffects/assets/31090457/72eb3dac-b716-47ff-be96-73341ca2b1ac" width="220" alt="Skeleton loading effect demo on iOS"/> | <img src="https://github.com/FreakyAli/Maui.FreakyEffects/assets/31090457/691abb5b-42cf-4619-9c0d-91f76d814d16" width="220" alt="Skeleton loading effect demo on Android"/> |

</div>

Skeleton Effect's Profile Design inspired from the [Daily UI Day 6 — User Profile Behance project](https://www.behance.net/gallery/155366855/Daily-UI-Day-6-User-Profile?tracking_source=search_projects%7Cuser+profile).

---

## Installation

```
dotnet add package FreakyEffects
```

Or via Package Manager Console:

```powershell
Install-Package FreakyEffects -Version xx.xx.xx
```

### Initialization

In your `MauiProgram.cs`:

```csharp
using Maui.FreakyEffects;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureEffects(effects =>
            {
                // Registers TouchEffect, TouchTracking and Commands routing effects
                effects.InitFreakyEffects();
            });

        return builder.Build();
    }
}
```

---

## Documentation

Full docs for every effect live in the [`docs/`](./docs/) folder.

| Page | Description |
| --- | --- |
| [Getting Started](docs/getting-started.md) | Installation, registration, XAML namespaces |
| [Skeleton Effect](docs/skeleton-effect.md) | Animated loading placeholders |
| [Touch Effects](docs/touch-effects.md) | Visual ripple/highlight and tap/long-tap commands |
| [Touch Tracking](docs/touch-tracking.md) | Low-level multi-touch point tracking |
| [SkiaScene](docs/skia-scene.md) | Gesture-driven 2D canvas with pinch, pan and rotation |

---

### Like what you saw? Want to keep this repo alive?

<div align="center">

[![Buy Me A Coffee](https://miro.medium.com/max/600/0*wrBJU05A3BULKcWA.gif)](https://www.buymeacoffee.com/FreakyAli)

</div>

---

## License

[MIT](https://github.com/FreakyAli/Maui.FreakyEffects/blob/master/LICENSE)

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyEffects?ref=badge_large)

---

## Activity

<div align="center">

[![Star History Chart](https://api.star-history.com/svg?repos=FreakyAli/Maui.FreakyEffects&type=Date)](https://star-history.com/#FreakyAli/Maui.FreakyEffects&type=Date)

![RepoBeats](https://repobeats.axiom.co/api/embed/d14386d457d5790da527b4b00dfddcd8dad8e2dc.svg "Repobeats analytics image")

</div>
