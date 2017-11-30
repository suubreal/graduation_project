using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SBL
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {

        Client client;

        public App()
        {
            client = new Client();
            client.Start();
            Console.WriteLine("start!!!!");

            Application.Current.Resources["ApplicationScopeResource"] = client;

            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }
}
