using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using magazine_music.Context;
using System;
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using MsBox.Avalonia;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;

namespace magazine_music;

public partial class RegisterWindow : Window
{
    string generatedCode;
    string verifyCode = GetVerificationCode();
    public RegisterWindow()
    {
        InitializeComponent();
        
    }
    
    //метод на генерацию кода подтверждения
    private static string GetVerificationCode()
    {
        int length = 6;
        Random rnd = new Random();
        string code = string.Empty;
        for (int i = 0; i < length; i++)
        {
            code += rnd.Next(0, 9).ToString();  
        }
        return code;
    }
    //метод на отправку письма
    static void SendVerificationEmail(string toEmail, string code2)
    {
        

    }
    //метод на проверку почты через нет.маил
    private bool IsValidEmail(string email)
    {
        try
        {
            var address = new System.Net.Mail.MailAddress(email);
            return address.Address == email;
        }
        catch 
        { 
            return false; 
        }
    }

    private async void ShowError(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Error", message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
        await box.ShowAsync();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // переводим дату в формат для бд и хэшируем пароль
        var selectedDate = DateOfBirthPicker.SelectedDate;
        using SHA256 sha256 = SHA256.Create();
        if(string.IsNullOrEmpty(EmailTextBox.Text))
        {
            ShowError("Введите Email!");
            return;
        }
        if (string.IsNullOrEmpty(PasswordTextBox.Text))
        {
            ShowError("Введите пароль!");
            return;
        }
        if (string.IsNullOrEmpty(FirstNameTextBox.Text))
        {
            ShowError("Введите имя!");
            return;
        }
        if (string.IsNullOrEmpty(LastNameTextBox.Text))
        {
            ShowError("Введите фамилию!");
            return;
        }
        if (!DateOfBirthPicker.SelectedDate.HasValue)
        {
            ShowError("Выберите дату рождения!");
        }
        var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text));
        
        //определяем гендер
        int genderValue = 0;
        if (MaleRadioButton.IsChecked == true)
        {
            genderValue = 1;
        }
        else if (FemaleRadioButton.IsChecked == true)
        {
            genderValue = 2;
        }
        else
        {
            ShowError("Выберите пол!");
            return;
        }
        //проверяем почту
        if (!IsValidEmail(EmailTextBox.Text))
        {
            ShowError("Неверный Email");
            return;
        }
        else //тут открываем окошко с верификацией
        {
            var newUser = new User
            {
                UserFirstname = FirstNameTextBox.Text,
                UserLastname = LastNameTextBox.Text,
                UserEmail = EmailTextBox.Text,
                GenderId = genderValue,
                UserPassword = hashedPassword,
                RoleId = 2,
                UserBirthday = DateOnly.FromDateTime(DateOfBirthPicker.SelectedDate.Value.Date),
            };
            SendVerificationEmail(EmailTextBox.Text, verifyCode);
            var verificationWindow = new VerificationWindow(verifyCode, newUser);
            verificationWindow.Show();
            this.Close();
            
            
        }
    }
}