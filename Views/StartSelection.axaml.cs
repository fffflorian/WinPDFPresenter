using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Newtonsoft.Json;
using WinPDFPresenter.Models;
using WinPDFPresenter.ViewModels;

namespace WinPDFPresenter.Views;

[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("macOS")]
[SupportedOSPlatform("iOS13.6")]
[SupportedOSPlatform("MacCatalyst13.5")]
[SupportedOSPlatform("Android31.0")]
public partial class StartSelection : Window {
	
	private readonly StartSelectionViewModel _viewModel;
	
	public StartSelection() {
		InitializeComponent();
		Height = 720;
		Width  = 480;
		_viewModel = new StartSelectionViewModel();
		DataContext = _viewModel;
	}

	private async void PdfFileSelectorButton_OnClick(object? sender, RoutedEventArgs eventArgs) {
		var topLevel = GetTopLevel(this);
		if (topLevel == null) return;
		var files    = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
			Title          = "Select a PDF file.",
			AllowMultiple  = false,
			FileTypeFilter = [FilePickerFileTypes.Pdf]
		});
		if (files.Count < 1) return;
		var pdf = files[0];
		Debug.WriteLine($"PDF file selected: {pdf} at {pdf.Path}:\n\t{pdf.Path.AbsolutePath} vs. {pdf.Path.LocalPath}");
		var conversionDirectory = await PdfConverter.ShowConverterAsync(pdf.Path.AbsolutePath, _viewModel.Dpi, _viewModel.Width);
		Debug.WriteLine($"Conversion directory given with {conversionDirectory!.OutputDirectory}.");
		WinPdfPresenter? presenter = null;
		if (!SelectionNewFileEdit.IsChecked ?? true) {
			var notes = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title          = "Select the notes file (.pdfnotes).",
				AllowMultiple = false,
				FileTypeFilter = [ new FilePickerFileType("Only Notes") {
					Patterns = [ "*.pdfnotes" ],
					AppleUniformTypeIdentifiers = null,
					MimeTypes = [ "application/json" ]
				} ]
			});
			if (notes.Count < 1) return;
			var       notesfile  = notes[0];
			var       jsonStream = await notesfile.OpenReadAsync();
			using var jsonReader = new StreamReader(jsonStream);
			var       json       = await jsonReader.ReadToEndAsync();
			presenter = JsonConvert.DeserializeObject<WinPdfPresenter>(json);
		}
		if (SelectionEdit.IsChecked ?? false) {
			var mainWindow =
				new EditorWindow(
					new PresentationViewModel(conversionDirectory!, presenter?.NoteFontSize, presenter?.Duration,
						presenter?.LastMinutes), presenter?.Slides);
			mainWindow.Show();
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
				desktop.MainWindow = mainWindow;
			}
		} else {
			var mainWindow =
				new MainWindow(new PresentationViewModel(conversionDirectory!, presenter?.NoteFontSize, presenter?.Duration,
					presenter?.LastMinutes), presenter?.Slides);
			mainWindow.Show();
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
				desktop.MainWindow = mainWindow;
			}
		}
		Close();
	}
}