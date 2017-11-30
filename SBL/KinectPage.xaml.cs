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

//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace SBL
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class KinectPage : Page, INotifyPropertyChanged
    {
        int tmp_count = 0;
        /// <summary>
        /// Active Kinect sensor 키넥트 센서
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for color frames 컬러프레임 리더
        /// </summary>
        private ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// Bitmap to display 보여줄 비트맵
        /// </summary>
        private WriteableBitmap colorBitmap = null;

        /// <summary>
        /// Current status text to display 
        /// </summary>
        private string statusText = null;


        System.DateTime start_time;

        Client client;
        Boolean save_flag = false;
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public KinectPage()
        {
            client = (Client)Application.Current.Resources["ApplicationScopeResource"];
            Console.WriteLine("kinectwindow! hello send");

            //이미지 저장 시작
            client.sendMsg("save");

            //시작 시간 받기
            start_time = System.DateTime.Now;


            //키넥트
            // get the kinectSensor object 키넥트센서
            this.kinectSensor = KinectSensor.GetDefault();

            // open the reader for the color frames 컬러프레임 리더
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // wire handler for frame arrival 프레임 도착 핸들러 *****************
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;


            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // create the bitmap to display 보여줄 비트맵 생성
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor 센서 오픈!
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display 보여줄 비트맵 가져옴! - viewbox : {binding imagesource}
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.colorBitmap;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to displa
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }


        //이미지 저장
        private void SaveImage(System.DateTime now_time)
        {
            save_flag = true;

            string time = now_time.ToString("hh'-'mm'-'ss'-'ffff", CultureInfo.CurrentUICulture.DateTimeFormat);
            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path = System.IO.Path.Combine("C:\\Users\\MBM\\Desktop\\EARTH\\kinectimage"
                          , "Kinect-" + time + ".jpg");

            var jpegEncoder = new JpegBitmapEncoder();
            jpegEncoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                jpegEncoder.Save(fs);
            }

            //이미지 파일 전송
            client.sendFile(path);
        }


        /// <summary>
        /// Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // ColorFrame is IDisposable
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();

                    }


                    
                    //이미지 저장
                    System.DateTime now_time = System.DateTime.Now;
                    if ((now_time - start_time).Seconds < 10)
                    {
                        SaveImage(now_time);
                        Console.WriteLine(tmp_count);
                        tmp_count++;
                    }                    
                    else
                    {
                        Console.WriteLine("else!!!!!!!!!!");
                        this.colorFrameReader.FrameArrived -= this.Reader_ColorFrameArrived;
                        this.kinectSensor.IsAvailableChanged -= this.Sensor_IsAvailableChanged;

                        //closing event
                        if (this.colorFrameReader != null)
                        {
                            // ColorFrameReder is IDisposable
                            this.colorFrameReader.Dispose();
                            this.colorFrameReader = null;
                        }

                        if (this.kinectSensor != null)
                        {
                            this.kinectSensor.Close();
                            this.kinectSensor = null;
                        }

                        System.Threading.Thread.Sleep(1000);
                        client.sendMsg("/save");
                        System.Threading.Thread.Sleep(1000);
                        client.sendMsg("predict");

                        NavigationService.Navigate(new PredictPage());
                            //setPredictWindow();
                        
                        //setPredictWindow();
                        // save_flag = false;
                       // predict();
                        

                        /*
                        if (save_flag)
                        {
                            client.sendMsg("/save");
                            client.sendMsg("predict");
                            //NavigationService.Navigate(new PredictPage());
                            //setPredictWindow();
                            save_flag = false;
                           // predict();
                        }
                        */
                        //predict();
                        /*
                        //왜지...........달ㅡㄴ화면전환을찾는다..........왤까..............
                        if (NavigationService != null)
                        {
                            Console.WriteLine(">in if navigation ");
                            NavigationService.Navigate(new Uri("PredictPage.xaml", UriKind.Relative));
                            predict();
                            Console.WriteLine("break!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11");

                        }
                        */
                    }
                }
            
            }
        }
        /*
        private void setPredictWindow()
        {
            Console.WriteLine(">setPredictWindow");
            PredictPage predictPage = new PredictPage();
            predictPage.SetLoadCompleted(NavigationService);
            this.NavigationService.Navigate(predictPage);
            //NavigationService.Navigate(new Uri("PredictPage.xaml", UriKind.Relative));
        }
        */
    
            //send msg to selectpage
            /*
            if (NavigationService != null)
            {
                NavigationService.Navigate(new SelectPage(msg));
            }
            //NavigationService.Navigate(new Uri("SelectPage.xaml", UriKind.Relative));
        }
    */
        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            //this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
            //                                                : Properties.Resources.SensorNotAvailableStatusText;
        }


    }
}
