using Avalonia;
using Avalonia.Controls;
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
            .Select(products => new ProductPresenter
            {
                InstrumentId = products.InstrumentId,
                InstrumentName = products.InstrumentName,
                InstrumentDescription = products.InstrumentDescription,
                InstrumentPrice = products.InstrumentPrice,
                InstrumentPhoto = products.InstrumentPhoto,
                InstrumentQuantity = products.InstrumentQuantity,
                BrandId = products.Brand.BrandId,
                BrandName = products.Brand.BrandName,
                TypeId = products.TypeId,
                TypeName = products.Type.TypeName
            }).ToList();
        productPresenters = new ObservableCollection<ProductPresenter>(products);
        ProductListBox.ItemsSource = productPresenters;
    }

    public class ProductPresenter : Instrument
    {
        public string BrandName { get; set; }
        public string TypeName { get; set; }

        public Bitmap Image
        {
            get
            {
                try
                {
                    return new Bitmap(InstrumentPhoto);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}