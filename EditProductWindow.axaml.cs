using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static magazine_music.MainAppWindow;

namespace magazine_music
{
    public partial class EditProductWindow : Window
    {
        private const string ImagesFolder = "Товары"; // папка с картинками относительно исполняемой директории

        private ProductPresenter? _product;
        private bool _isEditMode;

        public ObservableCollection<Instrumentimage> Images { get; } = new ObservableCollection<Instrumentimage>();

        public EditProductWindow(ProductPresenter? product = null)
        {
            InitializeComponent();
            DataContext = this;

            if (product == null)
            {
                // Создаем новый пустой инструмент в базе
                using var db = new User9Context();
                var newInstrument = new Instrument
                {
                    InstrumentName = "",
                    InstrumentDescription = "",
                    InstrumentPrice = 0,
                    InstrumentQuantity = 0,
                    BrandId = null,
                    TypeId = null
                };
                db.Instruments.Add(newInstrument);
                db.SaveChanges();

                _product = new ProductPresenter
                {
                    InstrumentId = newInstrument.InstrumentId,
                    InstrumentName = newInstrument.InstrumentName,
                    InstrumentDescription = newInstrument.InstrumentDescription,
                    InstrumentPrice = newInstrument.InstrumentPrice,
                    InstrumentQuantity = newInstrument.InstrumentQuantity,
                    BrandId = newInstrument.BrandId,
                    TypeId = newInstrument.TypeId
                };
                _isEditMode = true;
            }
            else
            {
                _product = product;
                _isEditMode = true;
            }

            LoadData();
            LoadImages();
        }
        public EditProductWindow() : this(null)
        {
        }

        private void LoadData()
        {
            using var dbContext = new User9Context();

            BrandComboBox.ItemsSource = dbContext.Brands.ToList();
            TypeComboBox.ItemsSource = dbContext.InstrumentsTypes.ToList();

            if (_product != null)
            {
                Title = _isEditMode ? "Редактирование товара" : "Добавление нового товара";
                SaveButton.Content = _isEditMode ? "Сохранить изменения" : "Добавить товар";

                NameBox.Text = _product.InstrumentName;
                DescriptionBox.Text = _product.InstrumentDescription;
                PriceBox.Text = _product.InstrumentPrice?.ToString() ?? "";
                QuantityBox.Text = _product.InstrumentQuantity?.ToString() ?? "";

                BrandComboBox.SelectedItem = BrandComboBox.ItemsSource.Cast<Brand>().FirstOrDefault(b => b.BrandId == _product.BrandId);
                TypeComboBox.SelectedItem = TypeComboBox.ItemsSource.Cast<InstrumentsType>().FirstOrDefault(t => t.TypeId == _product.TypeId);
            }
        }

        private void LoadImages()
        {
            Images.Clear();

            if (_product == null)
                return;

            using var db = new User9Context();

            var images = db.Instrumentimages
                .Where(img => img.InstrumentId == _product.InstrumentId)
                .ToList();

            foreach (var img in images)
                Images.Add(img);
        }

        /*private async void AddImage_Click(object? sender, RoutedEventArgs e)
        {
            if (_product == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Сначала сохраните товар, затем добавляйте картинки.").ShowAsync();
                return;
            }

            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Images", Extensions = { "jpg", "png", "jpeg", "bmp" } });
            dlg.AllowMultiple = true;
            var result = await dlg.ShowAsync(this);

            if (result == null || result.Length == 0)
                return;

            using var db = new User9Context();

            foreach (var filePath in result)
            {
                try
                {
                    var fileName = Path.GetFileName(filePath);
                    var destDir = Path.Combine(AppContext.BaseDirectory, ImagesFolder);
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    var destPath = Path.Combine(destDir, fileName);

                    // Если файл уже есть, добавляем суффикс
                    if (File.Exists(destPath))
                    {
                        var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                        destPath = Path.Combine(destDir, newFileName);
                        fileName = newFileName;
                    }

                    File.Copy(filePath, destPath);

                    var newImage = new Instrumentimage
                    {
                        InstrumentId = _product!.InstrumentId,
                        ImagePath = Path.Combine(ImagesFolder, fileName)
                    };

                    db.Instrumentimages.Add(newImage);
                    db.SaveChanges();

                    Images.Add(newImage);
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Не удалось добавить картинку: {ex.Message}").ShowAsync();
                }
            }
        }*/

        private void DeleteImage_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Instrumentimage img)
            {
                using var db = new User9Context();

                var imageToDelete = db.Instrumentimages.FirstOrDefault(i => i.ImageId == img.ImageId);
                if (imageToDelete != null)
                {
                    db.Instrumentimages.Remove(imageToDelete);
                    db.SaveChanges();

                    try
                    {
                        var fullPath = Path.Combine(AppContext.BaseDirectory, imageToDelete.ImagePath);
                        if (File.Exists(fullPath))
                            File.Delete(fullPath);
                    }
                    catch { }

                    Images.Remove(img);
                }
            }
        }

        private async void Button_Click_1(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string description = DescriptionBox.Text?.Trim() ?? "";
            bool isPriceValid = decimal.TryParse(PriceBox.Text, out var price);
            bool isQuantityValid = int.TryParse(QuantityBox.Text, out var quantity);
            var selectedBrand = BrandComboBox.SelectedItem as Brand;
            var selectedType = TypeComboBox.SelectedItem as InstrumentsType;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) ||
                !isPriceValid || price < 0 || !isQuantityValid || quantity < 0 ||
                selectedBrand == null || selectedType == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Проверьте корректность введённых данных").ShowAsync();
                return;
            }

            using var dbContext = new User9Context();

            var instrument = dbContext.Instruments.FirstOrDefault(i => i.InstrumentId == _product!.InstrumentId);
            if (instrument != null)
            {
                instrument.InstrumentName = name;
                instrument.InstrumentDescription = description;
                instrument.InstrumentPrice = price;
                instrument.InstrumentQuantity = quantity;
                instrument.BrandId = selectedBrand.BrandId;
                instrument.TypeId = selectedType.TypeId;
            }

            await dbContext.SaveChangesAsync();

            // Обновляем локальный объект
            _product.InstrumentName = name;
            _product.InstrumentDescription = description;
            _product.InstrumentPrice = price;
            _product.InstrumentQuantity = quantity;
            _product.BrandId = selectedBrand.BrandId;
            _product.TypeId = selectedType.TypeId;

            await MessageBoxManager.GetMessageBoxStandard("Успешно", "Товар сохранён.").ShowAsync();

            LoadImages();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            // Если товар был создан пустым (добавление), удаляем его из БД
            if (!_isEditMode && _product != null)
            {
                using var db = new User9Context();
                var inst = db.Instruments.Find(_product.InstrumentId);
                if (inst != null)
                {
                    db.Instruments.Remove(inst);
                    db.SaveChanges();
                }
            }

            var main = new MainAppWindow();
            main.Show();
            this.Close();
        }
    }
}
