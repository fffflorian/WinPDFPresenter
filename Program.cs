using Avalonia;
using System;
using System.Runtime.Versioning;
using Avalonia.Controls;
using Avalonia.ReactiveUI;

namespace WinPDFPresenter;

[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("macOS")]
[SupportedOSPlatform("iOS13.6")]
[SupportedOSPlatform("MacCatalyst13.5")]
[SupportedOSPlatform("Android31.0")]
sealed class Program {
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => BuildAvaloniaApp()
		.StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
		             .UsePlatformDetect()
		             .UseReactiveUI()
		             .WithInterFont()
		             .LogToTrace();
}