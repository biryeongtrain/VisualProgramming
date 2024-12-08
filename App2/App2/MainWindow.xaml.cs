using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using App2.Logic;
using App2.Views;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static bool backendEnabled = false;
        private static readonly ConcurrentQueue<(string filePath, string content, bool isSave)> FileOperationQueue =
            new ConcurrentQueue<(string filePath, string content, bool isSave)>();
        private static readonly AutoResetEvent FileOperationEvent = new AutoResetEvent(false);
        private static readonly CancellationTokenSource IoWorkerCancellationTokenSource = new CancellationTokenSource();
        public static readonly String BookFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books");
        public static readonly string FeedBacks = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "feedbacks");
        public static ElementTheme Theme = ElementTheme.Default;
        public static Thread IoWorkerThread;
        public static readonly JsonSerializerOptions Options
            = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        };
        
        public static Dictionary<string, BookData> books = new Dictionary<string, BookData>();

        public static Dictionary<string, List<BookFeedbackInstance>> BookFeedbackDictionary =
            new Dictionary<string, List<BookFeedbackInstance>>();

        public static MainWindow WINDOW { get; private set; }

        private string path;

        public MainWindow()
        {
            this.InitializeComponent();
            this.SetMaterial();
            this.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
            WINDOW = this;

            if (Directory.Exists(BookFolder))
            {
                foreach (var file in Directory.GetFiles(BookFolder))
                {
                    Debug.Print(file);
                    LoadFile(file);
                }
            }
            else
            {
                Directory.CreateDirectory(BookFolder);
            }

            if (Directory.Exists(FeedBacks))
            {
                foreach (var VARIABLE in Directory.GetFiles(FeedBacks))
                {
                    LoadFile(VARIABLE);
                }
            }


            this.ContentFrame.Navigate(Type.GetType("App2.Views.MainPage"), null,
                new DrillInNavigationTransitionInfo());

            IoWorkerThread = new Thread(IoWorker);
            IoWorkerThread.Start();
            this.MenuNavigation.IsBackEnabled = true;
            Debug.Print("Registered Books : " + books.Count);
            Debug.Print("Registered Isbns : " + BookFeedbackDictionary.Count);
        }
        /**
         * 프로그램의 기본 테마를 변경합니다.
         * Windows 11은 MicaAlt 테마를 사용하며, Windows 10 이하 버전은 Acrylic 테마를 사용합니다.
         */
        private void SetMaterial()
        {
            Debug.Print("Loading BackDrops");
            SystemBackdrop backDrop = MicaController.IsSupported()
                ? new MicaBackdrop() { Kind = MicaKind.BaseAlt }
                : new DesktopAcrylicBackdrop();

            this.SystemBackdrop = backDrop;
        }

        /**
         * 파일 입출력을 담당하는 Worker Thread의 사용 메소드입니다.
         */
        private static void IoWorker()
        {
            while (!IoWorkerCancellationTokenSource.Token.IsCancellationRequested)
            {
                FileOperationEvent.WaitOne();
                while (FileOperationQueue.TryDequeue(out var fileOperationTask))
                {
                    if (fileOperationTask.isSave)
                    {
                        SaveFile(fileOperationTask.filePath, fileOperationTask.content);
                    }
                    else
                    {
                        LoadFile(fileOperationTask.filePath);
                    }
                }
            }
        }

        /**
         * 특정 파일의 내용을 지정된 경로로 저장합니다.
         */
        private static void SaveFile(string filePath, String content)
        {
            try
            {
                if (filePath.Contains(BookFolder))
                {
                    if (!Directory.Exists(BookFolder))
                    {
                        Directory.CreateDirectory(BookFolder);
                    }
                } else if (filePath.Contains(FeedBacks))
                {
                    if (!Directory.Exists(FeedBacks))
                    {
                        Directory.CreateDirectory(FeedBacks);
                    }
                }
                using StreamWriter writer = new StreamWriter(filePath);
                writer.Write(content);
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving file: {ex.Message}");
            }
        }

        /**
         * 특정 파일의 내용을 읽어옵니다.
         */
        private static void LoadFile(string filePath)
        {
            try
            {
                using StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                if (filePath.Contains(BookFolder))
                {
                    if (!Directory.Exists(BookFolder))
                    {
                        return;
                    }

                    if (filePath.Contains(BookFolder))
                    {
                        BookData data = JsonSerializer.Deserialize<BookData>(content, Options);

                        Debug.Print($"File loaded: {content}");
                        backendEnabled = true;
                        books.Add(data.Isbn, data);
                    } 
                }
                else if (filePath.Contains(FeedBacks))
                {

                    BookFeedbackInstance instance = JsonSerializer.Deserialize<BookFeedbackInstance>(content, Options);
                    if (BookFeedbackDictionary.TryGetValue(instance.Isbn, out List<BookFeedbackInstance>? value))
                    {
                        value.Add(instance);
                    }
                    else
                    {
                        BookFeedbackDictionary.Add(instance.Isbn, [instance]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error loading file: {ex.Message}");
            }
        }

        /**
         * 파일 저장을 요청합니다. UI 스레드에서는 직접적으로 저장을 하는 것을 고려하지 않기 때문에 IO Worker Thread에게 저장을 요청합니다.
         */
        public static void QueueFileSave(string filePath, Object content)
        {
            var value = JsonSerializer.Serialize(content, Options);
            FileOperationQueue.Enqueue((filePath, value, true));
            FileOperationEvent.Set();
        }

        /**
         * 파일 로드를 요청합니다. 대부분의 경우 로드는 초반에만 고려됩니다. 
         */
        public static void QueueFileLoad(string filePath)
        {
            FileOperationQueue.Enqueue((filePath, string.Empty, false));
            FileOperationEvent.Set();
        }

        /**
         * NavigationView 메뉴에 있는 요소는 각 태그를 가지고 있습니다.
         * 해당 태그는 버튼을 눌렀을 때에 프레임이 어느 페이지로 이동하는지를 정의합니다.
         */
        private void MenuNavigation_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                this.ContentFrame.Navigate(typeof(SettingsPage), null, new DrillInNavigationTransitionInfo());
                return;
            }

            String selectedItem = ((NavigationViewItem)args.SelectedItem).Tag.ToString();
            if (selectedItem == "MainPage")
            {
                Type page = Type.GetType("App2.Views." + selectedItem);
                this.ContentFrame.Navigate(page, null, new DrillInNavigationTransitionInfo());
                return;
            }
            else if (selectedItem == "AddBookPage" || selectedItem == "BookInfoPage")

            {
                Type page = Type.GetType("App2.Views." + selectedItem);
                this.ContentFrame.Navigate(page, null, new DrillInNavigationTransitionInfo());
                return;
            }

            Type type = Type.GetType("App2.Views.SimplePage");
            Debug.Assert(type != null);
            this.ContentFrame.Navigate(type, null, new DrillInNavigationTransitionInfo());
        }

        /**
         * 테스트용 버튼이며, 현재는 사용되지 않습니다.
         * 해당 메소드는 직접 개발한 기록을 남기기 위하여 보존하였습니다.
         */
        private async void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            var popUp = new AddMemberPopup();
            var xaml = this.ContentFrame.XamlRoot;
            ContentDialog p = new ContentDialog()
            {
                XamlRoot = xaml,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = this.path,
                Content = popUp
            };
            var result = p.ShowAsync();
        }

        /**
         * 워커 스레드와 작업 큐를 종료합니다. 해당 부분이 누락되어 있을 경우, 윈도우만 닫히고 스레드는 동작함으로 꼭 닫아야합니다.
         */
        private void MainWindow_OnClosed(object sender, WindowEventArgs args)
        {
            IoWorkerCancellationTokenSource.Cancel();
            FileOperationEvent.Set();
            IoWorkerThread.Join();
        }

        /**
         * 프레임에서 표시하는 페이지를 변경합니다. 파라미터를 따로 전달하지 않을 경우 사용됩니다.
         */
        public void Navigate(Type type)
        {
            this.Navigate(type, null);
        }

        /**
         * 파라미터를 같이 넘겨줘야할 때 사용됩니다. 
         */
        public void Navigate(Type type, object? parameter)
        {
            this.ContentFrame.BackStack.Add(new PageStackEntry(this.ContentFrame.SourcePageType, null,
                new DrillInNavigationTransitionInfo()));
            this.ContentFrame.Navigate(type, parameter, new DrillInNavigationTransitionInfo());
        }

        /**
         * NavigationView를 반환합니다. 원래는 NavigationView를 받아 직접 Navigate 시키는 방식을 사용했으나, 지금은 사용하지 않습니다.
         */
        public NavigationView GetNavigationView()
        {
            return this.MenuNavigation;
        }

        /**
         * 뒤로 가기 버튼을 누를 때 행동입니다.
         */
        private void MenuNavigation_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(this.ContentFrame.CanGoBack)
            {
                this.ContentFrame.GoBack();
            }
        }

        public bool RequestGoBack()
        {
            if (this.ContentFrame.CanGoBack)
            {
                this.ContentFrame.GoBack();
                return true;
            }

            return false;
        }
    }
}