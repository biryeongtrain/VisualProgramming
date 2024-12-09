using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using App2.Logic;
using System.Collections.ObjectModel;
using Windows.UI.StartScreen;
using System.Linq;
using App2.Views.Dialogue;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views
{
    /// <summary>
    /// 메인 페이지에 대한 클래스
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static readonly string ImageFormat =
            "https://contents.kyobobook.co.kr/sih/fit-in/458x0/pdt/%format%.jpg";

        public static readonly string HyperLinkFormat =
            "http://www.kyobobook.co.kr/product/detailViewKor.laf?ejkGb=KOR&mallGb=KOR&barcode=9788936434120&orderClick=&Kc=";

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Load");

            // 등록된 책이 없으면 나오는 알림
            if (MainWindow.books.Count == 0)
            {
                this.BooksHeaderTile.GetEmptyAlertGrid().Visibility = Visibility.Visible;
            }
            else
            {
                this.BooksHeaderTile.GetEmptyAlertGrid().Visibility = Visibility.Collapsed;
                foreach (var bookPair in MainWindow.books)
                {
                    var book = bookPair.Value;
                    var control = book.CreateComponent();
                    this.AddPanelElement(control);
                }
            }

            // 하단에 보여줄 인스턴스 목록을 설정함.
            var instances = MainWindow.BookFeedbackDictionary.Values.SelectMany(list => list).ToList();
            ObservableCollection<BookFeedbackInstance> items2 = new ObservableCollection<BookFeedbackInstance>(instances);
            this.RecordGridView.ItemsSource = items2;
        }


        // NO-OP
        private void ItemGridView_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            
        }

        // NO-OP
        private void ItemGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
        }


        // 디버깅용으로 만든 버튼에 사용된 이벤트. 원래는 지워야하는데 남겼음.
        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            var control = new Controls.BooksControl
            {
                Title = "Hello World!",
                Description = "Sexy Description",
                Source = new BitmapIcon(),
                Rating = "4.5"
            };
            this.AddPanelElement(control);
        }

        // 패널 추가
        private void AddPanelElement(Controls.BooksControl control)
        {
            if (this.BooksHeaderTile.GetEmptyAlertGrid().Visibility == Visibility.Visible)
            {
                this.BooksHeaderTile.GetEmptyAlertGrid().Visibility = Visibility.Collapsed;
            }
            this.BooksHeaderTile.getBooksPanel().Children.Add(control);
        }

        /// <summary>
        /// 읽은 정보로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            MainWindow.WINDOW.Navigate(typeof(AddReadInfoPage), e.ClickedItem);
        }
    }
}
