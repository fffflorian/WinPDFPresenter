using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using WinPDFPresenter.Models;
using WinPDFPresenter.ViewModels;

namespace WinPDFPresenter.Views;

public partial class LoadingConvertingPdfDialog : Window {

	private readonly PdfToImageViewModel _viewModel;
	
	public LoadingConvertingPdfDialog() {
		InitializeComponent();
		Width       = 400;
		Height      = 200;
		_viewModel  = new PdfToImageViewModel();
		DataContext = _viewModel;
		_viewModel.PropertyChanged += (_, e) => {
			if (e.PropertyName == nameof(PdfToImageViewModel.IsConverting) && !_viewModel.IsConverting) {
				Task.Delay(2000).ContinueWith(_ => {
					Dispatcher.UIThread.InvokeAsync(() => Close(_viewModel.PdfProperties));
				});
			}
		};
	}

	private void Button_OnClick(object? sender, RoutedEventArgs e) {
		_viewModel.Cancel();
	}

	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("Linux")]
	[SupportedOSPlatform("macOS")]
	[SupportedOSPlatform("iOS13.6")]
	[SupportedOSPlatform("MacCatalyst13.5")]
	[SupportedOSPlatform("Android31.0")]
	public async Task<PdfModelProperties?> StartConversion(string pdfPath, int dpi = 300, int width = 2560) {
		_ = Task.Run(async () => {
			try {
				await _viewModel.ConvertPdfToImageAsync(pdfPath, dpi, width);
			} catch (Exception ex) {
				await Dispatcher.UIThread.InvokeAsync(() => {
					_viewModel.StatusText   = $"Error: {ex.Message}";
					_viewModel.IsConverting = false;
				});
			}
		});
		await ShowDialog((Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)!);
		return _viewModel.PdfProperties;
	}
}

[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("macOS")]
[SupportedOSPlatform("iOS13.6")]
[SupportedOSPlatform("MacCatalyst13.5")]
[SupportedOSPlatform("Android31.0")]
public static class PdfConverter {
	public static async Task<PdfModelProperties?> ShowConverterAsync(string pdfPath, int dpi = 300, int width = 2560) {
		var window = new LoadingConvertingPdfDialog();
		return await window.StartConversion(pdfPath, dpi, width);
	}
}