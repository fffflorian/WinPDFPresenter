using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using Newtonsoft.Json;
using TextMateSharp.Grammars;
using WinPDFPresenter.Extensions;
using WinPDFPresenter.Models;
using WinPDFPresenter.ViewModels;

namespace WinPDFPresenter.Views;

public partial class EditorWindow : Window {
	
	private readonly PresentationViewModel? _viewModel;
	private readonly TextEditor?            _textEditor;
	private readonly TextMate.Installation? _textMateInstallation;
	private const    int                    CurrentTheme = (int)ThemeName.DarkPlus;

	public EditorWindow() {
		// This is a dummy constructor; never use this!!!
		// Only used to fix warning AVLN3001.
	}

	public EditorWindow(PresentationViewModel viewModel, ICollection<SlideJsonModel>? notes = null) {
		InitializeComponent();
		#if DEBUG
		this.AttachDevTools();
		#endif
		DataContext = viewModel;
		_viewModel  = viewModel;
		var registryOptions = new RegistryOptions((ThemeName)CurrentTheme);
		if (notes != null) {
			for (var i = 0; i < _viewModel.PdfProperties.Slides.Count && i < notes.Count; i++) {
				_viewModel.PdfProperties.Slides[i].LogicalPageNumber = notes.ElementAt(i).Label;
				_viewModel.PdfProperties.Slides[i].NoteMarkdownText  = notes.ElementAt(i).Note ?? "";
			}
		}
		_textEditor                                     =  this.FindControl<TextEditor>("NotesEditor")!;
		_textEditor.Options.AllowToggleOverstrikeMode   =  true;
		_textEditor.Options.EnableTextDragDrop          =  true;
		_textEditor.Options.HighlightCurrentLine        =  true;
		_textEditor.Options.CompletionAcceptAction      =  CompletionAcceptAction.DoubleTapped;
		_textEditor.Options.ShowBoxForControlCharacters =  true;
		_textEditor.Options.ColumnRulerPositions        =  [80, 100];
		_textEditor.TextArea.Background                 =  NotesEditor.Background;
		_textEditor.TextArea.RightClickMovesCaret       =  true;
		_textEditor.TextArea.IndentationStrategy        =  new CSharpIndentationStrategy(_textEditor.Options);
		_textMateInstallation                           =  _textEditor.InstallTextMate(registryOptions);
		_textMateInstallation.AppliedTheme              += TextMateInstallationOnAppliedTheme;
		_textMateInstallation.SetGrammar(registryOptions.GetScopeByExtension(".md"));
		RefreshImages();
	}

	#region HelperFunctionsSettingThemes
	private static bool ApplyBrushAction(TextMate.Installation installation, string colourNameFromJson,
	                                     Action<IBrush>        applyColourAction) {
		if (!installation.TryGetThemeColor(colourNameFromJson, out var colourString)) return false;
		if (!Color.TryParse(colourString, out var colour)) return false;
		var colourBrush = new SolidColorBrush(colour);
		applyColourAction(colourBrush);
		return true;
	}

	private void ApplyThemeColoursToWindow(TextMate.Installation installation) {
		ApplyBrushAction(installation, "NotesEditor.background", brush => Background = brush);
		ApplyBrushAction(installation, "NotesEditor.foreground", brush => Foreground = brush);
	}

	private void ApplyThemeColoursToEditor(TextMate.Installation installation) {
		ApplyBrushAction(installation, "NotesEditor.background", brush => Background = brush);
		ApplyBrushAction(installation, "NotesEditor.foreground", brush => Foreground = brush);

		if (!ApplyBrushAction(installation, "NotesEditor.selectionBackground",
			    brush => _textEditor!.TextArea.SelectionBrush = brush) &&
		    Application.Current!.TryGetResource("TextAreaSelectionBrush", out var resourceObject) &&
		    resourceObject is IBrush brush1) {
			_textEditor!.TextArea.SelectionBrush = brush1;
		}

		if (!ApplyBrushAction(installation, "NotesEditor.lineHighlightBackground",
			    brush => {
				    _textEditor!.TextArea.TextView.CurrentLineBackground = brush;
				    _textEditor.TextArea.TextView.CurrentLineBorder     = new Pen(brush);
			    })) {
			_textEditor!.TextArea.TextView.SetDefaultHighlightLineColors();
		}
	}
	#endregion
	
	private void TextMateInstallationOnAppliedTheme(object? sender, TextMate.Installation e) {
		ApplyThemeColoursToEditor(e);
		ApplyThemeColoursToWindow(e);
	}
	
	protected override void OnClosed(EventArgs e) {
		base.OnClosed(e);
		_textMateInstallation!.Dispose();
	}

	private void RefreshImages() {
		_viewModel!.LoadImages();
		PdfPages.Text = _viewModel.CurrentSlideText + "\n" +
		                _viewModel.CurrentOverlayText;
		PdfPreview.Source      = _viewModel.CurrentImage;
		PdfPreviousPage.Source = _viewModel.PreviousImage;
		PdfNextPage.Source     = _viewModel.NextImage;
	}
	
	private void PreviousSlideButton_OnClick(object? sender, RoutedEventArgs e) {
		_viewModel!.PreviousPage(_viewModel.CurrentSlideNoteText);
		_viewModel.CurrentSlideNoteText = _viewModel.GetCurrentNoteText();
		RefreshImages();
	}

	private void NextSlideButton_OnClick(object? sender, RoutedEventArgs e) {
		_viewModel!.NextPage(_viewModel.CurrentSlideNoteText);
		_viewModel.CurrentSlideNoteText = _viewModel.GetCurrentNoteText();
		RefreshImages();
	}

	private async void SaveNotes(object? sender, RoutedEventArgs e) {
		List<SlideJsonModel> notes = [];
		notes.AddRange(_viewModel!.PdfProperties.Slides.Select(slide => new SlideJsonModel {
			Hidden  = false,
			Idx     = slide.PhysicalPageNumber,
			Label   = slide.LogicalPageNumber,
			Note    = slide.NoteMarkdownText,
			Overlay = slide.OverlayNumber
		}));
		var window = new SaveNoteFileDialog(new WinPdfPresenter {
			Duration     = _viewModel.Duration,
			LastMinutes  = _viewModel.LastMinutes,
			NoteFontSize = _viewModel.NoteFontSize,
			Slides       = notes
		});
		await window.ShowDialog(this);
	}
}