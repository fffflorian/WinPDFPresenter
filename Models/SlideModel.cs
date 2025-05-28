namespace WinPDFPresenter.Models;

public class SlideModel {
	public string LogicalPageNumber  { get; set; } = "";
	public int    PhysicalPageNumber { get; set; } = 0;
	public int    OverlayNumber      { get; set; } = 0;
	public string ImagePath          { get; set; } = "";
	public string NoteMarkdownText   { get; set; } = "";
}