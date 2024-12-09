using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views
{
    /**
     * 세팅 페이지는 현재 테마만 조정 가능합니다. 자세한 내용은 SettingsPage.xaml을 참조하세요.
     */
    public sealed partial class SettingsPage : Page
    {
        // 저장 루트 폴더 지정
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        public static DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(SettingsPage), new PropertyMetadata(":)"));
        private bool _initialized = false;
        public SettingsPage()
        {
            this.InitializeComponent();
            this.themeMode.SelectedIndex = 2;
            this.Path = MainWindow.BookFolder;
        }

        // 테마 선택버튼의 요소가 변경되었을 때 발생하는 이벤트 정의
        private void ThemeMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
            {
                _initialized = true;
                return;
            }
            var selectedTheme = ((ComboBoxItem)themeMode.SelectedItem)?.Tag?.ToString();
            MainWindow.Theme = App.GetEnum<ElementTheme>(selectedTheme);
            ((FrameworkElement)MainWindow.WINDOW.Content).RequestedTheme = MainWindow.Theme;

        }
    }
}
