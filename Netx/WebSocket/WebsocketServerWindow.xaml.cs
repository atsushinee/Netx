using Netx.Base;
using Netx.Dialog;
using Netx.Util;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Netx.WebSocket
{
    /// <summary>
    /// WebsocketServer.xaml 的交互逻辑
    /// </summary>
    public partial class WebsocketServerWindow : BaseWindow
    {
        private WebSocketServer socketServer;
        public List<WebSocketSession> sessionList;
        public List<WebSocketSession> sendSessionList;
        private string addr;
        private int port;
        private bool scrollToEnd = true;

        public WebsocketServerWindow()
        {
            InitializeComponent();
            InitEvents();
        }

        private void WebsocketServerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetIpv4AddrListHandler((addrList) =>
            {
                SelectIpAddr.ItemsSource = addrList;
            });
        }

        public override void InitEvents()
        {
            Loaded += WebsocketServerWindow_Loaded;
            Closed += WebsocketServerWindow_Closed;
            BtnControlServer.Click += BtnControlServer_Click;
            BtnStopServer.Click += BtnStopServer_Click;
            BtnSend.Click += BtnSend_Click;
            BtnClose.Click += BtnClose_Click;
            BtnMin.Click += BtnMin_Click;
            GridMainTitle.PreviewMouseLeftButtonDown += GroupBoxMain_PreviewMouseLeftButtonDown;
        }

        private void WebsocketServerWindow_Closed(object sender, EventArgs e)
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
                socketServer.Broadcast(sendSessionList, msg, null);
                AppendInfo($"【发送】: {msg}");
            }
        }

        private void BtnStopServer_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void BtnControlServer_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private bool InitServer()
        {
            try
            {
                sessionList = new List<WebSocketSession>();
                sendSessionList = new List<WebSocketSession>();
                socketServer = new WebSocketServer();
                socketServer.NewMessageReceived += SocketServer_NewMessageReceived; ;
                socketServer.NewSessionConnected += SocketServer_NewSessionConnected;
                socketServer.SessionClosed += SocketServer_SessionClosed;
                if (socketServer.Setup(addr, port))
                {
                    if (socketServer.Start())
                    {
                        AppendInfo($">>>服务启动：ws://{addr}:{port}");
                        return true;
                    }
                    else
                    {
                        MessageDialog.Show(this, "服务启动失败");
                        return false;
                    }
                }
                else
                {
                    MessageDialog.Show(this, "服务初始化失败");
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageDialog.Show(this, "服务初始化异常：" + e.Message);
                return false;
            }
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
        private void StartServer()
        {
            if (CheckInputPort(InputPort.Text.ToString()))
            {
                port = int.Parse(InputPort.Text.ToString());
                addr = SelectIpAddr.SelectedValue.ToString();
                if (InitServer())
                {
                    BtnControlServer.IsEnabled = false;
                    BtnStopServer.IsEnabled = true;
                    BtnSend.IsEnabled = true;
                    SelectIpAddr.IsEnabled = false;
                    InputPort.IsEnabled = false;
                };
            }
            else
            {
                MessageDialog.Show(this,"服务端口输入不合法");
            }
        }

        private void StopServer()
        {
            try
            {
                socketServer?.Stop();
                socketServer?.Dispose();
                BtnControlServer.IsEnabled = true;
                BtnSend.IsEnabled = false;
                BtnStopServer.IsEnabled = false;
                SelectIpAddr.IsEnabled = true;
                InputPort.IsEnabled = true;
                AppendInfo(">>>服务已停止");
            }
            catch (Exception e)
            {
                MessageDialog.Show(this, "服务停止异常： " + e.Message);
            }
        }

        private void RefreshSessionList(bool clear = false)
        {


            Dispatcher.Invoke(() =>
            {
                if (clear)
                {
                    sessionList.Clear();
                }
                ListViewSession.ItemsSource = null;
                ListViewSession.ItemsSource = sessionList;
            });
        }
        private void SocketServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {

            sessionList.Remove(session);
            sendSessionList.Remove(session);

            RefreshSessionList();
            AppendInfo($"【{session.RemoteEndPoint}】: 退出会话");
        }

        private void SocketServer_NewSessionConnected(WebSocketSession session)
        {

            sessionList.Add(session);
            sendSessionList.Add(session);

            RefreshSessionList();
            AppendInfo($"【{session.RemoteEndPoint}】: 加入会话");
        }

        private void SocketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            AppendInfo($"【{session.RemoteEndPoint}】: {value}");
        }

        private void SocketServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            AppendInfo($"【{session.RemoteEndPoint}】: NewDataReceived");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            var sessionId = (sender as CheckBox).Tag.ToString();

            foreach (var session in sessionList)
            {
                if (session.SessionID == sessionId)
                {
                    sendSessionList.Add(session);
                }
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var sessionId = (sender as CheckBox).Tag.ToString();
            foreach (var session in sessionList)
            {
                if (session.SessionID == sessionId)
                {
                    sendSessionList.Remove(session);
                }
            }
        }

        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            var sessionId = (sender as CheckBox).Tag.ToString();
            var search = false;
            foreach (var session in sendSessionList)
            {
                if (session.SessionID == sessionId)
                {
                    search = true;
                    break;
                }
            }
            (sender as CheckBox).IsChecked = search;
        }
    }
}
