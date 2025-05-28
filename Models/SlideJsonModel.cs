namespace WinPDFPresenter.Models;

public partial class SlideJsonModel {
	/// <summary>
	/// 0-based PDF page index
	/// </summary>
	[Newtonsoft.Json.JsonProperty("idx", Required = Newtonsoft.Json.Required.Always)]
	[System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
	public int Idx { get; set; }

	/// <summary>
	/// Logical Page Number
	/// </summary>
	[Newtonsoft.Json.JsonProperty("label", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string Label { get; set; }

	/// <summary>
	/// Overlay index having the same label, 0-based
	/// </summary>
	[Newtonsoft.Json.JsonProperty("overlay", Required = Newtonsoft.Json.Required.Always)]
	[System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
	public int Overlay { get; set; }

	/// <summary>
	/// Flag indicating the page should normally be skipped
	/// </summary>
	[Newtonsoft.Json.JsonProperty("hidden", Required = Newtonsoft.Json.Required.DisallowNull,
		NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public bool Hidden { get; set; } = false;

	/// <summary>
	/// User notes in Markdown
	/// </summary>
	[Newtonsoft.Json.JsonProperty("note", Required = Newtonsoft.Json.Required.AllowNull)]
	[System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
	public string? Note { get; set; } = "";
}
