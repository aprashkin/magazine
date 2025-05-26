using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace magazine_music;

public partial class MainAppWindow : Window
{
    public ObservableCollection<ProductPresenter> productPresenters { get; set; }
    private ObservableCollection<ProductPresenter> allProducts;
    private decimal selectedMaxPrice;
    public MainAppWindow()
    {
        InitializeComponent();
        LoadData();
        LoadFilters();
        HideAddButton();
    }
    private void HideAddButton()
    {
        
        if (Session.CurrentUser.RoleId != 1) // Если не администратор
        {
            Add_Button.IsVisible = false;
        }
    }

    public void LoadData()
    {
        using var dbContext = new User9Context();

        // Загружаем инструменты с брендами и типами
        var instruments = dbContext.Instruments
            .Include(i => i.Brand)
            .Include(i => i.Type)
            .ToList();

        // Загружаем все изображения
        var allImages = dbContext.Instrumentimages.ToList();

        // Преобразуем в ProductPresenter
        var products = instruments.Select(instrument => new ProductPresenter
        {
            InstrumentId = instrument.InstrumentId,
            InstrumentName = instrument.InstrumentName,
            InstrumentDescription = instrument.InstrumentDescription,
            InstrumentPrice = instrument.InstrumentPrice,
            InstrumentQuantity = instrument.InstrumentQuantity,
            BrandId = instrument.BrandId,
            BrandName = instrument.Brand?.BrandName,
            TypeId = instrument.TypeId,
            TypeName = instrument.Type?.TypeName,

            // Главное фото — берём первое из связанных
            InstrumentPhoto = allImages
                .Where(img => img.InstrumentId == instrument.InstrumentId)
                .Select(img => img.ImagePath)
                .FirstOrDefault(),

            // Все изображения
            ImagePaths = allImages
                .Where(img => img.InstrumentId == instrument.InstrumentId)
                .Select(img => img.ImagePath)
                .ToList()
        }).ToList();

        allProducts = new ObservableCollection<ProductPresenter>(products);
        ProductItemsControl.ItemsSource = allProducts;
    }


    public class ProductPresenter : Instrument
    {
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public List<string> ImagePaths { get; set; } 

        
        public string InstrumentPhoto { get; set; }

        public Bitmap Image
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(InstrumentPhoto) || !File.Exists(InstrumentPhoto))
                        return new Bitmap("Товары/default.jpg");

                    return new Bitmap(InstrumentPhoto);
                }
                catch
                {
                    return new Bitmap("Товары/default.jpg");
                }
            }
        }
    }

    private void LoadFilters()
    {
        using var dbContext = new User9Context();

        // Типы инструментов
        var types = dbContext.InstrumentsTypes.ToList();
        types.Insert(0, new InstrumentsType { TypeId = 0, TypeName = "Все" }); // пункт для сброса фильтра
        TypeFilterComboBox.ItemsSource = types;
        TypeFilterComboBox.SelectedIndex = 0;

        // Бренды
        var brands = dbContext.Brands.ToList();
        brands.Insert(0, new Brand { BrandId = 0, BrandName = "Все" }); // пункт для сброса фильтра
        BrandFilterComboBox.ItemsSource = brands;
        BrandFilterComboBox.SelectedIndex = 0;
    }
    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var profileWindow = new ProfileWindow();
        profileWindow.Show();
        this.Close();
    }
    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        var query = SearchBox.Text?.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(query))
        {
            ProductItemsControl.ItemsSource = allProducts;
        }
        else
        {
            var filtered = allProducts
                .Where(p =>
                    p.InstrumentName.ToLower().Contains(query) ||
                    p.BrandName.ToLower().Contains(query) ||
                    p.TypeName.ToLower().Contains(query))
                .ToList();

            ProductItemsControl.ItemsSource = filtered;
        }
    }

    private void Item_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is StackPanel panel && panel.DataContext is ProductPresenter product)
        {
            var detailsWindow = new ProductDetailsWindow(product);
            detailsWindow.Show();
            this.Close();
        }
    }

    private void ComboBox_SelectionChanged_1(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var search = SearchBox.Text?.ToLower() ?? "";
        var selectedType = TypeFilterComboBox.SelectedItem as InstrumentsType;
        var selectedBrand = BrandFilterComboBox.SelectedItem as Brand;

        var filtered = allProducts.Where(p =>
            (string.IsNullOrWhiteSpace(search) ||
             p.InstrumentName.ToLower().Contains(search) ||
             p.BrandName.ToLower().Contains(search) ||
             p.TypeName.ToLower().Contains(search)) &&
            (selectedType == null || selectedType.TypeId == 0 || p.TypeId == selectedType.TypeId) &&
            (selectedBrand == null || selectedBrand.BrandId == 0 || p.BrandId == selectedBrand.BrandId)
        ).ToList();
        ProductItemsControl.ItemsSource = filtered;
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var editWindow = new EditProductWindow();
        editWindow.Show();
        this.Close();
    }
}
