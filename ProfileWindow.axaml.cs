using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace magazine_music;

public partial class ProfileWindow : Window
{
    public string UserFirstName { get; set; }
    public string UserEmail { get; set; }
    public string UserBirthday { get; set; }
    public string UserLastname { get; set; }

    public ProfileWindow()
    {
        InitializeComponent();
        if (Session.CurrentUser != null)
        {
            UserFirstName = $"���: {Session.CurrentUser.UserFirstname}";
            UserEmail = $"Email: {Session.CurrentUser.UserEmail}";
            UserLastname = $"�������: {Session.CurrentUser.UserLastname}";
            UserBirthday = $"���� ��������: {Session.CurrentUser.UserBirthday}";
            DataContext = this;
        }
    }

    private void Logout_Click(object? sender, RoutedEventArgs e)
    {
        Session.CurrentUser = null;
        var loginWindow = new MainWindow();
        loginWindow.Show();
        this.Close();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var newWindow = new MainAppWindow();
        newWindow.Show();
        this.Close();
    }
}