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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2.Views.Dialogue
{
    /// <summary>
    /// 중요한 페이지는 아님. 그저 에러 내용을 표시하는 페이지임.
    /// </summary>
    public sealed partial class InvalidLinkDialog : Page
    {
        public InvalidLinkDialog()
        {
            this.InitializeComponent();
        }
    }
}
