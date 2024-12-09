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
    /// 책 추가 페이지. 크로니움 기반의 WebView 컴포넌트와 트리거용 버튼 2개만 존재함.
    /// </summary>
    public sealed partial class AddBookPage : Page
    {
        public AddBookPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// 버튼을 눌렀을 때 URL 을 체크하고 의도한 URL이면 크롤링을 시도함. <br></br>
        /// 받는 정보는 페이지의 metadata 인 isbn, title, image 등
        /// </summary>
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
                // js 명령어를 강제로 실행시켜서 결과값을 받음.
                var task = await this.KyoboViewer.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.outerHTML;");
                ;
                var html = task;

                // 사용 가능한 HTML 양식으로 되게끔 설정
                html = Regex.Unescape(html);
                html = html.Remove(0, 1);
                html = html.Remove(html.Length - 1, 1);

                // HtmlAgilityPack 의 HtmlDocument 인스턴스 생성
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var node = doc.DocumentNode;

                // html meta를 가져옴
                var isbn = node.SelectSingleNode("//meta[@property='books:isbn']")
                    .GetAttributeValue("content", "");

                var title = node.SelectSingleNode("//meta[@property='eg:itemName']")
                    .GetAttributeValue("content", "");
                var page = node.SelectSingleNode("//th[text()='쪽수']/following-sibling::td");

                double rating = double.Parse(node.SelectSingleNode("//meta[@property='books:rating:value']")
                    .GetAttributeValue("content", ""));

                var desc = node.SelectSingleNode("//meta[@name='twitter:description']")
                    .GetAttributeValue("content", "");

                // 지원 예정 or 정보 없음이 들어가는 경우도 존재함. 이 경우에는 0으로 설정
                int Ipage;
                Ipage = int.TryParse(page.InnerText.Trim().Replace("쪽", ""), out Ipage) ? Ipage : 0;

                // 파싱한 데이터를 토대로 인스턴스 생성
                BookData data = new BookData(rating, MainPage.ImageFormat.Replace("%format%", isbn), title, isbn,desc, 0,
                    Ipage);


                // 메인 딕셔너리에 파일 추가 및 Io Thread 에 저장 요청
                MainWindow.books.Add(data.Isbn, data);
                MainWindow.QueueFileSave(Path.Combine(MainWindow.BookFolder, isbn+".json"), data);
                
                // 팝업 생성
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

            // 예외 핸들링. 다만 어떤 예외가 나올지는 모르기때문에 Exception 으로 처리
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