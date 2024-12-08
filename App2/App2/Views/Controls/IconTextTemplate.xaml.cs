// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using App2.Logic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;

namespace App2.Views.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IconTextTemplate
    {
        public string RichContents
        {
            get { return (string)GetValue(_content); }
            set { SetValue(_content, value); }
        }

        public static readonly DependencyProperty _content = DependencyProperty.Register(nameof(RichContents), typeof(string),
            typeof(IconTextTemplate), new PropertyMetadata(""));

        public IconTextTemplate()
        {
            this.InitializeComponent();
        }


        private string ConvertToXaml(string rtf)
        {
            return $"<Paragraph>{rtf}</Paragraph>";
        }

        private void TextBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            var display = this.test;
            if (display != null)
            {
                var content = display.LoadContent() as FrameworkElement;
                if (content != null)
                {
                    // if (content.DataContext is not BookFeedbackInstance feedBack)
                    // {
                    //     return;
                    // }
                    return;
                    var dummyTextBlock = content.FindName("Dummy");
                    if (dummyTextBlock is TextBlock block)
                    {
                        var xaml = ConvertToXaml(block.Text);
                        var paragraph = (Paragraph)XamlReader.Load(xaml);
                        var textBlock = content.FindName("textBlock") as RichTextBlock;
                        if (textBlock != null)
                        {
                            textBlock.Blocks.Add(paragraph);
                        }
                    }
                }
            }

        }



        private void Dummy_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            
        }
    }
}