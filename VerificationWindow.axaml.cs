using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MsBox.Avalonia;
using System;

namespace magazine_music;

public partial class VerificationWindow : Window
{
    private string _correctCode;
    private DispatcherTimer _timer;
    private TimeSpan _timeLimit = TimeSpan.FromMinutes(5);

    public bool IsVerified { get; private set; } = false;
    public VerificationWindow(string correctCode)
    {
        InitializeComponent();
        _correctCode = correctCode;

        StartTimer();
        this.Closing += OnWindowClosing;
    }

    public VerificationWindow()
    {

    }
    private void StartTimer()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        _timeLimit = _timeLimit.Subtract(TimeSpan.FromSeconds(1));
        if (_timeLimit <= TimeSpan.Zero)
        {
            _timer.Stop();
            MessageBoxManager.GetMessageBoxStandard("����� �����", "�� �� ������ ������ ���.");
            Close(); // ��������� ���� ����� 5 �����
        }
    }
    private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _timer?.Stop(); // ������������� ������ ��� ����� �������� ����
                        // IsVerified ��������� false, ���� ������������ ������ ������ ���� �������
    }

    private void VerifyCode(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CodeTextBox.Text == _correctCode)
        {
            IsVerified = true;
            _timer?.Stop();
            this.Close();
        }
    }
}