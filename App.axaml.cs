using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Runtime.Versioning;
using Avalonia.Markup.Xaml;
using WinPDFPresenter.ViewModels;
using WinPDFPresenter.Views;

namespace WinPDFPresenter;

[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("macOS")]
[SupportedOSPlatform("iOS13.6")]
[SupportedOSPlatform("MacCatalyst13.5")]
[SupportedOSPlatform("Android31.0")]
public partial class App : Application {
	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			// Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();
			//desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel(), };
			desktop.MainWindow = new StartSelection { DataContext = new StartSelectionViewModel(), };
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation() {
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove) {
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}