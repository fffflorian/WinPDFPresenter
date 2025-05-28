using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using ReactiveUI;

namespace WinPDFPresenter.ViewModels;

public class EditorWindowViewModel(MainWindowViewModel mwvm, IStorageFile? storageFile) : ViewModelBase {
	private string _currentSlideNoteText = "";
	private MainWindowViewModel _mwvm = mwvm;

	public MainWindowViewModel MainWindowViewModel {
		get => _mwvm;
		set => this.RaiseAndSetIfChanged(ref _mwvm, value);
	}
	public IStorageFile? StorageFile { get; } = storageFile;
	public string CurrentSlideNoteText {
		get => _currentSlideNoteText;
		set => this.RaiseAndSetIfChanged(ref _currentSlideNoteText, value);
	}

	public EditorWindowViewModel() : this(new MainWindowViewModel(), null) {}

	public static void CopyMouseCommand(TextArea textArea) {
		ApplicationCommands.Copy.Execute(null, textArea);
	}
	public static void CutMouseCommand(TextArea textArea) {
		ApplicationCommands.Cut.Execute(null, textArea);
	}
	public static void PasteMouseCommand(TextArea textArea) {
		ApplicationCommands.Paste.Execute(null, textArea);
	}
	public static void SelectAllMouseCommand(TextArea textArea) {
		ApplicationCommands.SelectAll.Execute(null, textArea);
	}
}