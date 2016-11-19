using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestImage;

namespace VerificationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        OCRContext context = new OCRContext();
        private void FormLoad(object sender, RoutedEventArgs e)
        {
            Next();
        }

        public void Next()
        {
            var value = context.Values.FirstOrDefault(f => !f.Verified);
            if (value != null)
            {
                if (!System.IO.File.Exists(value.Location))
                {
                    context.Values.Remove(value);
                    context.SaveChanges();
                    Next();
                    return;
                }
                this.textBox.Text = value.HashText.Trim();
                this.textBox1.Text = value.Value.Trim();
                BitmapImage i = new BitmapImage();//虽然点了删除，可是图片还显示在窗体里，这时要想重新刷新，给image控件赋值一个空的图片
                i.BeginInit();
                i.StreamSource = System.IO.File.OpenRead(value.Location);
                i.EndInit();
                imageLable.Source = i;
            }
            this.button1.IsEnabled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var value = context.Values.FirstOrDefault(f => f.HashText == this.textBox.Text.Trim());

            value.Verified = true;
            context.SaveChanges();

            Next();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var value = context.Values.FirstOrDefault(f => f.HashText == this.textBox.Text.Trim());

            value.Value = this.textBox1.Text.Trim();
            value.Verified = true;
            context.SaveChanges();
            Next();
        }

        private void TextFocused(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var value = context.Values.FirstOrDefault(f => f.HashText == this.textBox.Text.Trim());
            context.Values.Remove(value);
            context.SaveChanges();
            Next();
        }

        private void press(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Click(sender, e);
            }
            if (e.Key == Key.F12)
            {
                button1_Click(sender, e);
            }
            if (e.Key == Key.F1)
            {
                button2_Click(sender, e);
            }
        }
    }
}
