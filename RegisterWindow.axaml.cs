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
    
    //����� �� ��������� ���� �������������
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
    //����� �� �������� ������
    static void SendVerificationEmail(string toEmail, string code2)
    {
        string verifyCode = code2;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("�������������", "aprashkind@yandex.ru"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "��� ������������� �����������";
        message.Body = new TextPart("plain")
        {
            Text = $"��� ��� �������������: {verifyCode}"
        };
        using (var client = new SmtpClient())
        {
            client.Connect("smtp.yandex.ru", 587, SecureSocketOptions.StartTls);
            client.Authenticate("aprashkind@yandex.ru", "kcabrdzqsjgzuxoi");
            client.Send(message);
            client.Disconnect(true);
        }
    }
    //����� �� �������� ����� ����� ���.����
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
            ShowError("������� Email!");
            return;
        }
        if (string.IsNullOrEmpty(PasswordTextBox.Text))
        {
            ShowError("������� ������!");
            return;
        }
        if (string.IsNullOrEmpty(RepeatPasswordTextBox.Text))
        {
            ShowError("��������� ������!");
            return;
        }
        ValidatePasswordMatch();

        if (string.IsNullOrEmpty(FirstNameTextBox.Text))
        {
            ShowError("������� ���!");
            return;
        }
        if (string.IsNullOrEmpty(LastNameTextBox.Text))
        {
            ShowError("������� �������!");
            return;
        }
        if (!DateOfBirthPicker.SelectedDate.HasValue)
        {
            ShowError("�������� ���� ��������!");
        }
        DateTime today = DateTime.Today;
        int age = today.Year - selectedDate.Value.Year;
        if (selectedDate.Value.Date > today.AddYears(-age)) age--; // ��������� ���� � �����
        if (age < 16)
        {
            ShowError("��� ����������� ��� ������ ���� �� ����� 16 ���.");
            return;
        }
        var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text));
        
        //���������� ������
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
            ShowError("�������� ���!");
            return;
        }
        //��������� �����
        if (!IsValidEmail(EmailTextBox.Text))
        {
            ShowError("�������� Email");
            return;
        }
        else
        {
            using (var dbContext = new User9Context())
            {
                bool emailExists = dbContext.Users.Any(u => u.UserEmail == EmailTextBox.Text);
                if (emailExists)
                {
                    ShowError("������������ � ����� Email ��� ���������������.");
                    return;
                }

                // ���� email �� ����������, �� ������� ������ ������������ � ���������� email � ������������
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
                // ��������� � �������� email ��� �����������     
                SendVerificationEmail(EmailTextBox.Text, verifyCode);
                var verificationWindow = new VerificationWindow(verifyCode, newUser);
                var result = await verificationWindow.ShowDialog<bool>(this);
                if (result)
                {
                    // ���� ����������� �������, ���������� ���������
                    var msgBox = MessageBoxManager
                        .GetMessageBoxStandard("�����������", "����������� ������� ���������!");
                    await msgBox.ShowAsync();
                    // ��������� ���� �����
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

    // ���������� TextChanged
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidatePasswordMatch();
    }

}