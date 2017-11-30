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

using System.Threading;
using System.Windows.Threading;

namespace SBL
{
    /// <summary>
    /// Interaction logic for PredictPage.xaml
    /// </summary>
    public partial class PredictPage : Page
    {
        Client client;

        public PredictPage()
        {
            InitializeComponent();
            client = (Client)Application.Current.Resources["ApplicationScopeResource"];

            
                Thread thread = new Thread(new ThreadStart(predict));
                thread.Start();
        
           

        }


        private void  Predict_Page_Loaded(object sender, RoutedEventArgs e)
        {


            string msg = client.recvMsg();

            NavigationService.Navigate(new SelectPage(msg));
        }



        private void predict()
        {           

            string msg = client.recvMsg();

        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        {
            NavigationService.Navigate(new SelectPage(msg));
        }));

        }

        /*
        internal void SetLoadCompleted(NavigationService navigationService)
        {
            Console.WriteLine("set load completed");
            navigationService.LoadCompleted += new LoadCompletedEventHandler(NavigationService_Loadcompleted);
            //throw new NotImplementedException();
        }

        void NavigationService_Loadcompleted(object sender, NavigationEventArgs e)
        {
            //if(e.ExtraData!=null){}
            Console.WriteLine("load completed");
            this.NavigationService.LoadCompleted -= new LoadCompletedEventHandler(NavigationService_Loadcompleted);
        }
        */
        /*
public void predict()
{
   client = (Client)Application.Current.Resources["ApplicationScopeResource"];
   string msg = client.recvMsg();

   Console.WriteLine("predictwindow [" + msg + "]");
   //send msg to selectpage
   if (NavigationService != null)
   {
       NavigationService.Navigate(new SelectPage(msg));
   }
       //NavigationService.Navigate(new Uri("SelectPage.xaml", UriKind.Relative));
}*/
    }
}
