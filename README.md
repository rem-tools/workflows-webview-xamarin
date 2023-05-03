# Workflows Webview Xamarin

Este repositorio contiene un proyecto de ejemplo de Xamarin que demuestra cómo integrar nuestra plataforma de Workflows en una aplicación móvil utilizando un WebView. La aplicación muestra cómo configurar adecuadamente un WebView para trabajar con la plataforma de Workflows, así como cómo manejar eventos enviados desde el WebView.

Características principales:

- Configuración del WebView: La aplicación muestra cómo configurar correctamente un WebView en Xamarin para interactuar con nuestra plataforma de Workflows.

- Manejo de eventos: La aplicación maneja dos eventos específicos enviados desde el WebView. Estos eventos son procesados y respondidos adecuadamente por la aplicación de ejemplo.

- Integración con la plataforma de Workflows: La aplicación carga e interactúa con nuestra plataforma de Workflows a través del WebView, proporcionando una experiencia de usuario fluida y consistente.

Este proyecto de ejemplo es un recurso valioso para los desarrolladores que buscan integrar nuestra plataforma de Workflows en sus aplicaciones Xamarin. Puedes utilizar este repositorio como punto de partida para tu propia integración o como una referencia para comprender mejor cómo interactuar con nuestra plataforma de Workflows desde un WebView en Xamarin.

## Permisos requeridos

- Camera
- Audio
- Geolocation

```csharp
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.RecordAudio)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
```

## Paquetes necesarios

- Xamarin.Forms 5.x.x
- Xamarin.Essentials 1.7.x
- NewtonSoft.JSON 13.x.x