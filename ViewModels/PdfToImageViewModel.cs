using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PDFtoImage;
using SkiaSharp;
using WinPDFPresenter.Models;

namespace WinPDFPresenter.ViewModels;

public class PdfToImageViewModel : INotifyPropertyChanged {

	private int                     _currentPage, _totalPages;
	private string                  _statusText         = "Initialising ...";
	private bool                    _isConverting       = true;
	private CancellationTokenSource _cancellationSource = new();
	
	public int CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }
	public int TotalPages { get => _totalPages; set => SetProperty(ref _totalPages, value); }
	public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
	public bool IsConverting { get => _isConverting; set => SetProperty(ref _isConverting, value); }
	
	public double              ProgressPercentage => TotalPages > 0 ? (double)CurrentPage / TotalPages * 100 : 0;
	public PdfModelProperties? PdfProperties      { get; private set; }

	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("Linux")]
	[SupportedOSPlatform("macOS")]
	[SupportedOSPlatform("iOS13.6")]
	[SupportedOSPlatform("MacCatalyst13.5")]
	[SupportedOSPlatform("Android31.0")]
	public async Task<PdfModelProperties?> ConvertPdfToImageAsync(string pdfPath, int dpi = 300, int width = 2560) {
		try {
			PdfProperties = new PdfModelProperties {
				FileNameBeforeNumber = "doc_page_",
				FileExtensionEnd     = ".png",
				OutputDirectory      = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
			};
			Directory.CreateDirectory(PdfProperties.OutputDirectory);

			StatusText = "Loading PDF document ...";
			int pageCount;
			using (var pdfStream = File.OpenRead(pdfPath)) {
				pageCount = Conversion.GetPageCount(pdfStream);
			}

			TotalPages  = pageCount;
			PdfProperties.PageCount = pageCount;
			for (var j = 0; j < pageCount; j++) {
				PdfProperties.Slides.Add(new SlideModel {
					PhysicalPageNumber = j, LogicalPageNumber = j.ToString(), OverlayNumber = 0
				});
			}
			CurrentPage = 0;
			StatusText  = $"Converting {TotalPages} pages ...";
			Debug.WriteLine($"Converting {TotalPages} pages");

			using (var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import)) {
				var pageLabels = document.Internals.Catalog.Elements.GetDictionary("/PageLabels");
				var numsArray  = pageLabels?.Elements.GetArray("/Nums");
				if (numsArray != null) {
					for (var j = 0; j < numsArray.Elements.Count; j += 2) {
						var pageIndex = int.Parse(numsArray.Elements[j].ToString() ?? "-1");
						var labelDict = numsArray.Elements[j + 1] as PdfDictionary;
						var prefix    = labelDict?.Elements.GetString("/P") ?? pageIndex.ToString();
						var overlay   = 0;
						for (var p = pageIndex; p != -1 && p < pageCount; ++p) {
							PdfProperties.Slides[p].LogicalPageNumber = prefix;
							PdfProperties.Slides[p].OverlayNumber     = overlay++;
						}
					}
				}
			}

			PdfProperties.LogicalPageCount = PdfProperties
			                                 .Slides.GroupBy(slide => slide.LogicalPageNumber).Select(s => s.First())
			                                 .Count();
			
			var pageNumbers    = Enumerable.Range(0, TotalPages);
			var completedPages = 0;
			var lockObject     = new object();

			await Task.Run(() => {
				var parallelOptions = new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount, 
					CancellationToken = _cancellationSource.Token
				};
				Parallel.ForEach(pageNumbers, parallelOptions, pageIndex => {
					_cancellationSource.Token.ThrowIfCancellationRequested();
					try {
						ConvertPageToImage(pdfPath, pageIndex, PdfProperties.OutputDirectory,
							new RenderOptions {
								Dpi             = dpi,
								BackgroundColor = SKColors.Transparent,
								UseTiling       = false,
								Width           = width,
								Height          = null,
								WithAspectRatio = true
							});
						lock (lockObject) {
							completedPages++;
							PdfProperties.Slides[pageIndex].ImagePath = Path.Combine(PdfProperties.OutputDirectory,
								$"doc_page_{(pageIndex + 1):000}.png");
							Dispatcher.UIThread.InvokeAsync(() => {
								CurrentPage = completedPages;
								StatusText = $"Converted {completedPages} of {TotalPages} pages ...";
								OnPropertyChanged(nameof(ProgressPercentage));
							});
						}
					} catch (Exception ex) {
						Dispatcher.UIThread.InvokeAsync(() => {
							StatusText = $"Error while converting page {pageIndex}: {ex.Message}";
						});
					}
				});
			}, _cancellationSource.Token);
			
			StatusText   = $"Conversion complete! {TotalPages} pages converted!";
			IsConverting = false;
			
			return PdfProperties;
		} catch (OperationCanceledException) {
			StatusText   = "Conversation cancelled.";
			IsConverting = false;
			return null;
		} catch (Exception ex) {
			StatusText   = $"Conversion failed: {ex.Message}";
			IsConverting = false;
			throw;
		}
	}
	
	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("Linux")]
	[SupportedOSPlatform("macOS")]
	[SupportedOSPlatform("iOS13.6")]
	[SupportedOSPlatform("MacCatalyst13.5")]
	[SupportedOSPlatform("Android31.0")]
	private static void ConvertPageToImage(string pdfPath, int pageIndex, string outputDirectory, RenderOptions options) {
		var       outputPath  = Path.Combine(outputDirectory, $"doc_page_{(pageIndex + 1):000}.png");
		using var inputStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
			FileOptions.SequentialScan);
		using var bitmap = Conversion.ToImage(inputStream, page: pageIndex, options: options);
		using var image  = SKImage.FromBitmap(bitmap);
		using var data   = image.Encode(SKEncodedImageFormat.Png, 100);
		using var stream = File.OpenWrite(outputPath);
		data.SaveTo(stream);
	}
	
	public event PropertyChangedEventHandler? PropertyChanged;
	
	protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
		if (Equals(field, value)) return;
		field = value;
		OnPropertyChanged(propertyName);
	}
	
	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
	
	public void Cancel() {
		_cancellationSource?.Cancel();
		StatusText = "Cancelling conversion...";
	}

	public void Dispose() {
		_cancellationSource?.Dispose();
	}
}