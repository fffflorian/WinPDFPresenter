namespace WinPDFPresenter.Models;

/// <summary>
/// Notes for PDF-presentations, to be used with WinPDFPresenter.
/// </summary>
public partial class WinPdfPresenter {
	/// <summary>
	/// Duration of the presentation, in minutes
	/// </summary>
	[Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	[System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
	public int Duration { get; set; }

	/// <summary>
	/// Number of last minutes when the timer color changes
	/// </summary>
	[Newtonsoft.Json.JsonProperty("lastMinutes", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	[System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
	public int LastMinutes { get; set; } = 5;

	/// <summary>
	/// Font size used to display slide notes
	/// </summary>
	[Newtonsoft.Json.JsonProperty("noteFontSize", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	[System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
	public int NoteFontSize { get; set; } = 20;

	/// <summary>
	/// Array of pages; only user-modified ones need to be listed
	/// </summary>
	[Newtonsoft.Json.JsonProperty("slides", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public System.Collections.Generic.ICollection<SlideJsonModel> Slides { get; set; } = [];
}