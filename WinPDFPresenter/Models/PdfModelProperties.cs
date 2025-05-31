using System.Collections.Generic;
using System.IO;

namespace WinPDFPresenter.Models;

public class PdfModelProperties {
	public string           OutputDirectory      { get; init; } = "";
	public int              PageCount            { get; set; }
	public int              LogicalPageCount     { get; set; } 
	public string           FileNameBeforeNumber { get; init; } = "";
	public string           FileExtensionEnd     { get; init; } = "";
	public List<SlideModel> Slides               { get; }       = [];

	public string? GetFileNameFromPage(int pageNumber) {
		if (pageNumber < 0 || pageNumber > PageCount) return null;
		return $"{FileNameBeforeNumber}{(pageNumber + 1):000}{FileExtensionEnd}";
	}

	public string? GetPathForPage(int pageNumber) {
		if (pageNumber < 0 || pageNumber > PageCount) return null;
		return Path.Combine(OutputDirectory, GetFileNameFromPage(pageNumber)!);	
	}

	public string? GetOverlayText(int pageNumber) {
		if (pageNumber < 0 || pageNumber > PageCount) return null;
		var maxOverlay = -1;
		for (var i = pageNumber; i < PageCount; i++) {
			if (maxOverlay < Slides[i].OverlayNumber)
				maxOverlay = Slides[i].OverlayNumber;
			else
				break;
		}
		return $"{Slides[pageNumber].OverlayNumber + 1} / {(maxOverlay + 1)}";
	}
}