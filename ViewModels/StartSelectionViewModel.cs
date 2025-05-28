using ReactiveUI;

namespace WinPDFPresenter.ViewModels;

public partial class StartSelectionViewModel : ViewModelBase {
	private int _dpi;
	public int Dpi { get => _dpi; set => this.RaiseAndSetIfChanged(ref _dpi, value); }

	private int _width = 2560;
	public int Width { get => _width; set => this.RaiseAndSetIfChanged(ref _width, value); }
}