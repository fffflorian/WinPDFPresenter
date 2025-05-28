using Avalonia.Controls;
using Avalonia.Media;

namespace WinPDFPresenter.Views;

public partial class PresentationWindow : Window {
	public PresentationWindow() {
		InitializeComponent();
	}

	public void SetImageShown(IImage image) {
		PdfViewer.Source = image;
	}
}