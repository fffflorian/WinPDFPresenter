using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using ReactiveUI;
using WinPDFPresenter.Models;

namespace WinPDFPresenter.ViewModels;

public class PresentationViewModel(PdfModelProperties props, int? noteFontSize, int? duration, int? lastMinutes) : ViewModelBase {
	private                 string _currentSlideNoteText = "";
	private static readonly Uri    BlackImagePath        = new("avares://WinPDFPresenter/Assets/sXK51.png");
	private                 int    _currentPage          = 0;
	private                 IImage _currentImage         = new Bitmap(props.GetPathForPage(0)!);
	private                 int    _noteFontSize         = noteFontSize ?? 20;
	private                 IImage _previousImage        = new Bitmap(AssetLoader.Open(BlackImagePath));
	private                 IImage _nextImage            = new Bitmap(AssetLoader.Open(BlackImagePath));
	
	public int                Duration      { get; } = duration ?? 25;
	public int                LastMinutes   { get; } = lastMinutes ?? 5;
	public PdfModelProperties PdfProperties { get; } = props;
	public Timer              SlideTimer    { get; } = new(duration ?? 25, lastMinutes ?? 5);

	public int NoteFontSize {
		get => _noteFontSize;
		set => this.RaiseAndSetIfChanged(ref _noteFontSize, value);
	}
	public int CurrentPage {
		get => _currentPage;
		set => this.RaiseAndSetIfChanged(ref _currentPage, value);
	}
	public IImage CurrentImage {
		get => _currentImage;
		set => this.RaiseAndSetIfChanged(ref _currentImage, value);
	}
	public IImage PreviousImage {
		get => _previousImage;
		set => this.RaiseAndSetIfChanged(ref _previousImage, value);
	}
	public IImage NextImage {
		get => _nextImage;
		set => this.RaiseAndSetIfChanged(ref _nextImage, value);
	}
	public string CurrentSlideNoteText {
		get => _currentSlideNoteText;
		set => this.RaiseAndSetIfChanged(ref _currentSlideNoteText, value);
	}

	public PresentationViewModel() : this(new PdfModelProperties(), 20, 25, 5) { }
	
	public string CurrentSlideText =>
		$"Slide {PdfProperties.Slides[CurrentPage].LogicalPageNumber} of {PdfProperties.LogicalPageCount}";
	public string CurrentOverlayText => PdfProperties.GetOverlayText(CurrentPage) ?? "";
	
	public void LoadImages() {
		PreviousImage = (CurrentPage == 0)
			? new Bitmap(AssetLoader.Open(BlackImagePath))
			: new Bitmap(PdfProperties.Slides[CurrentPage - 1].ImagePath);
		CurrentImage  = new Bitmap(PdfProperties.Slides[CurrentPage].ImagePath);
		NextImage = (CurrentPage + 1 == PdfProperties.PageCount)
			? new Bitmap(AssetLoader.Open(BlackImagePath))
			: new Bitmap(PdfProperties.Slides[CurrentPage + 1].ImagePath);
	}

	public string GetCurrentNoteText() {
		return PdfProperties.Slides[CurrentPage].NoteMarkdownText;
	}

	public void NextPage() {
		if (CurrentPage + 1 >= PdfProperties.PageCount) return;
		CurrentPage++;
		LoadImages();
	}
	
	public void NextPage(string noteTextToUpdate) {
		if (CurrentPage + 1 >= PdfProperties.PageCount) return;
		PdfProperties.Slides[CurrentPage].NoteMarkdownText = noteTextToUpdate;
		CurrentPage++;
		LoadImages();
	}
	
	public void PreviousPage() {
		if (CurrentPage == 0) return;
		CurrentPage--;
		LoadImages();
	}

	public void PreviousPage(string noteTextToUpdate) {
		if (CurrentPage == 0) return;
		PdfProperties.Slides[CurrentPage].NoteMarkdownText = noteTextToUpdate;
		CurrentPage--;
		LoadImages();
	}
	
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