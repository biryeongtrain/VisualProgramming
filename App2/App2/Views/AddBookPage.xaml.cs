using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using App2.Logic;
using App2.Views.Dialogue;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddBookPage : Page
    {
        public AddBookPage()
        {
            this.InitializeComponent();
        }


        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.KyoboViewer.Source.AbsoluteUri.Contains("https://product.kyobobook.co.kr/detail/"))
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "주소 에러!",
                    Content = new InvalidLinkDialog(),
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close,
                    RequestedTheme = MainWindow.Theme,
                };
                dialog.ShowAsync();
                return;
            }

            try
            {
                var task = await this.KyoboViewer.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.outerHTML;");
                ;
                var html = task;
                html = Regex.Unescape(html);
                html = html.Remove(0, 1);
                html = html.Remove(html.Length - 1, 1);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var node = doc.DocumentNode;
                var isbn = node.SelectSingleNode("//meta[@property='books:isbn']")
                    .GetAttributeValue("content", "");

                var title = node.SelectSingleNode("//meta[@property='eg:itemName']")
                    .GetAttributeValue("content", "");
                var page = node.SelectSingleNode("//th[text()='쪽수']/following-sibling::td");

                double rating = double.Parse(node.SelectSingleNode("//meta[@property='books:rating:value']")
                    .GetAttributeValue("content", ""));

                var desc = node.SelectSingleNode("//meta[@name='twitter:description']")
                    .GetAttributeValue("content", "");

                int Ipage;
                Ipage = int.TryParse(page.InnerText.Trim().Replace("쪽", ""), out Ipage) ? Ipage : 0;

                BookData data = new BookData(rating, MainPage.ImageFormat.Replace("%format%", isbn), title, isbn,desc, 0,
                    Ipage);



                MainWindow.books.Add(data.Isbn, data);
                MainWindow.QueueFileSave(Path.Combine(MainWindow.BookFolder, isbn+".json"), data);
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "불러오기 성공!",
                    Content = new SaveSuccessDialog(),
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close,
                    RequestedTheme = MainWindow.Theme,
                };
                var result = await successDialog.ShowAsync();
            }
            catch (Exception exception)
            {
                ContentDialog failDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "오류 발생!",
                    Content = new LoadErrorPopUp
                    {
                        Message = exception.Message
                    },
                    DefaultButton = ContentDialogButton.Close,
                    RequestedTheme = MainWindow.Theme
                };
                var result = await failDialog.ShowAsync();
            }
            MainWindow.WINDOW.Navigate(typeof(MainPage));
        }
    }
}