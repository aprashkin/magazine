using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using magazine_music.Context;
using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using MsBox.Avalonia;
using System.Threading.Tasks;

namespace magazine_music;

public partial class RegisterWindow : Window
{
    string generatedCode;
    public RegisterWindow()
    {
        InitializeComponent();
    }
    private async Task RegistrationButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //переводим дату в формат для бд и хэшируем пароль
        var selectedDate = DateOfBirthPicker.SelectedDate;
        using SHA256 sha256 = SHA256.Create();
        var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text));
        using var dbContext = new PostgresContext();
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
            SendVerificationEmail(EmailTextBox.Text);
            var verificationWindow = new VerificationWindow(generatedCode);
            await verificationWindow.ShowDialog(this);
            if (verificationWindow.IsVerified) 
            {
                var newMainWindow = new MainAppWindow();
                newMainWindow.Show();
                this.Close();
            }
        }

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
    static void SendVerificationEmail(string toEmail)
    {
        string verifyCode = GetVerificationCode();
        string generatedCode = verifyCode;
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("no-reply@myapp.com"); // не важно что здесь, для Mailtrap
        mail.To.Add(toEmail);
        mail.Subject = "Код подтверждения регистрации";
        mail.Body = $"Ваш код подтверждения: {verifyCode}";

        SmtpClient smtp = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
        {
            Credentials = new NetworkCredential("78f0830cba4d03", "973ec7818076a0"),
            EnableSsl = true
        };

        smtp.Send(mail);
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
        var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text));
        using var dbContext = new PostgresContext();
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
            SendVerificationEmail(EmailTextBox.Text);
            var verificationWindow = new VerificationWindow(generatedCode);
            sho();
            async Task sho()
            {
                await verificationWindow.ShowDialog(this);
            }
            
            if (verificationWindow.IsVerified)
            {
                var newMainWindow = new MainAppWindow();
                newMainWindow.Show();
                this.Close();
            }
        }
    }
}