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
    }

    public void LoadData()
    {
        using var dbContext = new User9Context();

        // ��������� ����������� � �������� � ������
        var instruments = dbContext.Instruments
            .Include(i => i.Brand)
            .Include(i => i.Type)
            .ToList();

        // ��������� ��� �����������
        var allImages = dbContext.Instrumentimages.ToList();

        // ����������� � ProductPresenter
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

            // ������� ���� � ���� ������ �� ���������
            InstrumentPhoto = allImages
                .Where(img => img.InstrumentId == instrument.InstrumentId)
                .Select(img => img.ImagePath)
                .FirstOrDefault(),

            // ��� �����������
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

        // ��������� ����� �������� InstrumentPhoto, ����� ��� ���� ��������
        public string InstrumentPhoto { get; set; }

        public Bitmap Image
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(InstrumentPhoto))
                        return null;

                    return new Bitmap(InstrumentPhoto);
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    private void LoadFilters()
    {
        using var dbContext = new User9Context();

        // ���� ������������
        var types = dbContext.InstrumentsTypes.ToList();
        types.Insert(0, new InstrumentsType { TypeId = 0, TypeName = "���" }); // ����� ��� ������ �������
        TypeFilterComboBox.ItemsSource = types;
        TypeFilterComboBox.SelectedIndex = 0;

        // ������
        var brands = dbContext.Brands.ToList();
        brands.Insert(0, new Brand { BrandId = 0, BrandName = "���" }); // ����� ��� ������ �������
        BrandFilterComboBox.ItemsSource = brands;
        BrandFilterComboBox.SelectedIndex = 0;
    }

    


    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
}
