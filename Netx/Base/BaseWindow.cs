using Netx.Util;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Netx.Base
{
    /// <summary>
    /// BaseWindow.xaml 的交互逻辑
    /// </summary>
    public abstract partial class BaseWindow : Window
    {
        public abstract void InitEvents();

        public async void GetIpv4AddrListHandler(Action<List<string>> action, bool showLocal = true)
        {
            var result = await CommonUtil.GetIpv4AddrListAsync(showLocal);
            action(result);
        }
    }
}
