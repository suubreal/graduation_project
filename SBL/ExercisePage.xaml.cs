using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using Microsoft.Kinect;
using Gecko;
using System.Windows.Forms.Integration;


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

    using DotNetBrowser;
    using DotNetBrowser.WinForms;

    
   
    public partial class ExercisePage : Page, INotifyPropertyChanged
    {

        /*****************body********************************** *****************************************/
        private const double HandSize = 30;
        
        private const double JointThickness = 3;
        
        private const double ClipBoundsThickness = 10;
        
        private const float InferredZPositionClamp = 0.1f;
        
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
        
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
        
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
        
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
  
        private readonly Brush inferredJointBrush = Brushes.Yellow;
   
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        
        private DrawingGroup drawingGroup;
        
        private DrawingImage imageSource;

        /*******************************************************************************************/

        private KinectSensor kinectSensor = null;
           
        private ColorFrameReader colorFrameReader = null;

        private CoordinateMapper coordinateMapper = null;

        private BodyFrameReader bodyFrameReader = null;

        private Body[] bodies = null;

        private List<Tuple<JointType, JointType>> bones;

        private int displayWidth;

        private int displayHeight;
        
        private List<Pen> bodyColors;

        private WriteableBitmap colorBitmap = null;

        private string statusText = null;

        public ExercisePage(String msg, String youtube_id)
        {

            //Client client = (Client)Application.Current.Resources["ApplicationScopeResource"];           
                       
            //string exercise = client.recvMsg();
            string exercise =msg;
          
            /**************************************************************************/
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();
            
            // open the reader for the color frames
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // wire handler for frame arrival
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;
            
            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.displayWidth = colorFrameDescription.Width;
            this.displayHeight = colorFrameDescription.Height;

                        
            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            
             /***********************************************************************************************************/            
            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();
            

            if (exercise.Equals("Lunges") || exercise.Equals("SidePlank")||exercise.Equals("StandingVRaise")||exercise.Equals("StandingPikeCrunch"))
            {
            //상체 운동

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            }
            else{
            //하체운동

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
            
            }

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));
            


            //*************************************************//
            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;


            //*********************************************

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;



            // use the window object as the view model in this simple example
            this.DataContext = this;
            InitializeComponent();

            Gecko.Xpcom.Initialize("Firefox");

            WindowsFormsHost host = new WindowsFormsHost();
            GeckoWebBrowser browser = new GeckoWebBrowser();
            host.Child = browser;
            GridWeb.Children.Add(host);
            browser.Navigate("https://www.youtube.com/embed/" + youtube_id + "?autoplay=1");



            Ex_Name.Text = msg;
            SetDescription(msg);



            //youtube url 설정           
            //Youtube_View.URL = "https://www.youtube.com/embed/"+youtube_id+"?autoplay=1";


            /*
            switch (exercise)
            {
                case "Lunges":
                    Youtube_View.URL = "https://www.youtube.com/embed/QF0BQS2W80k?autoplay=1";
                    break;
                case "StandingPikeCrunch":
                    Youtube_View.URL = "https://www.youtube.com/embed/QF0BQS2W80k?autoplay=1";
                    break;

                case "SidePlank":
                    Youtube_View.URL = "https://www.youtube.com/embed/QF0BQS2W80k?autoplay=1";
                    break;

                case "StandingVRaise":
                    Youtube_View.URL = "https://www.youtube.com/embed/QF0BQS2W80k?autoplay=1";
                    break;

                default:
                    Youtube_View.URL = "https://www.youtube.com/embed/QF0BQS2W80k?autoplay=1";
                    break;
            }
            */
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetDescription(string exercise)
        {
            switch(exercise)
            {
                case "ShoulderPress":
                    exercise_desc1.Text = "난이도 : 중급 \n";
                    exercise_desc1.Text += "운동부위 : 어깨 \n";
                    exercise_desc1.Text += "주운동 근육 부위 : 삼각근 \n";
                    exercise_desc1.Text += "부운동 근육 부위 : 상완삼두근, 승모근 상부 \n";

                    exercise_desc2.Text = "1. 등과 허리를 곱게 핀다.\n";
                    exercise_desc2.Text += "2. 팔을 귀와 수평이 되고 팔꿈치가 직각이 되도록 위치시킨다. \n";
                    exercise_desc2.Text += "3. 이두근이 귀에 닿는 느낌으로 덤벨을 머리 위로 올린다. \n";
                    break;
                case "Lunges":
                    exercise_desc1.Text = "난이도 : 중급 \n";
                    exercise_desc1.Text += "운동부위 : 허벅지 앞 \n";
                    exercise_desc1.Text += "주운동 근육 부위 : 대퇴사두근 \n";
                    exercise_desc1.Text += "부운동 근육 부위 : 대두근, 슬글곡근 \n";

                    exercise_desc2.Text = "1. 두 팔을 골반 너비로 벌리고 허리에 손을 대고 바로 선다. \n";
                    exercise_desc2.Text += "2. 오른발을 앞으로 70~100cm 정도 벌려 내밀고, 왼발의 뒤꿈치를 세운다. 이 때 시선은 정면을 향한다. \n";
                    exercise_desc2.Text += "3. 등과 허리를 똑바로 편 상태에서 오른쪽 무릎을 90도로 구부리고 왼쪽 무릎은 바닥에 닿는 느낌으로 무릎을 구부린다. \n";
                    break;
                case "Squat":
                    exercise_desc1.Text = "난이도 : 상급 \n";
                    exercise_desc1.Text += "운동부위 : 허벅지 앞 \n";
                    exercise_desc1.Text += "주운동 근육 부위 : 대퇴사두근 \n";
                    exercise_desc1.Text += "부운동 근육 부위 : 대두근, 슬글곡근 \n";

                    exercise_desc2.Text = "1. 어깨 넓이로 발을 벌리고 양 팔은 몸에 가볍게 붙인다. \n";
                    exercise_desc2.Text += "2. 밸런스를 취하며 그대로 무릎을 굽혀 허리 위 상반신을 내린다. \n";
                    exercise_desc2.Text += "3. 허벅지와 바닥이 평행을 이룰 때까지 허리를 낮춘다. 그리고 가능하면 평행 상태에서 1초 정도 머문다. \n";
                    exercise_desc2.Text += "4. 숨을 뱉으면서 무릎과 등을 세우면서 허리를 올린다. \n";
                    break;
                case "SideLunge":
                    exercise_desc1.Text = "난이도 : 초급 \n";
                    exercise_desc1.Text += "운동부위 : 허벅지 앞 \n";
                    exercise_desc1.Text += "주운동 근육 부위 : 대퇴사두근 \n";

                    exercise_desc2.Text = "1. 다리를 어깨너비로 벌리고 선다. \n";
                    exercise_desc2.Text += "2. 무게중심을 중앙에 둔 상태에서 오른발을 오른쪽으로 한 발 크게 내딛는다. \n";
                    exercise_desc2.Text += "3. 마찬가지로 무게중심을 중앙에 두고 반대로 왼발을 왼쪽으로 한 발 크게 내딛는다. \n";
                    exercise_desc2.Text += "4. 동작을 반복한다. \n";
                    break;
                case "JumpingJack":
                    exercise_desc1.Text = "난이도 : 초급 \n";
                    exercise_desc1.Text += "운동부위 : 전신 \n";

                    exercise_desc2.Text = "1. 차렷 자세를 취한다. \n";
                    exercise_desc2.Text += "2. 두 팔을 양 옆으로 올리면서 두 발을 점프해서 벌린다. \n";
                    exercise_desc2.Text += "3. 다시 차렷 자세로 돌아간다. \n";
                    exercise_desc2.Text += "4. 두 발을 점프해서 더 넓게 벌리면서 두 손은 머리 위로 올려 박수를 친다. \n";
                    exercise_desc2.Text += "5. 다시 차렷 자세로 돌아간다. 동작을 반복한다. \n";
                    break;
                case "SidePlank":
                    exercise_desc1.Text = "난이도 : 중급 \n";
                    exercise_desc1.Text += "운동부위 : 전신 \n";

                    exercise_desc2.Text = "1. 옆으로 돌아누운 자세에서 두 다리를 포갠다. \n";
                    exercise_desc2.Text += "2. 팔꿈치와 손바닥은 펴서 바닥에 대서 들어올린다. \n";
                    exercise_desc2.Text += "3. 상체를 들어올린다. \n";
                    exercise_desc2.Text += "4. 이 자세로 30초 ~ 1분을 버틴다. \n";
                    break;
                case "StandingPikeCrunch":
                    exercise_desc1.Text = "난이도 : 중급 \n";
                    exercise_desc1.Text += "운동부위 : 상체 \n";
                    exercise_desc1.Text += "주운동 근육 부위 : 삼각근, 송모근 \n";

                    exercise_desc2.Text = "1. 다리를 어깨 넓이로 벌린다. \n";
                    exercise_desc2.Text += "2. 두 팔을 어깨와 평행이 되도록 들어올린다. \n";
                    break;
                case "StandingVRaise":
                    exercise_desc1.Text = "난이도 : 중급 \n";
                    exercise_desc1.Text += "운동부위 : 전신 \n";

                    exercise_desc2.Text = "1. 정면을 보고 양 손을 귀 옆에 둔다. \n";
                    exercise_desc2.Text += "2. 무릎을 한 쪽 씩 올려 팔꿈치에 붙이는 것을 왼쪽 오른쪽 반복한다. \n";
                    break;
                default:
                    exercise_desc1.Text = "업데이트 중입니다^^ \n";

                    exercise_desc2.Text = "업데이트 중입니다^^ \n";
                    break;

            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }
                
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
                }
            }
        }

        
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                  
                    dc.DrawImage(this.colorBitmap, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));


                    int penIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space --> convert 필요없음!
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            //joing point 가져오는 부분*********************
                            foreach (JointType jointType in joints.Keys)
                            {

                                CameraSpacePoint position = joints[jointType].Position;

                                ColorSpacePoint colorSpacePoint = this.coordinateMapper.MapCameraPointToColorSpace(position);

                                jointPoints[jointType] = new Point(colorSpacePoint.X, colorSpacePoint.Y);

                            }

                            //skeleton 그리기******************************
                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }





        
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }
        
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }
        
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext) //손!!
        {
            switch (handState)
            {

                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }
        
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        
        
        private void ExercisePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }


        

        private void ExercisePage_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }

            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
        


        
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            //this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
            //                                                : Properties.Resources.SensorNotAvailableStatusText;
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
    }
}
