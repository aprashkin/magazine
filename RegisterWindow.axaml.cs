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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Avalonia.Media;

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
        string verifyCode = code2;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Подтверждение", "aprashkind@yandex.ru"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Код подтверждения регистрации";
        message.Body = new TextPart("plain")
        {
            Text = $"Ваш код подтверждения: {verifyCode}"
        };
        using (var client = new SmtpClient())
        {
            client.Connect("smtp.yandex.ru", 587, SecureSocketOptions.StartTls);
            client.Authenticate("aprashkind@yandex.ru", "kcabrdzqsjgzuxoi");
            client.Send(message);
            client.Disconnect(true);
        }
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

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
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
        if (string.IsNullOrEmpty(RepeatPasswordTextBox.Text))
        {
            ShowError("Повторите пароль!");
            return;
        }
        ValidatePasswordMatch();

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
        DateTime today = DateTime.Today;
        int age = today.Year - selectedDate.Value.Year;
        if (selectedDate.Value.Date > today.AddYears(-age)) age--; // Учитываем день и месяц
        if (age < 16)
        {
            ShowError("Для регистрации вам должно быть не менее 16 лет.");
            return;
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
        else
        {
            using (var dbContext = new User9Context())
            {
                bool emailExists = dbContext.Users.Any(u => u.UserEmail == EmailTextBox.Text);
                if (emailExists)
                {
                    ShowError("Пользователь с таким Email уже зарегистрирован.");
                    return;
                }

                // Если email не существует, то создаем нового пользователя и отправляем email с верификацией
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
                // Генерация и отправка email для верификации     
                SendVerificationEmail(EmailTextBox.Text, verifyCode);
                var verificationWindow = new VerificationWindow(verifyCode, newUser);
                var result = await verificationWindow.ShowDialog<bool>(this);
                if (result)
                {
                    // Если верификация успешна, показываем сообщение
                    var msgBox = MessageBoxManager
                        .GetMessageBoxStandard("Регистрация", "Регистрация успешно завершена!");
                    await msgBox.ShowAsync();
                    // Открываем окно входа
                    var loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
        }         
    }

    private void BackToMain_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
    private void ValidatePasswordMatch()
    {
        if (PasswordTextBox.Text != RepeatPasswordTextBox.Text)
        {
            RepeatPasswordTextBox.BorderBrush = Brushes.Red;
        }
        else
        {
            RepeatPasswordTextBox.BorderBrush = Brushes.Green;
        }
    }

    // Обработчик TextChanged
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidatePasswordMatch();
    }

}