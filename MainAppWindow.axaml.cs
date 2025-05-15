using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace magazine_music;

public partial class MainAppWindow : Window
{
    public ObservableCollection<ProductPresenter> productPresenters { get; set; }
    public MainAppWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public void LoadData()
    {
        using var dbContext = new User9Context();
        var products = dbContext.Instruments
            .Include(i => i.Type)
            .Include(i => i.Brand)
            .Select(i => new ProductPresenter
            {
                InstrumentId = i.InstrumentId,
                InstrumentName = i.InstrumentName,
                InstrumentDescription = i.InstrumentDescription,
                InstrumentPrice = i.InstrumentPrice,
                InstrumentQuantity = i.InstrumentQuantity,
                BrandId = i.Brand.BrandId,
                BrandName = i.Brand.BrandName,
                TypeId = i.TypeId,
                TypeName = i.Type.TypeName,
                // ƒобавл€ем путь к первой картинке в новое свойство
                InstrumentPhoto = dbContext.Instrumentimages
                    .Where(img => img.InstrumentId == i.InstrumentId)
                    .OrderBy(img => img.ImageId)
                    .Select(img => img.ImagePath)
                    .FirstOrDefault()
            }).ToList();

        productPresenters = new ObservableCollection<ProductPresenter>(products);
        ProductListBox.ItemsSource = productPresenters;
    }

    public class ProductPresenter : Instrument
    {
        public string BrandName { get; set; }
        public string TypeName { get; set; }

        // ƒобавл€ем здесь свойство InstrumentPhoto, чтобы оно было доступно
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

    private void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ProductListBox.SelectedItem is ProductPresenter selectedProduct)
        {
            var productDetailsWindow = new ProductDetailsWindow(selectedProduct);
            productDetailsWindow.Show();
        }
    }
}
