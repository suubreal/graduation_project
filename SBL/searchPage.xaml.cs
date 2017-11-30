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
using System.Windows.Threading;

namespace SBL
{
    /// <summary>
    /// searchPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class searchPage : Page
    {
  
        public searchPage()
        {
            InitializeComponent();
            
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // string msg = client.recvMsg();
            // string msg = 
            string msg = Search_Box.Text;

            if(msg == "")
            {
                MessageBox.Show("검색하실 운동명을 입력해주세요","Error3");
                return;
            }

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                //client.sendMsg(msg);
                NavigationService.Navigate(new SelectPage(msg));
            }));

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("No entries in back navigation history.");
            }
        }

        //button back일 경우, 서버한테 다른 메시지 보내야해
    }
}
