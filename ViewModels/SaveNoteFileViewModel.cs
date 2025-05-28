using ReactiveUI;
using WinPDFPresenter.Models;

namespace WinPDFPresenter.ViewModels;

public class SaveNoteFileViewModel(WinPdfPresenter nfc) : ViewModelBase {
	private WinPdfPresenter _noteFileContent = nfc;
	public WinPdfPresenter NoteFileContent {
		get => _noteFileContent;
		set => this.RaiseAndSetIfChanged(ref _noteFileContent, value);
	}

	public SaveNoteFileViewModel() : this(new WinPdfPresenter( )) {}
}