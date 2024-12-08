using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App2.Logic;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using App2.Views.Dialogue;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        private BookData? _data;

        public float ReadPercent
        {
            get => (float)GetValue(ReadPercentProperty);
            set => SetValue(ReadPercentProperty, value);
        }

        public static DependencyProperty ReadPercentProperty =
            DependencyProperty.Register(nameof(ReadPercent), typeof(float), typeof(BookInfoPage),
                new PropertyMetadata(0f));

        public BookInfoPage()
        {
            this.InitializeComponent();
        }

        public string Isbn
        {
            get => (string)GetValue(IsbnProperty);
            set => SetValue(IsbnProperty, value);
        }

        public static DependencyProperty IsbnProperty =
            DependencyProperty.Register("Isbn", typeof(string), typeof(BookInfoPage), new PropertyMetadata("Unknown"));

        public string Rating
        {
            get => GetValue(RatingProperty).ToString();
            set => SetValue(RatingProperty, value);
        }

        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(float), typeof(Controls.BooksControl),
                new PropertyMetadata(0));

        private void BookInfoPage_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null || !(e.Parameter.GetType() == typeof(BookData)))
            {
                Debug.Print("Error, Args not Found.");
                var sourceType = e.SourcePageType;
                // MainWindow.WINDOW.Navigate(sourceType);
                return;
            }

            this._data = (BookData)e.Parameter;
            this.Title.Text = _data.BookTitle;
            this.SourceImage.Source = new BitmapImage(new Uri(_data.BookIconPath));
            this.Isbn = _data.Isbn;
            this.Rating = _data.Rating.ToString();
            this.ReadPercent = _data.TotalPages == 0 ? 0 : (float)_data.CurrentPage / _data.TotalPages;

            ObservableCollection<BookFeedbackInstance> items2 =
                new ObservableCollection<BookFeedbackInstance>(MainWindow.BookFeedbackDictionary.TryGetValue(Isbn, out var value) ? value : []);
            this.RecordGridView.ItemsSource = items2;
        }
            
        private void AddReadInfoButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.WINDOW.Navigate(typeof(AddReadInfoPage), this.Isbn);
        }


        private void RecordGridView_OnGettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
        }

        private void RecordGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.Print("CLicked");
            BookFeedbackInstance instance = (BookFeedbackInstance)e.ClickedItem;
            MainWindow.WINDOW.Navigate(typeof(AddReadInfoPage), instance);
        }
    }
}