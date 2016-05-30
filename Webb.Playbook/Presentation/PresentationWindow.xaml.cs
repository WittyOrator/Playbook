using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.Windows.Threading;

using System.Windows.Interop;

using Webb.Playbook.Data;
using Webb.Playbook.Geometry;
using Draw = Webb.Playbook.Geometry.Drawing;

namespace Webb.Playbook.Presentation
{
    public enum RemoteState
    {
        REV = 1,
        RPLAY = 2,
        PLAY = 4,
        FF = 8,
        SLOMO = 16,
        STILL = 32,
        FRAME = 64,
        STOP = 128,
        DOWN = 4096,
        UP = 8192,
    }

    public enum VideoState
    {
        state_Play = 0,
        state_Still = 1,
        state_Stop = 2,
        state_Slow5x = 3,
        state_Slow20x = 4,
        state_Play2x = 5,
        state_Play5x = 6,
        state_RPlay = 7,
        state_RSlow5x = 8,
        state_RSlow20x = 9,
        state_RPlay2x = 10,
        state_RPlay5x = 11
    }

    /// <summary>
    /// PresentationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PresentationWindow : Window
    {
        private double scale = Webb.Playbook.Data.GameSetting.Instance.ScalingAnimation;
        private Webb.Playbook.Data.Presentation presentation;
        private int index = 0;
        private int Index
        {
            get { return index; }
            set { index = value; }
        }
        private Draw OriDrawing;
        private Canvas OriCanvas;
        private bool Play = false;
        private double Opacity = 1;
        private Color freeDrawColor = Colors.Black;
        private ToolManager toolManager2DToolBar = new ToolManager();
        private ToolManager toolManagerColorsToolBar = new ToolManager();
        private ToolManager toolManagerVideoToolBar = new ToolManager();
        private int nLastRemoteState = 0;
        private AxWEBBWEBPLAYERLib.AxWebbWebPlayer wPlayer = new AxWEBBWEBPLAYERLib.AxWebbWebPlayer();
        private DispatcherTimer timer = null;
        private DispatcherTimer timerProgress = null;

        private DrawingWindow drawWindow = new DrawingWindow(); // 11-10-2011 Scott

        private string strSoundFile = "Sound.wma";
        private bool bRecord = false;
        private DX.SoundRecord recorder = null;
        private Microsoft.DirectX.AudioVideoPlayback.Audio audio = null;
        private bool bEditCoachingPoint = false;
        private bool bShowPresentationTools = false;
        private bool bPlayCoachingPoint = false;

        private int nLastVideoState = (int)VideoState.state_Stop;

        private MessageFilter messageFilter = new MessageFilter();

        public bool Recording
        {
            get 
            {
                return bRecord; 
            }
            set 
            {
                if (bRecord != value)
                {
                    bRecord = value;

                    RecordChanged(bRecord);
                }
            }
        }

        private VideoCoachingPoint editVCP = null;
        public VideoCoachingPoint EditVCP
        {
            set
            {
                if (editVCP != value)
                {
                    editVCP = value;

                    EditVCPChanged();
                }
            }
            get
            {
                return editVCP;
            }
        }

        private VideoCoachingPoint playVCP = null;
        public VideoCoachingPoint PlayVCP
        {
            set
            {
                if (playVCP != value)
                {
                    playVCP = value;
                }
            }
            get
            {
                return playVCP;
            }
        }

        private void RecordChanged(bool bRec)
        {
            if (bRec)
            {
                tbAudioState.Text = "Recording...";

                btnPauseCaptureAudio.Visibility = Visibility.Visible;
            }
            else
            {
                tbAudioState.Text = string.Empty;

                btnPauseCaptureAudio.Visibility = Visibility.Collapsed;
            }
        }

        public bool PlayingCoachingPoint
        {
            get { return bPlayCoachingPoint; }
            set
            {
                if (bPlayCoachingPoint != value)
                {
                    bPlayCoachingPoint = value;
                }
            }
        }

        public bool EditCoachingPoint
        {
            get { return bEditCoachingPoint; }
            set
            {
                if (bEditCoachingPoint != value)
                {
                    bEditCoachingPoint = value;

                    if (bEditCoachingPoint)
                    {
                        spCoachingPoint.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        spCoachingPoint.Visibility = Visibility.Collapsed;
                    }

                    EditCoachingPointChanged(bEditCoachingPoint);
                }
            }
        }

        public PresentationWindow()
        {
            InitializeComponent();

            IEHost.Child = wPlayer;

            this.Loaded += new RoutedEventHandler(PresentationWindow_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(PresentationWindow_Closing);
            this.KeyUp += new KeyEventHandler(PresentationWindow_KeyUp);
            this.Activated += new EventHandler(PresentationWindow_Activated);

            toolManager2DToolBar.Init2DFullScreenToolBarMenu();
            toolBarPlayControl.ItemsSource = toolManager2DToolBar.Tools;

            toolManagerColorsToolBar.InitPresentationColorToolBarMenu();
            toolBarColors.ItemsSource = toolManagerColorsToolBar.Tools;

            toolManagerVideoToolBar.InitVideoFullScreenToolBarMenu();
            toolBarPlayVideoControl.ItemsSource = toolManagerVideoToolBar.Tools;

            timer = new DispatcherTimer { Interval = new TimeSpan(500000) };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            timerProgress = new DispatcherTimer { Interval = new TimeSpan(200000) };
            timerProgress.Tick += new EventHandler(timerProgress_Tick);
            timerProgress.Start();
        }

        public void EditVCPChanged()
        {
            if (EditVCP == null)
            {
                ClearCoachingPoint();
            }
            else
            {
                LoadCoachingPoint(EditVCP);
            }
        }

        public void LoadCoachingPoint(VideoCoachingPoint vcp)
        {
            drawWindow.LoadDrawing(vcp.DrawingFile);

            SetBehavior("Dragger");
        }

        public void ClearCoachingPoint()
        {
            drawWindow.CloseDrawing();
        }

        public void EditCoachingPointChanged(bool bEdit)
        {
            if (!bEdit)
            {
                EditVCP = null;
            }
            spMainTools.Visibility = bEdit ? Visibility.Collapsed : Visibility.Visible;
            spCoachingPointList.Visibility = bEdit ? Visibility.Collapsed : Visibility.Visible;
        }

        void PresentationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        void PresentationWindow_Activated(object sender, EventArgs e)
        {
            if (this.IsActive)
            {

            }
            else
            {

            }
        }

        private void CheckVideo(int indexPlay, int indexVideo)
        {
            string strVideoFile = presentation.Plays[indexPlay].Videos[indexVideo].ToString();

            if (!System.IO.File.Exists(strVideoFile))
            {
                if (MessageBox.Show(string.Format("{0} has not exist, do you want to choose another video ?", strVideoFile), "Webb Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
                    {
                        Filter = Webb.Playbook.Data.Extensions.VideoFileFilter,
                        AddExtension = true,
                    };

                    if (openFileDialog.ShowDialog().Value)
                    {
                        presentation.Plays[indexPlay].Videos[indexVideo] = openFileDialog.FileName;
                    }
                }
            }
        }

        private int ConvertPptToJpg(string strPptPath)
        {
            PowerPoint ppt = new PowerPoint(strPptPath);

            string strJpgPath = ppt.ImagePath;

            int nCount = 0;

            var convertResults = Office_To_XPS.OfficeToXps.ConvertToXps(strPptPath, ref strJpgPath, ref nCount);

            switch (convertResults.Result)
            {
                case Office_To_XPS.ConversionResult.OK:

                    break;
                case Office_To_XPS.ConversionResult.InvalidFilePath:
                    // 处理文件路径错误或文件不存在
                    break;
                case Office_To_XPS.ConversionResult.UnexpectedError:

                    break;
                case Office_To_XPS.ConversionResult.ErrorUnableToInitializeOfficeApp:
                    // Office2007 未安装会出现这个异常
                    break;
                case Office_To_XPS.ConversionResult.ErrorUnableToOpenOfficeFile:
                    // 文件被占用会出现这个异常
                    break;
                case Office_To_XPS.ConversionResult.ErrorUnableToAccessOfficeInterop:
                    // Office2007 未安装会出现这个异常
                    break;
                case Office_To_XPS.ConversionResult.ErrorUnableToExportToXps:
                    // 微软 OFFICE2007 Save As PDF 或 XPS  插件未安装异常
                    break;
            }

            return nCount;
        }

        public BitmapImage GetBitmapImage(string path)  // fix can't release image
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();

            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(path));
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        void Load(string strFilePah)
        {
            canvasPowerPoint.Visibility = Visibility.Collapsed;
            canvasDrawing.Visibility = Visibility.Visible;

            if (OriDrawing != null)
            {
                OriDrawing.Dispose();
            }
            OriDrawing = Draw.Load(strFilePah, canvasDrawing);
            OriDrawing.Behavior = null;

            if (OriDrawing.Title)
            {
                OriDrawing.HideTitleBorder();
                toolBarSize.Visibility = Visibility.Collapsed;
                Scale(100);
            }
            else
            {
                toolBarSize.Visibility = Visibility.Visible;
                Scale(scale);
            }

            canvasDrawing.Focus();
        }

        private void gridProgress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int nVideoState = wPlayer.GetCurrentState();

            Point pt = e.GetPosition(gridProgress);

            int nTotalFrame = wPlayer.GetTotalFrames();

            int nSetFrame = (int)(pt.X * nTotalFrame / gridProgress.ActualWidth);

            // 12-12-2011 Scott
            VideoCoachingPointInfo vcpi = GetCurrentVideoInfo(presentation);
            if (vcpi != null)
            {
                Point ptMouse = e.GetPosition(gridProgress);

                VideoCoachingPoint vcp = vcpi.CheckFrame(ptMouse);

                if (vcp != null && e.ClickCount > 1)
                {
                    wPlayer.SetCurrentFrame(vcp.Frame);

                    wPlayer.Play1X();

                    wPlayer.Pause();

                    wPlayer.SetCurrentFrame(vcp.Frame); 

                    PlayCoachingPoint(vcp);
                
                    return;
                }
            }

            wPlayer.SetCurrentFrame(nSetFrame);

            StopCoachingPoint();

            switch (nVideoState)
            {
                case (int)VideoState.state_Still:
                case (int)VideoState.state_Stop:
                    break;
                default:
                    wPlayer.Play1X();
                    break;
            }
        }

        void timerProgress_Tick(object sender, EventArgs e)
        {
            if (rectProgress.IsLoaded)
            {
                int nFrame = wPlayer.GetCurrentFrame();
                int nTotalFrame = wPlayer.GetTotalFrames();

                rectProgress.Width = nFrame * gridProgress.ActualWidth / nTotalFrame;

                tbTime.Text = VideoHelper.GetTimeString(nFrame);

                // 11-25-2011 Scott

                VideoCoachingPointInfo vcpi = GetCurrentVideoInfo(presentation);
                if (vcpi != null)
                {
                    int nVideoState = wPlayer.GetCurrentState();

                    if (nVideoState != (int)VideoState.state_Still && nVideoState != (int)VideoState.state_Stop)
                    {
                        int nLeftBuffer = 0;
                        int nRightBuffer = 0;
                        int nLeftLeaveBuffer = 0;
                        int nRightLeaveBuffer = 0;

                        switch (nVideoState)
                        {
                            case (int)VideoState.state_Play5x:
                                nLeftBuffer = 10;
                                nRightLeaveBuffer = 5;
                                break;
                            case (int)VideoState.state_RPlay5x:
                                nRightBuffer = 10;
                                nLeftLeaveBuffer = 5;
                                break;
                            case (int)VideoState.state_Play2x:
                                nLeftBuffer = 6;
                                nRightLeaveBuffer = 3;
                                break;
                            case (int)VideoState.state_RPlay2x:
                                nRightBuffer = 6;
                                nLeftLeaveBuffer = 3;
                                break;
                            case (int)VideoState.state_Play:
                                nLeftBuffer = 3;
                                nRightLeaveBuffer = 1;
                                break;
                            case (int)VideoState.state_RPlay:
                                nRightBuffer = 3;
                                nLeftLeaveBuffer = 1;
                                break;
                            case (int)VideoState.state_Slow5x:
                                nLeftBuffer = 1;
                                nRightLeaveBuffer = 1;
                                break;
                            case (int)VideoState.state_RSlow5x:
                                nRightBuffer = 1;
                                nLeftLeaveBuffer = 1;
                                break;
                            case (int)VideoState.state_Slow20x:
                                nLeftBuffer = 1;
                                nRightLeaveBuffer = 1;
                                break;
                            case (int)VideoState.state_RSlow20x:
                                nRightBuffer = 1;
                                nLeftLeaveBuffer = 1;
                                break;
                        }

                        bool bFind = false;
                        foreach (VideoCoachingPoint vcp in vcpi.VideoCoachingPoints)
                        {
                            if (!PlayingCoachingPoint)
                            {
                                if (nFrame > vcp.Frame - nLeftBuffer && nFrame < vcp.Frame + nRightBuffer)
                                {
                                    wPlayer.SetCurrentFrame(vcp.Frame);

                                    wPlayer.Play1X();

                                    wPlayer.Pause();

                                    wPlayer.SetCurrentFrame(vcp.Frame);

                                    PlayCoachingPoint(vcp);

                                    bFind = true;

                                    break;
                                }
                            }
                            else
                            {
                                if (nFrame <= vcp.Frame - nLeftLeaveBuffer || nFrame >= vcp.Frame + nRightLeaveBuffer)
                                {
                                    StopCoachingPoint();

                                    bFind = true;

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {

                    }

                    nLastVideoState = nVideoState;
                }

                // end
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int playIndex = 0;
            int videoIndex = 0;
            GetIndex(out playIndex, out videoIndex);
            int nVideoState = wPlayer.GetCurrentState();
            int nRemoteState = wPlayer.GetRemoteStatus();
            int nFrame = wPlayer.GetCurrentFrame();
            int nTotalFrame = wPlayer.GetTotalFrames();

            if (nLastRemoteState == nRemoteState)
            {
                return;
            }

            nLastRemoteState = nRemoteState;

            switch (nRemoteState)
            {
                case (int)RemoteState.UP:
                    this.btnPrevious_Click(null, null);
                    return;
                case (int)RemoteState.DOWN:
                    this.btnNext_Click(null, null);
                    return;
            }

            if (videoIndex < 0)
            {
                switch (nRemoteState)
                {
                    case (int)RemoteState.STOP:
                        OriDrawing.Stop();
                        break;
                    case (int)RemoteState.STILL:
                        OriDrawing.Still();
                        break;
                    case (int)RemoteState.PLAY:
                        OriDrawing.Run();
                        break;
                }
            }
            else
            {
                if (nTotalFrame > 0)
                {
                    if (nFrame >= nTotalFrame - 1)
                    {// go to next play
                        if (nRemoteState == (int)RemoteState.FF)
                        {
                            this.btnNext_Click(null, null);
                        }
                    }

                    if (nFrame <= 1)
                    {// go to previous play
                        if (nRemoteState == (int)RemoteState.REV)
                        {
                            this.btnPrevious_Click(null, null);
                        }
                    }
                }
            }
        }

        public void PlayCoachingPoint(VideoCoachingPoint vcp)
        {
            PlayingCoachingPoint = true;

            PlayVCP = vcp;

            LoadCoachingPoint(vcp);

            PlayAudio(vcp);

            drawWindow.SetBehavior(string.Empty);   // 12-16-2011 Scott
        }

        public void StopCoachingPoint()
        {
            PlayingCoachingPoint = false;

            PlayVCP = null;

            drawWindow.CloseDrawing();

            Recording = false;
            StopCapture();

            StopAudio();
        }

        void PresentationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            messageFilter.AddThisMessageFilter();

            drawWindow.Show();
            Behavior.Owner = drawWindow;

            imgLogo.Effect = Webb.Playbook.Geometry.PBEffects.SelectedEffect;
            this.slider.Value = Webb.Playbook.Data.GameSetting.Instance.ScalingAnimation;
            Scale(Webb.Playbook.Data.GameSetting.Instance.ScalingAnimation);

            ScaleVideoWindow();

            VisiblePlayControl();

            // 11-10-2011 Scott
            HwndSource hs = (HwndSource)HwndSource.FromVisual(this);
            hs.AddHook(new HwndSourceHook(WndProc));

            int handle = hs.Handle.ToInt32();
            wPlayer.SetNotifyWnd(handle);

            lvShapes.DataContext = CoachingPointObjectInfo.Instance;
            lvTextBoxes.DataContext = CoachingPointObjectInfo.Instance;

            this.Focus();
        }

        void form_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        void window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScaleVideoWindow()
        {
            double width = gridVideo.ActualWidth;
            double height = gridVideo.ActualHeight;
            if (2 * width > 3 * height)
            {
                width = height * 3 / 2;
            }
            else if (2 * width < 3 * height)
            {
                height = width * 2 / 3;
            }
            dockPanelVideo.Margin = new Thickness(
                (gridVideo.ActualWidth - width) / 2, (gridVideo.ActualHeight - height) / 2,
                (gridVideo.ActualWidth - width) / 2, (gridVideo.ActualHeight - height) / 2);

            wPlayer.Width = (int)width;
            wPlayer.Height = (int)height;
            wPlayer.ResizeWindow();

            // 12-01-2011 Scott
            VideoCoachingPointInfo vcpi = GetCurrentVideoInfo(presentation);
            if (vcpi != null)
            {
                int nTotalFrame = wPlayer.GetTotalFrames();

                foreach (VideoCoachingPoint vcp in vcpi.VideoCoachingPoints)
                {
                    if (vcp.Shape != null)
                    {
                        vcp.Shape.Margin = new Thickness(vcp.Frame * this.gridProgress.ActualWidth / nTotalFrame - VideoCoachingPointInfo.ShapeSize / 2, 0, 0, 0);
                    }
                }
            }
            // end

            // 12-02-2011 Scott
            SetDrawingWindowPos(gridVideo, (int)width, (int)height);
            // end

            gridMedia.Focus();
        }

        public void SetDrawingWindowPos(FrameworkElement fe, double acWidth, double acHeight)
        {
            Point pt = fe.TranslatePoint(new Point(0, 0), this);

            pt.Offset((fe.ActualWidth - acWidth) / 2, (fe.ActualHeight - acHeight) / 2);

            Point ptLT = new Point(pt.X + this.Left, pt.Y + this.Top);

            drawWindow.Left = ptLT.X;
            drawWindow.Top = ptLT.Y;

            drawWindow.Width = acWidth;
            drawWindow.Height = acHeight;
        }

        private void StopVideo()
        {
            gridMedia.Visibility = Visibility.Hidden;

            wPlayer.Stop();

            wPlayer.Close();
        }

        private void SavePresentation(string strFile)
        {
            if (presentation != null)
            {
                CombinePpt(presentation);

                presentation.Save(strFile);
            }
        }

        private void CombinePpt(Data.Presentation presentation)
        {
            for (int i = 0; i < presentation.Plays.Count; i++)
            {
                Data.PresentationPlay pPlay = presentation.Plays[i];

                for (int j = pPlay.Videos.Count - 1; j >= 0; j--)
                {
                    string strPPT = pPlay.Videos[j].ToString();

                    if (strPPT.EndsWith(".ppt", true, null) && strPPT.Contains('@'))
                    {
                        if (j > 0)
                        {
                            string strPrePPT = pPlay.Videos[j - 1].ToString();

                            if (GetPPTPath(strPPT) == GetPPTPath(strPrePPT))
                            {
                                pPlay.Videos.RemoveAt(j);
                            }
                            else
                            {
                                pPlay.Videos[j] = RemoveBitmapPath(strPPT);
                            }
                        }
                        else
                        {
                            pPlay.Videos[j] = RemoveBitmapPath(strPPT);
                        }
                    }
                }
            }
        }

        private string GetBitmapPath(string strPPT)
        {
            if (strPPT.Contains('@'))
            {
                string[] strArr = strPPT.Split('@');

                return strArr[0];
            }

            return string.Empty;
        }

        private string GetPPTPath(string strPPT)
        {
            if (strPPT.Contains('@'))
            {
                string[] strArr = strPPT.Split('@');

                return strArr[1];
            }

            return strPPT;
        }

        private string RemoveBitmapPath(string strPPT)
        {
            if (strPPT.EndsWith(".ppt", true, null) && strPPT.Contains('@'))
            {
                int index = strPPT.IndexOf('@');

                return strPPT.Remove(0, index + 1);
            }

            return strPPT;
        }

        private void PlayVideo(int indexPlay, int indexVideo)
        {
            string strFilePah = presentation.Plays[indexPlay].Videos[indexVideo].ToString();
            if (strFilePah.EndsWith(".ppt", true, null))
            {
                string strBitmap = GetBitmapPath(strFilePah);

                wPlayer.Stop();

                gridMedia.Visibility = Visibility.Hidden;

                canvasPowerPoint.Visibility = Visibility.Visible;
                canvasDrawing.Visibility = Visibility.Collapsed;

                if (System.IO.File.Exists(strBitmap))
                {
                    canvasPowerPoint.Source = GetBitmapImage(strBitmap);
                }
                else
                {
                    canvasPowerPoint.Source = null;
                }

                canvasPowerPoint.Focus();
            }
            else
            {
                wPlayer.Stop();

                gridMedia.Visibility = Visibility.Visible;

                gridMedia.InvalidateVisual();

                CheckVideo(indexPlay, indexVideo);

                string strVideoFile = presentation.Plays[indexPlay].Videos[indexVideo].ToString();

                wPlayer.Open2(strVideoFile);

                LoadVideoInfos();

                wPlayer.Run();

                gridMedia.Focus();
            }
        }

        private void UpdateTitle()
        {
            int playIndex = 0;
            int videoIndex = 0;
            GetIndex(out playIndex, out videoIndex);

            if (videoIndex >= 0)
            {
                drawWindow.Title.Foreground = Brushes.Red;
            }
            else
            {
                drawWindow.Title.Foreground = Brushes.Black;
            }

            drawWindow.Title.Text = string.Format("({0}/{1}) {2}", playIndex + 1, presentation.Plays.Count(), System.IO.Path.GetFileNameWithoutExtension(presentation.Plays[playIndex].PlayPath));

            if (videoIndex >= 0)
            {
                drawWindow.Title.Text += string.Format(" - {0}", System.IO.Path.GetFileName(presentation.Plays[playIndex].Videos[videoIndex].ToString()));
            }
        }

        public PresentationWindow(Webb.Playbook.Data.Presentation presentation)
            : this()
        {
            this.presentation = presentation;

            SplitPPT(presentation);

            Load(presentation.Plays[0].PlayPath);

            UpdateTitle();
        }

        void SplitPPT(Webb.Playbook.Data.Presentation pres)
        {
            if (pres != null)
            {
                for (int i = pres.Plays.Count - 1; i >= 0; i--)
                {
                    PresentationPlay pPlay = pres.Plays[i];

                    for (int k = 0; k < pPlay.Videos.Count(); k++)
                    {
                        if (pPlay.Videos[k].ToString().EndsWith("ppt", true, null))
                        {
                            string strPPT = pPlay.Videos[k].ToString();

                            int count = ConvertPptToJpg(pPlay.Videos[k].ToString());

                            string strJpgPath = new PowerPoint(pPlay.Videos[k].ToString()).ImagePath;

                            pPlay.Videos.RemoveAt(k);

                            int j = k;

                            foreach (string strJpgFile in System.IO.Directory.GetFiles(strJpgPath, "*.jpg"))
                            {
                                pPlay.Videos.Insert(j, strJpgFile + "@" + strPPT);

                                j++;
                            }

                            k = j;
                        }
                    }
                }
            }
        }

        private int GetTotalCount()
        {
            int count = 0;

            foreach (PresentationPlay play in presentation.Plays)
            {
                count++;
                count += play.Videos.Count;
            }

            return count;
        }

        private void GetIndex(out int playIndex, out int videoIndex)
        {
            playIndex = -1;
            videoIndex = -1;
            int tempIndex = 0;
            int tempPlayIndex = 0;

            if (Index >= 0 && Index < GetTotalCount())
            {
                foreach (PresentationPlay play in presentation.Plays)
                {
                    if (Index >= tempIndex && Index <= tempIndex + play.Videos.Count)
                    {
                        playIndex = tempPlayIndex;

                        if (Index != tempIndex)
                        {
                            videoIndex = Index - tempIndex - 1;
                        }

                        return;
                    }

                    tempPlayIndex++;
                    tempIndex++;
                    tempIndex += play.Videos.Count;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tool b = (sender as Button).Tag as Tool;

            if (b != null)
            {
                switch (b.Command)
                {
                    case Commands.PlayAni:
                        OriDrawing.Run();
                        break;
                    case Commands.StopAni:
                        OriDrawing.Stop();
                        break;
                    case Commands.Still:
                        OriDrawing.Still();
                        break;
                    case Commands.CloseAnimation:
                        CloseWindow();
                        break;
                    case Commands.SwitchRoutes:
                        OriDrawing.Stop();
                        switch (Opacity.ToString())
                        {
                            case "0":
                                Opacity = 0.2;
                                OriDrawing.TransparentRoute(0.2);
                                break;
                            case "0.2":
                                Opacity = 1;
                                OriDrawing.TransparentRoute(1);
                                break;
                            case "1":
                                Opacity = 0;
                                OriDrawing.TransparentRoute(0);
                                break;
                            default:
                                Opacity = 1;
                                OriDrawing.TransparentRoute(1);
                                break;
                        }
                        break;
                    case Commands.Color:
                        if (b.Color is SolidColorBrush)
                        {
                            freeDrawColor = (b.Color as SolidColorBrush).Color;
                            Webb.Playbook.Geometry.Shapes.Factory.Color = freeDrawColor;
                            b.Select();
                        }
                        break;
                }
            }
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tool b = (sender as Grid).Tag as Tool;

            if (b != null)
            {
                switch (b.Command)
                {
                    case Commands.CloseAnimation:
                        CloseWindow();
                        break;
                    case Commands.VideoRev:
                        wPlayer.RPlay5X();
                        break;
                    case Commands.VideoRPlay:
                        wPlayer.RPlay1X();
                        break;
                    case Commands.VideoRSlow:
                        wPlayer.RSlow5();
                        break;
                    case Commands.VideoStill:
                        wPlayer.Pause();
                        break;
                    case Commands.VideoSlow:
                        wPlayer.Slow5();
                        break;
                    case Commands.VideoPlay:
                        wPlayer.Play1X();
                        break;
                    case Commands.VideoFF:
                        wPlayer.Play5X();
                        break;
                    case Commands.VideoStop:
                        wPlayer.SetCurrentFrame(0);
                        StopCoachingPoint();
                        wPlayer.Stop2();
                        break;
                }
            }
        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Tool b = (sender as Grid).Tag as Tool;

            if (b != null)
            {
                switch (b.Command)
                {
                    case Commands.VideoRev:
                    case Commands.VideoRPlay:
                    case Commands.VideoRSlow:
                    //case Commands.VideoStill:
                    case Commands.VideoSlow:
                    //case Commands.VideoPlay:
                    case Commands.VideoFF:
                        //case Commands.VideoStop:
                        if (!PlayingCoachingPoint)
                        {
                            wPlayer.Play1X();
                        }
                        break;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CloseWindow();
                    break;
                case Key.Space:
                    {
                        int playIndex = 0;
                        int videoIndex = 0;
                        GetIndex(out playIndex, out videoIndex);

                        if (videoIndex < 0)
                        {
                            OriDrawing.Still();
                        }
                        else
                        {
                            wPlayer.Pause();
                        }
                    }
                    break;
                case Key.Enter:
                    OriDrawing.Run();
                    break;
                case Key.Left:
                    if (!e.IsRepeat)
                    {
                        wPlayer.RPlay5X();
                    }
                    break;
                case Key.Right:
                    if (!e.IsRepeat)
                    {
                        wPlayer.Play5X();
                    }
                    break;
            }

            if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                e.Handled = true;
            }
        }
        private int i = 0;

        void PresentationWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    this.btnPrevious_Click(null, null);
                    break;
                case Key.Down:
                    this.btnNext_Click(null, null);
                    break;
                case Key.Left:
                    if (!PlayingCoachingPoint)
                    {
                        wPlayer.Play1X();
                    }
                    break;
                case Key.Right:
                    if (!PlayingCoachingPoint)
                    {
                        wPlayer.Play1X();
                    }
                    break;
            }
        }

        public void CloseWindow()
        {
            messageFilter.RemoveThisMessageFilter();

            StopVideo();

            OriDrawing.Stop();

            OriDrawing.Canvas = OriCanvas;

            timer.Stop();

            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "PowerPoint"))
            {
                System.IO.Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "PowerPoint", true);
            }

            SavePresentation(Data.ProductInfo.PresentationPath + presentation.Name + ".Pres");

            this.Close();

            drawWindow.Close();

            Behavior.Owner = null;
        }

        private void gridContainer_Loaded(object sender, RoutedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void gridContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridDrawingContainer.Clip = new RectangleGeometry(new Rect(0, 0, gridDrawingContainer.ActualWidth, gridDrawingContainer.ActualHeight));
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OriDrawing != null)
            {
                OriDrawing.Stop();
            }

            Scale(e.NewValue);
            scale = e.NewValue;
        }

        private void Scale(double scale)
        {
            if (OriDrawing != null)
            {
                int nSpace = (int)((100 - scale) / 50 * 200);

                canvasDrawing.Margin = new Thickness(nSpace, 5, nSpace, 5);
            }
        }

        private void VisiblePlayControl()
        {
            int playIndex = 0;
            int videoIndex = 0;
            GetIndex(out playIndex, out videoIndex);

            if (videoIndex < 0)
            {
                toolBarColors.Visibility = Visibility.Visible;
                toolBarPlayControl.Visibility = Visibility.Visible;
                toolBarSize.Visibility = Visibility.Visible;
                toolBarPlayVideoControl.Visibility = Visibility.Collapsed;
                btnPresentationTools.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (presentation.Plays[playIndex].Videos[videoIndex].ToString().EndsWith(".ppt", true, null))
                {
                    toolBarColors.Visibility = Visibility.Visible;
                    toolBarPlayControl.Visibility = Visibility.Visible;
                    toolBarSize.Visibility = Visibility.Visible;
                    toolBarPlayVideoControl.Visibility = Visibility.Collapsed;
                    btnPresentationTools.Visibility = Visibility.Collapsed;
                }
                else
                {
                    toolBarColors.Visibility = Visibility.Visible;
                    toolBarPlayControl.Visibility = Visibility.Collapsed;
                    toolBarSize.Visibility = Visibility.Collapsed;
                    toolBarPlayVideoControl.Visibility = Visibility.Visible;
                    btnPresentationTools.Visibility = Visibility.Visible;
                }
            }
        }

        private DateTime previousTime = DateTime.Now;
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (Index > 0)
            {
                Index--;

                int playIndex = 0;
                int videoIndex = 0;
                GetIndex(out playIndex, out videoIndex);

                if (videoIndex < 0)
                {
                    VisiblePlayControl();
                    StopVideo();

                    Load(presentation.Plays[playIndex].PlayPath);
                }
                else
                {
                    VisiblePlayControl();
                    PlayVideo(playIndex, videoIndex);
                }

                EditCoachingPoint = false;

                StopCoachingPoint();

                UpdateTitle();
            }

            this.Focus();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Index < GetTotalCount() - 1)
            {
                Index++;

                int playIndex = 0;
                int videoIndex = 0;
                GetIndex(out playIndex, out videoIndex);

                if (videoIndex < 0)
                {
                    VisiblePlayControl();
                    StopVideo();

                    Load(presentation.Plays[playIndex].PlayPath);
                }
                else
                {
                    VisiblePlayControl();
                    PlayVideo(playIndex, videoIndex);
                }

                EditCoachingPoint = false;

                StopCoachingPoint();

                UpdateTitle();
            }

            this.Focus();
        }

        private void canvasOutLine_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    Point pt = e.GetPosition(drawWindow.Canvas);

            //    DrawLine(pt);
            //}
        }

        private void canvasOutLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Point pt = e.GetPosition(drawWindow.Canvas);

            //StartLine(pt);
        }

        private void canvasOutLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Point pt = e.GetPosition(drawWindow.Canvas);

            //ClearLines();
        }

        private void ClearLines()
        {
            if (previousLine != null)
            {
                drawWindow.Canvas.Children.Remove(previousLine);
            }
            else
            {
                for (int i = drawWindow.Canvas.Children.Count - 1; i >= 0; i--)
                {
                    if (drawWindow.Canvas.Children[i] is Polyline)
                    {
                        drawWindow.Canvas.Children.RemoveAt(i);
                    }
                }
            }

            previousLine = null;
        }

        private Polyline previousLine = null;
        private void StartLine(Point pt)
        {
            if (!EditCoachingPoint)
            {
                previousLine = new Polyline()
                {
                    Stroke = new SolidColorBrush(freeDrawColor),
                    StrokeThickness = 3,
                };

                previousLine.Points.Add(pt);
            }
        }

        private void DrawLine(Point pt)
        {
            if (!EditCoachingPoint)
            {
                if (previousLine != null)
                {
                    if (previousLine.Points != null && previousLine.Points.Count == 1)
                    {
                        drawWindow.Canvas.Children.Add(previousLine);
                    }

                    previousLine.Points.Add(pt);
                }
            }
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            // 12-05-2011 Scott
            if (DrawingWindow.OriDrawing != null && DrawingWindow.OriDrawing.Behavior != null && DrawingWindow.OriDrawing.Behavior.Drawing != null)
            {
                DrawingWindow.OriDrawing.Behavior.WndProc(gridVideo, this, hwnd, msg, wParam, lParam, ref b);
            }
            // end

            switch (msg)
            {
                case (int)MessageCommands.WM_LBUTTONDOWN:
                    {
                        int nPosX = (lParam.ToInt32() & 65535);
                        int nPosY = (lParam.ToInt32() >> 16);
                        Point pt = new Point(nPosX, nPosY);

                        if (gridMedia.IsVisible)
                        {
                            pt = gridVideo.TranslatePoint(pt, this);
                        }

                        StartLine(pt);
                    }
                    break;
                case (int)MessageCommands.WM_LBUTTONUP:
                    break;
                case (int)MessageCommands.WM_RBUTTONDOWN:
                    ClearLines();
                    break;
                case (int)MessageCommands.WM_RBUTTONUP:
                    break;
                case (int)MessageCommands.WM_MOUSEMOVE:
                    {
                        if (wParam.ToInt32() == 1) // left button down
                        {
                            int nPosX = (lParam.ToInt32() & 65535);
                            int nPosY = (lParam.ToInt32() >> 16);
                            Point pt = new Point(nPosX, nPosY);

                            if (gridMedia.IsVisible)
                            {
                                pt = gridVideo.TranslatePoint(pt, this);
                            }

                            DrawLine(pt);
                        }
                    }
                    break;
                case (int)MessageCommands.WM_KEYDOWN:
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.webbelectronics.com//");
        }

        private void btnPresentationTools_Click(object sender, RoutedEventArgs e)
        {
            bShowPresentationTools = !bShowPresentationTools;

            if (bShowPresentationTools)
            {
                colTools.Width = new GridLength(300);
            }
            else
            {
                colTools.Width = new GridLength(0);
            }
        }

        private void gridVideo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleVideoWindow();
        }

        private void btnCloseTools_Click(object sender, RoutedEventArgs e)
        {
            bShowPresentationTools = false;
            colTools.Width = new GridLength(0);
        }

        private VideoCoachingPointInfo GetCurrentVideoInfo(Data.Presentation pres)
        {
            int playIndex = 0;
            int videoIndex = 0;
            GetIndex(out playIndex, out videoIndex);

            if (videoIndex >= 0)
            {
                object objVideo = pres.Plays[playIndex].Videos[videoIndex];

                VideoCoachingPointInfo vcpi = objVideo as VideoCoachingPointInfo;

                if (vcpi == null)
                {
                    vcpi = new VideoCoachingPointInfo(objVideo.ToString());

                    pres.Plays[playIndex].Videos.RemoveAt(videoIndex);
                    pres.Plays[playIndex].Videos.Insert(videoIndex, vcpi);
                }

                return vcpi;
            }

            return null;
        }

        private void btnAddCoachingPoint_Click(object sender, RoutedEventArgs e)
        {
            VideoCoachingPointInfo vcpi = GetCurrentVideoInfo(presentation);

            int nVideoState = wPlayer.GetCurrentState();

            if (nVideoState == (int)VideoState.state_Stop)
            {
                return;
            }

            if (nVideoState != (int)VideoState.state_Still)
            {
                wPlayer.Pause();
            }

            // 11-30-2011 Scott
            int nFrame = wPlayer.GetCurrentFrame();

            if (vcpi == null || vcpi.CheckFrame(nFrame) != null)
            {
                MessageBox.Show("This has been a coaching point near this point.");

                return;
            }

            EditVCP = new VideoCoachingPoint(vcpi.VideoPath, nFrame, true);

            EditCoachingPoint = true;
        }

        private void btnEditCoachingPoint_Click(object sender, RoutedEventArgs e)
        {
            VideoCoachingPoint vcp = this.lbCoachingPoitList.SelectedItem as VideoCoachingPoint;
            VideoCoachingPointInfo videoInfo = this.lbCoachingPoitList.DataContext as VideoCoachingPointInfo;

            if (vcp != null && videoInfo != null)
            {
                EditVCP = vcp.Copy();

                int nTip = 2;

                int nCurrentFrame = wPlayer.GetCurrentFrame();

                if (nCurrentFrame <= EditVCP.Frame + nTip && nCurrentFrame >= EditVCP.Frame - nTip)
                {

                }
                else
                {
                    wPlayer.SetCurrentFrame(EditVCP.Frame - nTip);

                    int nCurrentState = wPlayer.GetCurrentState();

                    if (PlayingCoachingPoint)
                    {
                        wPlayer.Pause();

                        wPlayer.SetCurrentFrame(EditVCP.Frame - nTip);
                    }
                }

                EditCoachingPoint = true;
            }
        }

        private void btnDeleteCoachingPoint_Click(object sender, RoutedEventArgs e)
        {
            VideoCoachingPoint vcp = this.lbCoachingPoitList.SelectedItem as VideoCoachingPoint;
            VideoCoachingPointInfo videoInfo = this.lbCoachingPoitList.DataContext as VideoCoachingPointInfo;

            if (vcp != null && videoInfo != null)
            {
                if (PlayingCoachingPoint)
                {
                    StopCoachingPoint();
                }

                if (EditCoachingPoint && EditVCP == vcp)
                {
                    EditCoachingPoint = false;
                }

                this.lbCoachingPoitList.Items.Remove(this.lbCoachingPoitList.SelectedItem);

                videoInfo.RemoveCoachingPoint(vcp);

                this.gridProgress.Children.Remove(vcp.Shape);
            }
        }

        private void AddCoachingPointShape(VideoCoachingPoint vcp)
        {
            int nTotalFrame = this.wPlayer.GetTotalFrames();

            PointCollection points = new PointCollection();
            points.Add(new Point(VideoCoachingPointInfo.ShapeSize / 2, 0));
            points.Add(new Point(0, VideoCoachingPointInfo.ShapeSize));
            points.Add(new Point(VideoCoachingPointInfo.ShapeSize, VideoCoachingPointInfo.ShapeSize));
            System.Windows.Shapes.Polygon triangle = new Polygon()
            {
                Points = points,
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                DataContext = vcp,
            };

            vcp.Shape = triangle;

            triangle.Margin = new Thickness(vcp.Frame * this.gridProgress.ActualWidth / nTotalFrame - VideoCoachingPointInfo.ShapeSize / 2, 0, 0, 0);

            gridProgress.Children.Add(triangle);
        }

        private void LoadVideoInfos()
        {
            StopCoachingPoint();
            ClearCoachingPointShapes();

            Data.VideoCoachingPointInfo vcpi = this.GetCurrentVideoInfo(presentation);

            lbCoachingPoitList.Items.Clear();
            lbCoachingPoitList.DataContext = vcpi;

            foreach (Data.VideoCoachingPoint vcp in vcpi.VideoCoachingPoints)
            {
                lbCoachingPoitList.Items.Add(vcp);

                AddCoachingPointShape(vcp);
            }
        }

        private void ClearCoachingPointShapes()
        {
            for (int i = gridProgress.Children.Count - 1; i >= 0; i--)
            {
                if (gridProgress.Children[i] is Shape && gridProgress.Children[i] != rectProgress)
                {
                    gridProgress.Children.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Show help document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Comfirm create coaching point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            VideoCoachingPointInfo vcpi = GetCurrentVideoInfo(presentation);

            if (vcpi != null && EditVCP != null)
            {
                if (!vcpi.Contains(EditVCP))
                {
                    vcpi.VideoCoachingPoints.Add(EditVCP);

                    this.lbCoachingPoitList.Items.Add(EditVCP);

                    AddCoachingPointShape(EditVCP);

                    if (DrawingWindow.OriDrawing != null)
                    {
                        DrawingWindow.OriDrawing.Save(EditVCP.DrawingFile);
                    }
                }
                else
                {
                    VideoCoachingPoint vcp = this.lbCoachingPoitList.SelectedItem as VideoCoachingPoint;

                    if (vcp != null)
                    {
                        vcp.Apply(EditVCP);

                        if (DrawingWindow.OriDrawing != null)
                        {
                            DrawingWindow.OriDrawing.Save(EditVCP.DrawingFile);
                        }
                    }
                }
            }

            wPlayer.Play1X();

            EditCoachingPoint = false;
        }

        /// <summary>
        /// Cancel create coaching point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            wPlayer.Play1X();

            EditCoachingPoint = false;
        }

        /// <summary>
        /// Start and stop capture audio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCaptureAudio_Click(object sender, RoutedEventArgs e)
        {
            if (EditVCP != null)
            {
                if (!IsAudioEnded())
                {
                    MessageBox.Show("Playing Audio...");

                    return;
                }
                else
                {
                    StopAudio();
                }

                Recording = !Recording;

                if (Recording)
                {
                    imgRecord.Effect = new System.Windows.Media.Effects.BlurEffect();

                    recorder = new Webb.Playbook.DX.SoundRecord();

                    recorder.SetFileName(EditVCP.AudioFile);

                    recorder.RecStart();
                }
                else
                {
                    StopCapture();
                }
            }
        }

        public bool IsAudioEnded()
        {
            if (audio == null)
            {
                return true;
            }
            else
            {
                if (audio.CurrentPosition * 10000000 < audio.StopPosition)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void StopCapture()
        {
            imgRecord.Effect = null;

            if (recorder != null)
            {
                recorder.RecStop();

                recorder = null;
            }
        }

        /// <summary>
        /// Play the captured audio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayAudio_Click(object sender, RoutedEventArgs e)
        {
            PlayAudio(EditVCP);
        }

        private void PlayAudio(VideoCoachingPoint vcp)
        {
            if (vcp != null)
            {
                if (Recording)
                {
                    MessageBox.Show("Recording Audio...");

                    return;
                }

                StopAudio();

                if (System.IO.File.Exists(vcp.AudioFile))
                {
                    audio = new Microsoft.DirectX.AudioVideoPlayback.Audio(vcp.AudioFile);

                    audio.Play();

                    audio.Ending += new EventHandler(audio_Ending);

                    tbAudioState.Text = "Playing Audio...";
                }
            }
        }

        void audio_Ending(object sender, EventArgs e)
        {
            tbAudioState.Text = string.Empty;
        }

        private void StopAudio()
        {
            if (audio != null)
            {
                audio.Stop();

                audio.Dispose();

                audio = null;

                tbAudioState.Text = string.Empty;
            }
        }

        /// <summary>
        /// Pause and resume audio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPauseAudio_Click(object sender, RoutedEventArgs e)
        {
            if (EditVCP != null)
            {
                if (Recording)
                {
                    MessageBox.Show("Recording Audio...");
                    
                    return;
                }

                PauseAudio();
            }
        }

        private void PauseAudio()
        {
            if (!IsAudioEnded())
            {
                if (audio.State == Microsoft.DirectX.AudioVideoPlayback.StateFlags.Running)
                {
                    audio.Pause();

                    tbAudioState.Text = "Playing Audio...(Pause)";
                }
                else if (audio.State == Microsoft.DirectX.AudioVideoPlayback.StateFlags.Paused)
                {
                    audio.Play();

                    tbAudioState.Text = "Playing Audio...";
                }
            }
        }

        private void btnStopAudio_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAudioEnded())
            {
                StopAudio();

                tbAudioState.Text = string.Empty;
            }
        }

        private void btnDeleteAudio_Click(object sender, RoutedEventArgs e)
        {
            if (EditVCP == null)
            {
                return;
            }

            if (Recording)
            {
                MessageBox.Show("Recording Audio...");

                return;
            }

            if (!IsAudioEnded())
            {
                MessageBox.Show("Playing Audio...");

                return;
            }

            if (System.IO.File.Exists(EditVCP.AudioFile))
            {
                if (MessageBox.Show("Do you want to delete current audio file ?", "Playbook", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.IO.File.Delete(EditVCP.AudioFile);
                }
            }
        }

        private void Button_Shape_Click(object sender, RoutedEventArgs e)
        {
            if (EditVCP != null)
            {
                FrameworkElement fe = sender as FrameworkElement;

                if (fe != null)
                {
                    CPShape shape = fe.DataContext as CPShape;

                    if (shape != null)
                    {
                        switch (shape.Text)
                        {
                            case "Hand":
                                SetBehavior("Dragger");
                                break;
                            case "Line":
                                SetBehavior("ClassicLine");
                                break;
                            case "ArrowLine":
                                SetBehavior("ClassicArrowLine");
                                break;
                            case "BlockLine":
                                SetBehavior("ClassicBlockLine");
                                break;
                            case "Square":
                                Webb.Playbook.Geometry.Shapes.PBRect rect = Webb.Playbook.Geometry.Shapes.Factory.CreateRectShape(DrawingWindow.OriDrawing, new Point(0, 0));
                                rect.Width = 4;
                                rect.Height = 4;
                                DrawingWindow.OriDrawing.Add(rect, true);
                                SetBehavior("Dragger");
                                break;
                            case "Circle":
                                Webb.Playbook.Geometry.Shapes.PBCircle circle = Webb.Playbook.Geometry.Shapes.Factory.CreateCircleShape(DrawingWindow.OriDrawing, new Point(0, 0));
                                circle.Radius = 2;
                                DrawingWindow.OriDrawing.Add(circle, true);
                                SetBehavior("Dragger");
                                break;
                        }
                    }
                }
            }
        }

        public void SetBehavior(string behavior)
        {
            if (drawWindow != null)
            {
                drawWindow.SetBehavior(behavior);
            }
        }

        private void Button_TextBox_Click(object sender, RoutedEventArgs e)
        {
            if (EditVCP != null)
            {
                FrameworkElement fe = sender as FrameworkElement;

                if (fe != null)
                {
                    CPTextBox textBox = fe.DataContext as CPTextBox;

                    if (textBox != null)
                    {
                        switch (textBox.Text)
                        {
                            case "Bubble":
                                SetBehavior("Text");
                                break;
                        }
                    }
                }
            }
        }

        private void btnDragger_Click(object sender, RoutedEventArgs e)
        {
            SetBehavior("Dragger");
        }

        private void btnPauseCaptureAudio_Click(object sender, RoutedEventArgs e)
        {
            if (recorder != null)
            {
                if (recorder.Capturing.Value)
                {
                    recorder.RecPause();

                    tbAudioState.Text = "Recording...(Pause)";
                }
                else
                {
                    recorder.RecContinue();

                    tbAudioState.Text = "Recording...";
                }
            }
        }
    }
}
