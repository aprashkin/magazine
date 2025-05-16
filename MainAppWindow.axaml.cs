using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        productPresenters = new ObservableCollection<ProductPresenter>(products);
        ProductListBox.ItemsSource = productPresenters;
    }


    public class ProductPresenter : Instrument
    {
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public List<string> ImagePaths { get; set; } 

        // Добавляем здесь свойство InstrumentPhoto, чтобы оно было доступно
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
            this.Close();
        }
    }
}
