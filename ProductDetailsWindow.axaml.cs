using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Diagnostics.Metrics;
using static magazine_music.MainAppWindow;

namespace magazine_music;

public partial class ProductDetailsWindow : Window
{
    Instrument Instrumentus { get; set; }
    public ProductDetailsWindow(ProductPresenter product)
    {
        InitializeComponent();
        ProductName.Text = product.InstrumentName;
        ProductDescription.Text = product.InstrumentDescription;
        ProductPrice.Text = $"{product.InstrumentPrice:C}";
        BrandName.Text = product.BrandName;
        ProductImage.Source = product.Image;
    }

    public ProductDetailsWindow()
    {
        
    }
}