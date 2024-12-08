using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views.Controls
{
    public sealed partial class BooksControl : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BooksControl), new PropertyMetadata(null));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public string Rating
        {
            get {return GetValue(RatingProperty).ToString();
            }
            set {SetValue(RatingProperty, value);}
        }

        public float ReadPercent
        {
            get => (float)GetValue(ReadPercentProperty);
            set => SetValue(ReadPercentProperty, value);
        }

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string Isbn
        {
            get;
            set;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(BooksControl), new PropertyMetadata(null));
        
        // 원래는 교보문고로 리다이렉션 시키려 했으나 취소.
        // public string Link
        // {
        //     get { return (string)GetValue(LinkProperty); }
        //     set { SetValue(LinkProperty, value); }
        // }

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(BooksControl), new PropertyMetadata(null));
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(float), typeof(BooksControl), new PropertyMetadata(0));
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(BooksControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ReadPercentProperty = 
            DependencyProperty.Register("ReadPercent", typeof(float), typeof(BooksControl), new PropertyMetadata(0));
        public BooksControl()
        {
            this.InitializeComponent();
        }

        private void BookControlButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.WINDOW.Navigate(typeof(BookInfoPage), MainWindow.books[Isbn]);
        }
    }
}
