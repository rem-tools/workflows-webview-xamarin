# Workflows Webview Xamarin

## Permissions required

- Camera
- Audio
- Geolocation

```csharp
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.RecordAudio)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
```

## Packages Needed

- Xamarin.Forms 5.x.x
- Xamarin.Essentials 1.7.x
- NewtonSoft.JSON 13.x.x