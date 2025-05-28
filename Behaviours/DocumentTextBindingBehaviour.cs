using System;
using AvaloniaEdit;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace WinPDFPresenter.Behaviours;

public class DocumentTextBindingBehaviour : Behavior<TextEditor> {
	private TextEditor? _textEditor = null;
	public string Text {
		get => GetValue(TextProperty); set => SetValue(TextProperty, value);
	}
	public static readonly StyledProperty<string> TextProperty =
		AvaloniaProperty.Register<DocumentTextBindingBehaviour, string>(nameof(Text));

	protected override void OnAttached() {
		base.OnAttached();
		var textEditor = AssociatedObject;
		if (textEditor is null) return;
		_textEditor             =  textEditor;
		_textEditor.TextChanged += TextEditorOnTextChanged;
		this.GetObservable(TextProperty).Subscribe(TextPropertyChanged);
	}

	protected override void OnDetaching() {
		base.OnDetaching();
		if (_textEditor != null) {
			_textEditor.TextChanged -= TextEditorOnTextChanged;
		}
	}

	private void TextPropertyChanged(string? text) {
		if (_textEditor?.Document is null || text is null) return;
		var caretOffset = _textEditor.CaretOffset;
		_textEditor.Document.Text = text;
		_textEditor.CaretOffset = caretOffset;
	}

	private void TextEditorOnTextChanged(object? sender, EventArgs e) {
		if (_textEditor?.Document is null) return;
		Text = _textEditor.Document.Text;
	}
}