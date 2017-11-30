//using Google.Apis.Samples.Helper;
//using Google.Apis.Services;
//using Google.Apis.YouTube.v3;//
//using Google.Apis.YouTube.v3.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace SBL
{
    /// <summary>
    /// Interaction logic for SelectPage.xaml
    /// </summary>
    public partial class SelectPage : Page
    {
        Client client;
        string[,] youtube_data;
        string exercise_name;
        
        public SelectPage(String msg)
        {
            InitializeComponent();

            exercise_name = msg;
            Ex_Name.Text = msg;
            client = (Client)Application.Current.Resources["ApplicationScopeResource"];
            
            //test
            client.sendMsg("select");
            client.sendMsg(exercise_name);
            
            //grid 속성 지정
            Image[] image_list = new Image[8] { image1, image2, image3, image4, image5, image6, image7, image8 };
            TextBlock[] title_list = new TextBlock[8] { title1, title2, title3, title4, title5, title6, title7, title8 };
            TextBlock[] channel_list = new TextBlock[8] { channel1, channel2, channel3, channel4, channel5, channel6, channel7, channel8 };
            TextBlock[] viewcount_list = new TextBlock[8] { viewcount1, viewcount2, viewcount3, viewcount4, viewcount5, viewcount6, viewcount7, viewcount8 };

            //title, id, url, channel, duration, viewcount 
            youtube_data = new string[8, 6];
            for(int  i=0; i<8; i++)
            {
                for(int j=0; j<6; j++)
                {
                    youtube_data[i, j] = "";
                }
            }


            int k = 0;

           
            while (true) { 
               string str1 = client.recvMsg();

                if (str1.Equals("/select"))
                {
                    Console.WriteLine(str1);
                    Console.WriteLine("select ended!!!");
                    break;
                }

                string str2 = client.recvMsg();
                string str3 = client.recvMsg();
                string str4 = client.recvMsg();
                string str5 = client.recvMsg();
                string str6 = client.recvMsg();

                if (k < 8)
                {
                    youtube_data[k, 0] = str1;
                    youtube_data[k, 1] = str2;
                    youtube_data[k, 2] = str3 ;
                    youtube_data[k, 3] = str4;
                    youtube_data[k, 4] = str5;
                    youtube_data[k, 5] = str6;

                    k++;
                }

                //각 이미지주소 표에 바인딩 - 몇개인지 받아오는거 필요하면 앞에서 처리해야함.

            }

            //title, id, url, channel, duration, viewcount 
            for (int i = 0; i < 8; i++)
            {
                image_list[i].Source = LoadImage(youtube_data[i, 2]); //ggggggggggggggggggggg
                title_list[i].Text = youtube_data[i, 0 ];
                channel_list[i].Text = youtube_data[i, 3];
                viewcount_list[i].Text ="조회수  "+ youtube_data[i, 5];
            }



          
            //Select_View.URL =  "https://www.youtube.com/embed?listType=search&list=lunges";
            //Select_View.URL = " https://www.youtube.com/results?search_query=Lunges";

          
            
        }

        public void SetExercisePage(String msg){
            //send msg to exercise page
           // NavigationService.Navigate(new ExercisePage(msg));
      
        }


        public BitmapImage LoadImage(string url)   //Image URL -> Bitmap으로..
        {
            try

            {

                if (string.IsNullOrEmpty(url))

                    return null;

                WebClient wc = new WebClient();

                Byte[] MyData = wc.DownloadData(url);

                wc.Dispose();

                BitmapImage bimgTemp = new BitmapImage();

                bimgTemp.BeginInit();

                bimgTemp.StreamSource = new MemoryStream(MyData);

                bimgTemp.EndInit();

                return bimgTemp;

            }

            catch

            {

                return null;

            }

        }

        internal void SetLoadCompleted(NavigationService navigationService)
        {
            throw new NotImplementedException();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            string name =( (Button)sender).Name;
            int   num = int.Parse(name.Substring(6));
            Console.WriteLine(">num:" + num);
            NavigationService.Navigate(new ExercisePage(exercise_name, youtube_data[num - 1, 1]));
                



        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
                this.NavigationService.GoBack();
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("No entries in back navigation history.");
            }
        }
    }
}

/*
namespace dotnet
{
  class search
  {
    static void Main(string[] args)
    {
      CommandLine.EnableExceptionHandling();
      CommandLine.DisplayGoogleSampleHeader("YouTube Data API: Search");

      SimpleClientCredentials credentials = PromptingClientCredentials.EnsureSimpleClientCredentials();

      YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer() {
        ApiKey = credentials.ApiKey
      });

      SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
      listRequest.Q = CommandLine.RequestUserInput<string>("Search term: ");
      listRequest.Order = SearchResource.Order.Relevance;

      SearchListResponse searchResponse = listRequest.Fetch();

      List<string> videos = new List<string>();
      List<string> channels = new List<string>();
      List<string> playlists = new List<string>();

      foreach (SearchResult searchResult in searchResponse.Items)
      {
        switch (searchResult.Id.Kind)
        {
          case "youtube#video":
            videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
          break;

          case "youtube#channel":
            channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
          break;

          case "youtube#playlist":
            playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
          break;
        }
      }

      CommandLine.WriteLine(String.Format("Videos:\n{0}\n", String.Join("\n", videos.ToArray())));
      CommandLine.WriteLine(String.Format("Channels:\n{0}\n", String.Join("\n", channels.ToArray())));
      CommandLine.WriteLine(String.Format("Playlists:\n{0}\n", String.Join("\n", playlists.ToArray())));

      CommandLine.PressAnyKeyToExit();
    }
  }
}
*/