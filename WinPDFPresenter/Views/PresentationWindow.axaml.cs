using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace WinPDFPresenter.Views;

public partial class PresentationWindow : Window {
	
	private double _oldWidth, _oldHeight;
	private bool _isFullScreen;
	
	public PresentationWindow() {
		InitializeComponent();
		KeyDown += (_, args) => {
			if (args.Key == Key.F11) ToggleFullScreen();
		};
	}

	public void SetImageShown(IImage image) {
		PdfViewer.Source = image;
	}

	public void ToggleFullScreen() {
		if (_isFullScreen) {
			Width             = _oldWidth;
			Height            = _oldHeight;
			WindowState       = WindowState.Normal;
			SystemDecorations = SystemDecorations.Full;
			Topmost           = false;
		} else {
			_oldWidth         = Width;
			_oldHeight        = Height;
			WindowState       = WindowState.FullScreen;
			SystemDecorations = SystemDecorations.None;
			Topmost           = true;
		}
		_isFullScreen = !_isFullScreen;
	}
}