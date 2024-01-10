using System;
using System.ComponentModel;

public class AudioPlayerViewModel : INotifyPropertyChanged
{
    private TimeSpan currentTime;
    private TimeSpan totalTime;
    private double currentProgress;

    public event PropertyChangedEventHandler PropertyChanged;

    public TimeSpan CurrentTime
    {
        get { return currentTime; }
        set
        {
            if (currentTime != value)
            {
                currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
    }

    public TimeSpan TotalTime
    {
        get { return totalTime; }
        set
        {
            if (totalTime != value)
            {
                totalTime = value;
                OnPropertyChanged(nameof(TotalTime));
            }
        }
    }

    public double CurrentProgress
    {
        get { return currentProgress; }
        set
        {
            if (currentProgress != value)
            {
                currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }
    }

    // You can add more properties and methods related to your audio player here.

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
