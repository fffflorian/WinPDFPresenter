using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Newtonsoft.Json;
using WinPDFPresenter.Models;
using WinPDFPresenter.ViewModels;

namespace WinPDFPresenter.Views;

public partial class SaveNoteFileDialog : Window {
	
	private readonly SaveNoteFileViewModel _viewModel;

	public SaveNoteFileDialog() : this(new WinPdfPresenter
		{ Duration = 25, NoteFontSize = 20, LastMinutes = 5, Slides = [] }) {
		// This is a dummy constructor; never use this!!!
		// Only used to fix warning AVLN3001.
	}

	public SaveNoteFileDialog(WinPdfPresenter winPdfPresenter) {
		InitializeComponent();
		Width                  = 500;
		Height                 = 450;
		_viewModel             = new SaveNoteFileViewModel(winPdfPresenter);
		DataContext            = _viewModel;
		InputDuration.Value    = winPdfPresenter.Duration;
		InputFontSize.Value    = winPdfPresenter.NoteFontSize;
		InputLastMinutes.Value = winPdfPresenter.LastMinutes;
	}

	private async void SaveFile(object? sender, RoutedEventArgs e) {
		_viewModel.NoteFileContent.Duration = (int)(InputDuration.Value ?? 25);
		_viewModel.NoteFileContent.LastMinutes = (int)(InputLastMinutes.Value ?? 5);
		_viewModel.NoteFileContent.NoteFontSize = (int)(InputFontSize.Value ?? 18);
		var topLevel = GetTopLevel(this);
		var file     = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
			Title = "Save .pdfnotes file",
			DefaultExtension = ".pdfnotes",
			ShowOverwritePrompt = true,
			FileTypeChoices = [ new FilePickerFileType(".pdfnotes") {
				Patterns                    = [ "*.pdfnotes" ],
				AppleUniformTypeIdentifiers = null,
				MimeTypes                   = [ "application/json" ]
			} ]
		});
		if (file is null) return;
		await using var stream       = await file.OpenWriteAsync();
		await using var streamWriter = new StreamWriter(stream);
		var             json         = JsonConvert.SerializeObject(_viewModel.NoteFileContent, Formatting.Indented);
		await streamWriter.WriteAsync(json);
		Close();
	}
}