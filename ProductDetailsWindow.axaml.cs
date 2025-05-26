using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using MsBox.Avalonia;
using System.Collections.Generic;
using System.Linq;
using static magazine_music.MainAppWindow;

namespace magazine_music;

public partial class ProductDetailsWindow : Window
{
    private ProductPresenter _product;
    private bool isInCart = false;

    public ProductDetailsWindow(ProductPresenter product)
    {
        InitializeComponent();
        _product = product;

        LoadProductDetails();
        EditButton.IsVisible = Session.CurrentUser.RoleId == 1;
    }

    public ProductDetailsWindow()
    {
        InitializeComponent();
        
    }

    private void LoadProductDetails()
    {
        // Назначение базовых текстов и первой картинки
        this.FindControl<TextBlock>("ProductNameText").Text = _product.InstrumentName;
        this.FindControl<TextBlock>("ProductDescriptionText").Text = _product.InstrumentDescription;
        this.FindControl<TextBlock>("ProductPriceText").Text = $"Цена: {_product.InstrumentPrice} ₽";
        this.FindControl<TextBlock>("ProductTypeText").Text = $"Тип инструмента: {_product.TypeName}";

        // Отображение главной картинки
        var mainImage = this.FindControl<Image>("ProductImage");
        try
        {
            if (!string.IsNullOrWhiteSpace(_product.InstrumentPhoto))
                mainImage.Source = new Bitmap(_product.InstrumentPhoto);
        }
        catch
        {
            mainImage.Source = new Bitmap("Товары/default.jpg");
        }

        // Загрузка всех изображений в ItemsControl
        var imagesList = this.FindControl<ItemsControl>("ImagesList");

        var bitmaps = _product.ImagePaths?
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path =>
            {
                try
                {
                    return new Bitmap(path);
                }
                catch
                {
                    return null;
                }
            })
            .Where(bitmap => bitmap != null)
            .ToList();

        imagesList.ItemsSource = bitmaps ?? new List<Bitmap>();
    }

    private void Image_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is Image img && img.Source is Bitmap bitmap)
        {
            var mainImage = this.FindControl<Image>("ProductImage");
            mainImage.Source = bitmap;
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
            CartService.AddToCart(Session.CurrentUser.UserId, _product.InstrumentId);
            isInCart = true;
    }

    private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainAppWindow = new MainAppWindow();
        mainAppWindow.Show();
        this.Close();
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var editProductWindow = new EditProductWindow(_product);
        editProductWindow.Show();
        this.Close();
    }
}
