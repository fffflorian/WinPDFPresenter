using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using WinPDFPresenter.Models;
using WinPDFPresenter.ViewModels;

namespace WinPDFPresenter.Views;

public partial class MainWindow : Window {
	
	private readonly PresentationViewModel? _viewModel;
	private readonly PresentationWindow?  _presentationWindow;
	private bool _isDark = false;
	private readonly string _isDarkString = "(currently black)";

	public MainWindow() {
		// This is a dummy constructor; never use this!!!
		// Only used to fix warning AVLN3001.
	}

	public MainWindow(PresentationViewModel viewModel, ICollection<SlideJsonModel>? notes = null) {
		InitializeComponent();
		#if DEBUG
		this.AttachDevTools();
		#endif
		_presentationWindow =  new PresentationWindow();
		_presentationWindow.Show();
		_presentationWindow.SetImageShown(viewModel.CurrentImage!);
		PdfPages.Text =  viewModel.CurrentSlideText + "\n" +
		                 viewModel.CurrentOverlayText;
		DataContext   =  viewModel;
		_viewModel    =  viewModel;
		if (notes != null) {
			for (var i = 0; i < _viewModel.PdfProperties.Slides.Count && i < notes.Count; i++) {
				_viewModel.PdfProperties.Slides[i].LogicalPageNumber = notes.ElementAt(i).Label;
				_viewModel.PdfProperties.Slides[i].NoteMarkdownText  = notes.ElementAt(i).Note ?? "";
			}
		}
		RefreshSlides();
		KeyDown       += MainOnKeyDown;
	}

	private void RefreshSlides() {
		_viewModel!.LoadImages();
		if (!_isDark) _presentationWindow!.SetImageShown(_viewModel.CurrentImage!);
		PdfPages.Text                   = _viewModel.CurrentSlideText + "\n" +
		                                  _viewModel.CurrentOverlayText + (_isDark ? "\n" + _isDarkString : "");
		PdfPreview.Source               = _viewModel.CurrentImage;
		PdfPreviousPage.Source          = _viewModel.PreviousImage;
		PdfNextPage.Source              = _viewModel.NextImage;
		_viewModel.CurrentSlideNoteText = _viewModel.GetCurrentNoteText();
	}

	private void ToggleDark() {
		_isDark = !_isDark;
		_presentationWindow!.SetImageShown(_isDark ? PresentationViewModel.BlackImage : _viewModel!.CurrentImage!);
		PdfPages.Text = _viewModel!.CurrentSlideText + "\n" +
		                _viewModel.CurrentOverlayText + (_isDark ? "\n" + _isDarkString : "");
	}

	private void MainOnKeyDown(object? sender, KeyEventArgs e) {
		switch (e.Key) {
			case Key.F11:
				_presentationWindow!.ToggleFullScreen();
				break;
			case Key.Right:
			case Key.PageDown:
			case Key.D:
				_viewModel!.NextPage();
				RefreshSlides();
				break;
			case Key.Left:
			case Key.PageUp:
			case Key.A:
				_viewModel!.PreviousPage();
				RefreshSlides();
				break;
			case Key.B:
				ToggleDark();
				break;
			case Key.P when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
				_viewModel!.SlideTimer.PauseTimer();
				break;
			case Key.R when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
				_viewModel!.SlideTimer.RestartTimer();
				break;
			case Key.S when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
				_viewModel!.SlideTimer.StartTimer();
				break;
			case Key.F5:
				if (!_presentationWindow!.IsFullScreen()) _presentationWindow.ToggleFullScreen();
				_viewModel!.SlideTimer.StartTimer();
				break;
			default: break;
		}
	}

}