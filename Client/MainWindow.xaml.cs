using Client.Models;
using Client.Services;
using System.Windows;
using System.Configuration;
using System;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string captionWarning = "Warning";
        private string oldName;
        private CardService cardService;

        public MainWindow()
        {
            InitializeComponent();
            cardService = new CardService(ConfigurationManager.ConnectionStrings["localhost"].ConnectionString);
            DataContext = new ViewModel(cardService.LoadAPI());
            dataGrid1.ItemsSource = cardService.LoadAPI();
        }

        private void Warning(string messageBoxText)
        {
            MessageBox.Show(messageBoxText,
                captionWarning,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes);
        }

        private void btnReloadCard_Click1(object sender, RoutedEventArgs e)
        {
            dataGrid1.ItemsSource = cardService.LoadAPI();
        }

        private void btnAddCard_Click1(object sender, RoutedEventArgs e)
        {
            AddMenu.Visibility = Visibility.Visible;
        }

        private void btnDeleteCard_Click1(object sender, RoutedEventArgs e)
        {
            var items = dataGrid1.SelectedItems;
            if (items.Count == 0)
            {
                Warning("No one selected!");
                return;
            }
            try
            {
                cardService.DeleteCard(items);
                dataGrid1.ItemsSource = cardService.LoadAPI();
            }
            catch (Exception ex)
            {
                Warning(ex.Message);
            }
        }

        private void btnUpdateCard_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid1.SelectedItem;
            if (item is null)
            {
                Warning("No one selected!");
                return;
            }
            if (item is ViewCard card)
            {
                oldName = card.Name;
                UpdateMenu.Visibility = Visibility.Visible;
                updateTextBox.Text = card.Name;
                imgPhotoUpdate.Source = new BitmapImage(new Uri(card.Image));
            }
        }

        private void btnAddMenuLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "Select a picture";
            op.Filter = "Image Files (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void btnAddMenuClose_Click(object sender, RoutedEventArgs e)
        {
            AddMenu.Visibility = Visibility.Hidden;
        }

        private void btnAddMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            if (addTextBox.Text == string.Empty)
            {
                Warning("Text box is empty!");
                return;
            }

            if (imgPhoto.Source is null)
            {
                Warning("Download image!");
                return;
            }

            try
            {
                cardService.AddCard(addTextBox.Text, new Uri(imgPhoto.Source.ToString()).AbsolutePath);
                AddMenu.Visibility = Visibility.Hidden;
                dataGrid1.ItemsSource = cardService.LoadAPI();
            }
            catch (Exception ex)
            {
                Warning(ex.Message);
            }
        }

        private void btnUpdateMenuImageLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a picture";
            openFileDialog.Filter = "Image Files (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                imgPhotoUpdate.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void btnUpdateMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            if (updateTextBox.Text == string.Empty)
            {
                Warning("Text box is empty!");
                return;
            }

            if (imgPhotoUpdate.Source is null)
            {
                Warning("Download image!");
                return;
            }

            var card = cardService.GetCard(oldName);

            if (card.Image == imgPhotoUpdate.Source.ToString())
            {
                if (oldName == updateTextBox.Text)
                {
                    Warning("Name and image is same!");
                    return;
                }
                cardService.UpdateCard(updateTextBox.Text, oldName);

            }
            else
            {
                cardService.UpdateCard(updateTextBox.Text,
                    new Uri(imgPhotoUpdate.Source.ToString()).AbsolutePath, oldName);
            }
            UpdateMenu.Visibility = Visibility.Hidden;
            dataGrid1.ItemsSource = cardService.LoadAPI();
            updateTextBox.Text = string.Empty;
        }

        private void btnUpdateMenuClose_CLick(object sender, RoutedEventArgs e)
        {
            UpdateMenu.Visibility = Visibility.Hidden;
        }
    }
}
