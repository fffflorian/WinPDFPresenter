using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Media;
using Avalonia.Threading;

namespace WinPDFPresenter.Models;

public class Timer : INotifyPropertyChanged {
	private          int             _elapsedTime;
	private readonly int             _durationInSeconds;
	private readonly int             _lastSeconds;
	private readonly DispatcherTimer _timer        = new ();
	private readonly string          _timeFormat   = @"mm\:ss";
	private readonly IBrush          _warningBrush = Brushes.Coral;
	private readonly IBrush          _finalBrush   = Brushes.Red;
	
	public event PropertyChangedEventHandler? PropertyChanged;
	public string                             CurrentTime { get; set; }
	public IBrush                             TimerBrush  { get; set; } = Brushes.White;

	public Timer(int duration, int lastMinutes) {
		if (duration >= 60) _timeFormat = @"hh\:mm\:ss";
		_elapsedTime       =  0;
		_durationInSeconds =  duration * 60;
		_lastSeconds       =  lastMinutes * 60;
		_timer.Interval    =  TimeSpan.FromSeconds(1);
		_timer.Tick        += TimerOnTick;
		CurrentTime        =  $"{TimeSpan.FromSeconds(_durationInSeconds).ToString(_timeFormat)}";
	}

	public void StartTimer() {
		_timer.Start();
	}

	public void PauseTimer() {
		_timer.Stop();
	}

	public void RestartTimer() {
		PauseTimer();
		_elapsedTime = 0;
	}

	private void TimerOnTick(object? sender, EventArgs e) {
		Debug.WriteLine("Tick Tock!");
		_elapsedTime++;
		var currentTime = _durationInSeconds - _elapsedTime;
		if (_elapsedTime >= _durationInSeconds) {
			PauseTimer();
			TimerBrush  = _finalBrush;
			CurrentTime = "Time is up!";
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimerBrush)));
			return;
		}
		if (_elapsedTime >= _durationInSeconds - _lastSeconds) {
			TimerBrush  = _warningBrush;
		}
		CurrentTime = $"{TimeSpan.FromSeconds(currentTime).ToString(_timeFormat)}";
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimerBrush)));
	}
}