using System;
using System.Windows;

namespace Netx.Dialog
{
    /// <summary>
    /// MessageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MessageDialog : Window
    {
        private MessageDialog()
        {
            InitializeComponent();
           
        }


        public static void Show(Window window, string message, Action<MessageDialog> action = null, bool showCancel = false, string btnName = "我知道了")
        {
            var dialog = new MessageDialog();
            if (window == null)
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.Owner = window;
            }

            dialog.TextMessage.Text = message;
            dialog.BtnOK.Content = btnName;
            dialog.BtnCancel.Visibility = showCancel ? Visibility.Visible : Visibility.Collapsed;
            dialog.BtnCancel.Click += (sender, e) =>
            {
                dialog.Close();
            };
            dialog.BtnOK.Click += (sender, e) =>
            {
                dialog.Close();
                action?.Invoke(dialog);
            };
            dialog.ShowDialog();
        }
    }
}
