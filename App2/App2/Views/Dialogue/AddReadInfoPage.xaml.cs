using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Popups;
using App2.Logic;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views.Dialogue
{
/// <summary>
/// 책을 읽은 정보를 추가하는 내역. 원래는 페이지와 그에 따른 동작도 바뀌어야 하나,
/// DataTemplate DataContext 참조 문제로 인해 시간이 부족하여 제작하지 못함. <br></br>
/// KNOWN ISSUE : 어두운 테마일 때 텍스트 컬러가 제대로 적용되지 않음.
/// </summary>
    public sealed partial class AddReadInfoPage : Page
    {
        private BookFeedbackInstance Instance { get; set; }
        private string? Isbn { get; set; }

        public AddReadInfoPage()
        {
            this.InitializeComponent();
            this.SetDefaultTime();
        }

        /**
         * 기본 시간을 설정합니다. 기본 시간은 현재 시간으로 설정됩니다.
         */
        private void SetDefaultTime()
        {
            var time = System.DateTime.Now;
            var timeSpan = new TimeSpan(time.Hour, time.Minute, time.Second);
            this.StartDate.Date = time;
            this.EndDate.Date = time;
            this.StartTime.Time = timeSpan;
            this.EndTime.Time = timeSpan;
        }

        /**
         * 텍스트의 색을 즉시 변경
         */
        private void Editor_OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (Editor.IsReadOnly)
            {
                return;
            }
            // 색이 변하지 않는 이슈가 있는데, CommunityToolkit에 있는 ColorPalette 의 문제로 확인됨. 
            // 일단 Issue 를 넣어둔 상태인데, 지금당장 해결 할 방법은 없어보임.
            Editor.Document.Selection.CharacterFormat.ForegroundColor = FontColorButton.SelectedColor;
           
        }

        /**
         * 텍스트 에디트 하이라이트 부분의 색을 설정한 색으로 변경합니다.
         */
        private void Editor_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Editor.Document.GetText(TextGetOptions.UseCrlf, out _);

            ITextRange docuRange = Editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush backBrush = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

            if (backBrush != null)
            {
                if (Editor.IsReadOnly)
                {
                    return;
                }
                docuRange.CharacterFormat.BackgroundColor = backBrush.Color;
            }
        }

        private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.Editor.IsReadOnly)
            {
                MainWindow.WINDOW.RequestGoBack();
                return;
            }
            if (String.IsNullOrEmpty(this.TitleBox.Text) || this.StartDate.Date == null || this.StartTime == null || this.EndTime == null)
            {
                ContentDialog errorBox = new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "입력 오류",
                    Content = "제목, 시작 날짜, 시작 시간, 끝 시간은 필수 입력 사항입니다.",
                    CloseButtonText = "확인",
                    DefaultButton = ContentDialogButton.Close
                };
                await errorBox.ShowAsync();
                return;
            }
            this.Editor.Document.GetText(TextGetOptions.FormatRtf, out string? output);
            var startDate = this.StartDate.Date.Value.Date;
            startDate = startDate.Add(this.StartTime.Time);
            var endDate = this.EndDate.Date.Value.Date;
            endDate = endDate.Add(this.EndTime.Time);
            BookFeedbackInstance newInstance = new BookFeedbackInstance(this.TitleBox.Text, this.Isbn, output,  startDate, endDate, 0);
            int index;
            if (MainWindow.BookFeedbackDictionary.TryGetValue(Isbn, out List<BookFeedbackInstance>? list))
            {
                if (list.Contains(this.Instance))
                {
                    index = list.IndexOf(this.Instance) - 1;
                    list.Remove(this.Instance);
                    list.Insert(index, newInstance);
                }
                list.Add(newInstance);
                index = list.Count - 1;
            }
            else
            {
                var list2 = new List<BookFeedbackInstance>
                {
                    newInstance
                };
                MainWindow.BookFeedbackDictionary.Add(Isbn, list2);
                index = 0;
            }

            string path = Path.Combine(MainWindow.FeedBacks, Isbn + "_" + index + ".json");
            MainWindow.QueueFileSave(path, newInstance);
            ContentDialog acceptBox = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Title = "저장 완료!",
                Content = "저장이 완료되었습니다 :)",
                CloseButtonText = "확인",
                DefaultButton = ContentDialogButton.Close
            };
            await acceptBox.ShowAsync();
            MainWindow.WINDOW.RequestGoBack();
            Debug.Print(output);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null)
            {
                Debug.Print("ERROR! NO CONTENT WITH NAVIGATION!!!");
               MessageDialog errorBox = new MessageDialog("알 수 없는 접근입니다! 메인으로 돌아갑니다.");
                await errorBox.ShowAsync();

                MainWindow.WINDOW.Navigate(typeof(MainPage));
                return;
            }
            if (e.Parameter is string parameter)
            {
                Isbn = parameter;
                return;
            }

            if (e.Parameter is BookFeedbackInstance instance)
            {
                this.Isbn = Isbn;
                this.Editor.Document.SetText(TextSetOptions.FormatRtf, instance.Description);
                this.TitleBox.Text = instance.Title;
                this.Rating.Value = instance.Rating;
                this.StartDate.Date = instance.StartTime;
                this.EndDate.Date = instance.EndTime;
                this.StartTime.Time = instance.GetStartTimeSpan();
                this.EndTime.Time = instance.GetEndTimeSpan();
                this.Instance = instance;

                // LOCK ALL BOXES
                this.Editor.IsReadOnly = true;
                this.TitleBox.IsReadOnly = true;
                this.Rating.IsReadOnly = true;
                this.StartDate.IsEnabled = false;
                this.EndDate.IsEnabled = false;
                this.StartTime.IsEnabled = false;
                this.EndTime.IsEnabled = false;
                // TODO : read pages :)
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.WINDOW.RequestGoBack();  
        }
    }
}