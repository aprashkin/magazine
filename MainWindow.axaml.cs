using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using magazine_music.Context;
using MsBox.Avalonia;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace magazine_music
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void EnterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            using var dbContext = new PostgresContext();
            var email = EmailTextBox.Text;
            var password = PasswordTextBox.Text;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Пожалуйста, заполните все поля");
                return;
            }
            else
            {
                using SHA256 sha256 = SHA256.Create();

                var unhashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text));
                var user = dbContext.Users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == unhashedPassword);
                if (user != null)
                {
                    ShowError("Вы успешно вошли в систему");
                }
                else
                {
                    ShowError("Неправильный логин или пароль");
                }
            }
        }
        private void RegisterButton_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newWindow = new RegisterWindow();
            newWindow.Show();
            this.Close();
        }
        private async void ShowError(string message)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await box.ShowAsync();
        }
    }
}

