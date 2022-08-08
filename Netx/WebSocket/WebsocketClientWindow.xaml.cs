using Netx.Base;
using Netx.Dialog;
using Netx.Util;
using SuperSocket.ClientEngine;
using System;
using System.Windows;
using System.Windows.Input;

namespace Netx.WebSocket
{
    /// <summary>
    /// WebsocketServer.xaml 的交互逻辑
    /// </summary>
    public partial class WebsocketClientWindow : BaseWindow
    {
        private WebSocket4Net.WebSocket socketClient;
        
        private string url;
        
        private bool scrollToEnd = true;

        public WebsocketClientWindow()
        {
            InitializeComponent();
            InitEvents();
        }

        private void WebsocketServerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetIpv4AddrListHandler((addrList) =>
            {
                SelectIpAddr.ItemsSource = addrList;
            }, false);

        }

        public override void InitEvents()
        {
            Loaded += WebsocketServerWindow_Loaded;
            Closed += WebsocketClientWindow_Closed;
            
            BtnControlConn.Click += BtnControlConn_Click;
            BtnStopConn.Click += BtnStopConn_Click;
            BtnSend.Click += BtnSend_Click;
            BtnClose.Click += BtnClose_Click;
            BtnMin.Click += BtnMin_Click;
            
            GridMainTitle.PreviewMouseLeftButtonDown += GroupBoxMain_PreviewMouseLeftButtonDown;
        }

        private void WebsocketClientWindow_Closed(object sender, EventArgs e)
        {
            StopServer();
        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        private void GroupBoxMain_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            var msg = InputSendMessage.Text.ToString();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                socketClient.Send(msg);
                AppendInfo($"【发送消息】: {msg}");
            }
        }

        private void BtnStopConn_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void BtnControlConn_Click(object sender, RoutedEventArgs e)
        {
            StartToConn();
        }

        private bool InitClient()
        {
            try
            {
                url = InputAddr.Text.ToString();
                socketClient = new WebSocket4Net.WebSocket(url);
                socketClient.DataReceived += SocketClient_DataReceived;
                socketClient.MessageReceived += SocketClient_MessageReceived; ;
                socketClient.Closed += SocketClient_Closed;
                socketClient.Error += SocketClient_Error;
                socketClient.Opened += SocketClient_Opened;
                socketClient.Open();
                AppendInfo($">>正在连接[{url}]，请等待...");
            }
            catch (Exception e)
            {
                MessageDialog.Show(this, "连接初始化异常：" + e.Message);
                return false;
            }
            return true;
        }

        private void SocketClient_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            AppendInfo($"【服务消息】: {e.Message}");
        }

        private void SocketClient_Opened(object sender, EventArgs e)
        {
            AppendInfo($"【连接】: 已连接到 {url}");
            Dispatcher.Invoke(() =>
            {
                BtnControlConn.IsEnabled = false;
                BtnStopConn.IsEnabled = true;
                BtnSend.IsEnabled = true;
                InputAddr.IsEnabled = false;
            });
        }

        private void SocketClient_Error(object sender, ErrorEventArgs e)
        {
            AppendInfo($"【连接】: 服务连接错误>>{e.Exception.Message}");
        }

        private void SocketClient_Closed(object sender, EventArgs e)
        {
            AppendInfo($"【连接】: 服务连接关闭");
            RefreshState();
        }

        private void SocketClient_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            AppendInfo($"【服务消息】: SocketClient_DataReceived");
        }

        private void AppendInfo(string info)
        {
            Dispatcher.Invoke(() =>
            {
                InputInfo.AppendText($"[{CommonUtil.GetNowDateTime()}]{info}\n");
                if (scrollToEnd)
                {
                    InputInfo.ScrollToEnd();
                }
            });
        }

        private bool CheckInputPort(string port)
        {
            return int.TryParse(port, out int portInt) && (portInt <= 65500 && portInt >= 0);
        }
        private void StartToConn()
        {
            if (InitClient())
            {
                BtnControlConn.IsEnabled = false;
                BtnStopConn.IsEnabled = false;
                BtnSend.IsEnabled = false;
                InputAddr.IsEnabled = false;
            };
        }

        private void StopServer()
        {
            try
            {
                socketClient?.Close();
                socketClient?.Dispose();
                AppendInfo("【连接】: 本地断开连接");
                RefreshState();
            }
            catch (Exception e)
            {
                MessageDialog.Show(this, "断开连接异常" + e.Message);
            }
        }

        private void RefreshState()
        {
            Dispatcher.Invoke(() =>
            {
                BtnControlConn.IsEnabled = true;
                BtnStopConn.IsEnabled = false;
                BtnSend.IsEnabled = false;
                InputAddr.IsEnabled = true;
            });
        }
    }
}
