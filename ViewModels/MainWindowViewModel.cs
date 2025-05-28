using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using WinPDFPresenter.Models;

namespace WinPDFPresenter.ViewModels;

public partial class MainWindowViewModel(PdfModelProperties props) : ViewModelBase {
	public int                CurrentPage     { get; set; } = 0;
	public PdfModelProperties PdfProperties   { get; }      = props;
	public Uri                BlackImagePath  { get; }      = new ("avares://WinPDFPresenter/Assets/sXK51.png");
	public IImage?            CurrentImage    { get; set; } = new Bitmap(props.GetPathForPage(0)!);
	public IImage? PreviousImage { get; set; } =
		new Bitmap(AssetLoader.Open(new Uri("avares://WinPDFPresenter/Assets/sXK51.png")));
	public IImage? NextImage { get; set; } =
		new Bitmap(AssetLoader.Open(new Uri("avares://WinPDFPresenter/Assets/sXK51.png")));

	public MainWindowViewModel() : this(new PdfModelProperties { FileExtensionEnd = ".png", FileNameBeforeNumber = "doc_page_", PageCount = 3, OutputDirectory = ""}) { }

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
		Debug.WriteLine("NextPage Called");
		if (CurrentPage + 1 >= PdfProperties.PageCount) return;
		CurrentPage++;
		LoadImages();
	}
	
	public void NextPage(string noteTextToUpdate) {
		Debug.WriteLine("NextPageWithUpdates Called");
		if (CurrentPage + 1 >= PdfProperties.PageCount) return;
		PdfProperties.Slides[CurrentPage].NoteMarkdownText = noteTextToUpdate;
		CurrentPage++;
		LoadImages();
	}
	
	public void PreviousPage() {
		Debug.WriteLine("PreviousPage Called");
		if (CurrentPage == 0) return;
		CurrentPage--;
		LoadImages();
	}

	public void PreviousPage(string noteTextToUpdate) {
		Debug.WriteLine("PreviousPageWithUpdates Called");
		if (CurrentPage == 0) return;
		PdfProperties.Slides[CurrentPage].NoteMarkdownText = noteTextToUpdate;
		CurrentPage--;
		LoadImages();
	}
}