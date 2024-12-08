using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views.Controls
{
    public sealed partial class BooksTile : UserControl
    {
        public BooksTile()
        {
            this.InitializeComponent();
            if (this.BooksPanel.Children.Count == 1)
            {
                this.EmptyStackPanelAlertBorder.Visibility = Visibility.Visible;
                this.NoBooksAlertText.HorizontalTextAlignment = TextAlignment.Center;
                this.BooksPanel.CanBeScrollAnchor = false;
            }
        }

        public Grid GetEmptyAlertGrid()
        {
            return this.EmptyStackPanelAlertBorder;
        }
        private void Scroller_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            UpdateScrollButtonsVisibility();

            this.EmptyStackPanelAlertBorder.Width = e.NewSize.Width * 0.95;
            this.EmptyStackPanelAlertBorder.Height = e.NewSize.Height;
        }

        public StackPanel getBooksPanel()
        {
            return this.BooksPanel;
        }

        private void Scroller_OnViewChanging(object? sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.FinalView.HorizontalOffset < 1)
            {
                ScrollBackBtn.Visibility = Visibility.Collapsed;
            }
            else if (e.FinalView.HorizontalOffset > 1)
            {
                ScrollBackBtn.Visibility = Visibility.Visible;
            }

            if (e.FinalView.HorizontalOffset > scroller.ScrollableWidth - 1)
            {
                ScrollForwardBtn.Visibility = Visibility.Collapsed;
            }
            else if (e.FinalView.HorizontalOffset < scroller.ScrollableWidth - 1)
            {
                ScrollForwardBtn.Visibility = Visibility.Visible;
            }
        }

        private void ScrollBackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            scroller.ChangeView(scroller.HorizontalOffset - scroller.ViewportWidth, null, null);
            ScrollBackBtn.Focus(FocusState.Programmatic);

        }

        private void ScrollForwardBtn_OnClick(object sender, RoutedEventArgs e)
        {
            scroller.ChangeView(scroller.HorizontalOffset + scroller.ViewportWidth, null, null);
            ScrollForwardBtn.Focus(FocusState.Programmatic);
        }
        private void UpdateScrollButtonsVisibility()
        {
            if (scroller.ScrollableWidth > 0)
            {
                ScrollForwardBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ScrollForwardBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNewBook_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.WINDOW.Navigate(typeof(AddBookPage));
        }
    }
}
