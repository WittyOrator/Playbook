using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ColorPickerControl
{
    public partial class ColorPicker : UserControl
    {
        private bool isMouseDownOnCanvas;
        private bool isMouseDownOnRainbow; 
        private double colorCanvasXProperty;
        private double colorCanvasYProperty;
        private double colorHueYProperty;
        private SolidColorBrush baseHueProperty;
        private bool isCompactProperty;
        private bool bInnerInvoke = false;
        public bool UpdateColor = false;

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(ColorPicker),
            new PropertyMetadata(Colors.White, new PropertyChangedCallback(OnColorPropertyChanged)));

        public static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cp = d as ColorPicker;

            Color color = (Color)e.NewValue;

            if (!cp.bInnerInvoke)
            {
                cp.UpdateFromColor(color);
            }

            if (cp.resultCanvas != null)
            {
                cp.resultCanvas.Background = new SolidColorBrush(color);
            }
        }

        private Color startColor;

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        Storyboard story = new Storyboard();

        public ColorPicker()
        {
            InitializeComponent();

            colorCanvasXProperty = 1;
            colorHueYProperty = 1;
            ColorCanvasY = 1;
            isMouseDownOnCanvas = false;
            this.UpdateLayout();
            if(RainbowBorder.ActualWidth > 1)
                RainbowHandle.Width = RainbowBorder.ActualWidth - 1;

            this.Visibility = Visibility.Hidden;

            //create story
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform scale = new ScaleTransform(0.1, 0.1);
            this.RenderTransform = scale;
            this.RegisterName("scale", scale);

            double sec = 0.5;
            Duration duration = new Duration(TimeSpan.FromSeconds(sec));

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.From = 0.1;
            animationY.From = 0.1;
            animationX.To = 1.0;
            animationY.To = 1.0;
            animationX.Duration = duration;
            animationY.Duration = duration;

            story.Duration = new Duration(TimeSpan.FromSeconds(sec));
            story.Children.Add(animationX);
            story.Children.Add(animationY);
            Storyboard.SetTargetName(animationX, "scale");
            Storyboard.SetTargetName(animationY, "scale");
            Storyboard.SetTargetProperty(animationX, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(ScaleTransform.ScaleYProperty));

            this.Resources.Add("colorStory", story);
        }

        #region ColorChanged Event code

        private event ColorChangedEventHandler ColorChangedEvent;

        public event ColorChangedEventHandler ColorChanged
        {
            add { ColorChangedEvent += value; }
            remove { ColorChangedEvent -= value; }
        }

        protected virtual void OnColorChanged(ColorChangedEventArgs e)
        {
            if (ColorChangedEvent != null)
            {
                ColorChangedEvent(this, e);
            }
        }
        #endregion

        #region Dependency Properties

        private double ColorCanvasX
        {
            get { return colorCanvasXProperty; }
            set
            {
                colorCanvasXProperty = value;
                UpdateSelectedColor();
            }
        }

        private double ColorCanvasY
        {
            get { return colorCanvasYProperty; }
            set
            {
                colorCanvasYProperty = value;
                UpdateSelectedColor();
            }
        }

        private SolidColorBrush BaseHue
        {
            get { return baseHueProperty; }
            set
            {
                baseHueProperty = value;
                UpdateSelectedColor();
                if (BackgroundCanvas != null)
                {
                    BackgroundCanvas.Background = baseHueProperty;
                }
            }
        }

        public bool IsCompact
        {
            get { return isCompactProperty; }
            set 
            { 
                isCompactProperty = value;
                if (LayoutRoot != null)
                {
                    CompactLayoutChange(value);
                }
            }
        }

        private void CompactLayoutChange(bool compactChange)
        {
            Point oldColorPosition = new Point(Canvas.GetLeft(FinalColor),
                                                Canvas.GetTop(FinalColor));
            Point oldHuePosition = new Point(0.0,
                                                Canvas.GetTop(RainbowHandle)); 
            Size oldColorCanvasSize = new Size(this.ColorCanvas.ActualWidth,
                                                this.ColorCanvas.ActualHeight);
            Size oldHueCanvasSize = new Size(this.HueCanvas.ActualWidth,
                                                this.HueCanvas.ActualHeight); 
            
            if (compactChange)
            {
                LargePanel.Visibility = Visibility.Collapsed;
                CompactPanel.Visibility = Visibility.Visible;
                rightColumn.MinWidth = 0;
                RootControl.MinHeight = 0;
                RootControl.MinWidth = 0;
                RootControl.Width = 130;
                RootControl.Height = 200;                                          
            }
            else
            {
                LargePanel.Visibility = Visibility.Visible;
                CompactPanel.Visibility = Visibility.Collapsed;
                rightColumn.MinWidth = 90;
                RootControl.Height = double.NaN;
                RootControl.Width = double.NaN;
                RootControl.MinHeight = 190;
                RootControl.MinWidth = 240;
            }
            this.UpdateLayout();

            RealignElement(oldColorPosition, 
                            oldColorCanvasSize, 
                            new Size(this.ColorCanvas.ActualWidth, this.ColorCanvas.ActualHeight),
                            (UIElement)this.FinalColor);
            RealignElement(oldHuePosition,
                            oldHueCanvasSize,
                            new Size(this.HueCanvas.ActualWidth, this.HueCanvas.ActualHeight),
                            (UIElement)this.RainbowHandle);

            if (RainbowBorder.ActualWidth > 1) 
                RainbowHandle.Width = RainbowBorder.ActualWidth - 1;

        }

        private void RealignElement(Point oldPosition,
                                    Size oldCanvasSize,
                                    Size newCanvasSize,
                                    UIElement elementToRealign)
        {
            //OK... so we find the old size and the old position, turn them into a
            // percentage an apply the new position based on the new size
            if ((oldCanvasSize.Width != 0) && (oldCanvasSize.Height != 0))
            {

                double relativeX = oldPosition.X / oldCanvasSize.Width;
                double relativeY = oldPosition.Y / oldCanvasSize.Height;

                Canvas.SetLeft(elementToRealign, (newCanvasSize.Width * relativeX));
                Canvas.SetTop(elementToRealign, (newCanvasSize.Height * relativeY));
            }
        }

#endregion

        #region Update Color methods
        private void UpdateSelectedColor()
        {
            if (baseHueProperty == null)
            {
                UpdateColorCanvas(150, 0);
            }
            
            Color baseColor = ((System.Windows.Media.SolidColorBrush)(baseHueProperty)).Color;

            Color newColor = new Color();
            if (colorCanvasXProperty > 1.0)
            {
                colorCanvasXProperty = 1.0;
            }

            if (colorCanvasYProperty > 1.0)
            {
                colorCanvasYProperty = 1.0;
            }

            newColor = ColorConverter.HSLtoRGB(colorHueYProperty, 1 - colorCanvasXProperty, colorCanvasYProperty);

            SolidColorBrush updatedColor = new SolidColorBrush(newColor);

            //OnColorChanged(new ColorChangedEventArgs(SelectedColor, updatedColor));
            //SelectedColor = new SolidColorBrush(newColor);

            OnColorChanged(new ColorChangedEventArgs(new SolidColorBrush(Color), updatedColor));
            bInnerInvoke = true;
            Color = newColor;
            bInnerInvoke = false;

            //if (CopyColorText != null)
            //{
                RedText.Text = newColor.R.ToString();
                GreenText.Text = newColor.G.ToString();
                BlueText.Text = newColor.B.ToString();
                
                /*CopyColorText.Text = */CompactRGBText.Text = "" + newColor.R.ToString() + "," + newColor.G.ToString() + "," + newColor.B.ToString();
                /*CopyColorText.Text = */CompactRGBText.Text = "" + baseHueProperty.Color.R.ToString() + "," + baseHueProperty.Color.G.ToString() + "," + baseHueProperty.Color.B.ToString();  
                CopyHexText.Text = CompactHexText.Text = newColor.ToString();
            //}
        }

        //private void UpdateColorCanvas(object sender, RoutedEventArgs e)
        private void UpdateColorCanvas(double max, double position)
        {
            Color targetColor = new Color();

            if (position > max)
                position = max;

            colorHueYProperty = 1 - position / max;
            targetColor = ColorConverter.HSLtoRGB(colorHueYProperty, 1, 1);

            BaseHue = new SolidColorBrush(targetColor);
        }

        #endregion
               
        #region Color Canvas Mouse Event Handlers

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDownOnCanvas = false;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Let the dragging issue begin
            isMouseDownOnCanvas = true;
            Canvas backgroundCanvas = sender as Canvas;

            //move the little circular canvas thingy
            Point newGridPoint = e.GetPosition(backgroundCanvas);
            Canvas.SetTop(this.FinalColor, (newGridPoint.Y - 6));
            Canvas.SetLeft(this.FinalColor, (newGridPoint.X - 6));
                       
            //Set the new Brush
            colorCanvasXProperty = (Math.Abs(backgroundCanvas.ActualWidth - newGridPoint.X))/backgroundCanvas.ActualWidth;
            ColorCanvasY = (Math.Abs(newGridPoint.Y - backgroundCanvas.ActualHeight)) / backgroundCanvas.ActualHeight;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDownOnCanvas)
            {
                Canvas backgroundCanvas = sender as Canvas;

                Point newGridPoint = e.GetPosition(backgroundCanvas);
                Canvas.SetTop(this.FinalColor, (newGridPoint.Y - 6));
                Canvas.SetLeft(this.FinalColor, (newGridPoint.X - 6));

                //Set the new Brush
                colorCanvasXProperty = (Math.Abs(backgroundCanvas.ActualWidth - newGridPoint.X)) / backgroundCanvas.ActualWidth;
                ColorCanvasY = (Math.Abs(newGridPoint.Y - backgroundCanvas.ActualHeight)) / backgroundCanvas.ActualHeight;
            }
        }
        
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //find out which side the mouse left
            if (isMouseDownOnCanvas)
            {
                if ((colorCanvasXProperty - 1) > -.1)
                {
                    colorCanvasXProperty = 1;
                }
                else if ((colorCanvasXProperty - .1) < 0)
                {
                    colorCanvasXProperty = 0;
                }
                
                if ((colorCanvasYProperty - 1) > -.1)
                {
                    colorCanvasYProperty = 1;
                }
                else if ((colorCanvasYProperty - .1) < 0)
                {
                    colorCanvasYProperty = 0;
                }

                ColorCanvasY = colorCanvasYProperty;
            }
        }
        
        private void ColorCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RealignElement(new Point(Canvas.GetLeft(FinalColor),
                                     Canvas.GetTop(FinalColor)),
                         e.PreviousSize,
                         e.NewSize,
                         (UIElement)FinalColor);
        }

        #endregion

        #region Rainbow Border Event Handlers

        private void RainbowBorder_TurnOn(object sender, MouseButtonEventArgs e)
        {
            isMouseDownOnRainbow = true;

            FrameworkElement thisRainbowBorder = (FrameworkElement)sender;
            Point mousePosInRainbow = e.GetPosition(thisRainbowBorder);
            UpdateColorCanvas(thisRainbowBorder.ActualHeight, (thisRainbowBorder.ActualHeight - mousePosInRainbow.Y));
            Canvas.SetTop(this.RainbowHandle, (mousePosInRainbow.Y - (this.RainbowHandle.ActualHeight / 2)));

        }

        private void RainbowBorder_TurnOff(object sender, MouseButtonEventArgs e)
        {
            isMouseDownOnRainbow = false;

        }

        private void RainbowBorder_UpdateHue(object sender, MouseEventArgs e)
        {
            if (isMouseDownOnRainbow)
            {
                FrameworkElement thisRainbowBorder = (FrameworkElement)sender;
                Point mousePosInRainbow = e.GetPosition(thisRainbowBorder);
                if ((mousePosInRainbow.Y < this.RainbowBorder.ActualHeight) && (mousePosInRainbow.Y > 0))
                {
                    UpdateColorCanvas(thisRainbowBorder.ActualHeight, (thisRainbowBorder.ActualHeight - mousePosInRainbow.Y));
                    Canvas.SetTop(this.RainbowHandle, (mousePosInRainbow.Y - (this.RainbowHandle.ActualHeight / 2)));
                }
            }
        }

        private void HueCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RealignElement(new Point(0.0,
                                     Canvas.GetTop(RainbowHandle)),
                         e.PreviousSize,
                         e.NewSize,
                         (UIElement)RainbowHandle);

            if (RainbowBorder.ActualWidth > 1) 
                RainbowHandle.Width = RainbowBorder.ActualWidth - 1;

            if (e.NewSize.Height < 100)
            {
                RainbowHandle.Height = 10;
            }
            else if (e.NewSize.Height > 300)
            {
                RainbowHandle.Height = 30;
            }
            else
            {
                RainbowHandle.Height = e.NewSize.Height / 10;
            }
        }

        #endregion

        private void TurnEverythingOff(object sender, MouseButtonEventArgs e)
        {
            isMouseDownOnRainbow = false;
            isMouseDownOnCanvas = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HueCanvas.Clip = new RectangleGeometry(new Rect(0, 0, HueCanvas.ActualWidth, HueCanvas.ActualHeight));

            UpdateFromColor(Color);
        }

        private void CopyColorText_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void UpdateFromColor(Color color)
        {
            double hue = 1;
            double saturation = 1;
            double luminance = 1;
            ColorConverter.RGBtoHSL(color, out hue, out saturation, out luminance);

            double x = ColorCanvas.ActualWidth * (saturation);
            double y = ColorCanvas.ActualHeight * (1 - luminance);
            double yHue = HueCanvas.ActualHeight * (hue);

            Canvas.SetLeft(this.FinalColor, x - (this.FinalColor.ActualHeight / 2));
            Canvas.SetTop(this.FinalColor, y - (this.FinalColor.ActualHeight / 2));
            Canvas.SetTop(this.RainbowHandle, yHue - (this.RainbowHandle.ActualHeight / 2));
            colorCanvasXProperty = 1 - saturation;
            colorHueYProperty = 1 - hue;
            ColorCanvasY = luminance;
            UpdateColorCanvas(HueCanvas.ActualHeight, HueCanvas.ActualHeight * (1 - hue));
        }

        public void Show()
        {
            if (this.Visibility == Visibility.Visible)
            {
                return;
            }

            this.Visibility = Visibility.Visible;
            startColor = Color;
            story.Begin(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SetValue(ColorProperty, startColor);
            this.Visibility = Visibility.Hidden;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void ColorText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    int r = int.Parse(RedText.Text);
                    int g = int.Parse(GreenText.Text);
                    int b = int.Parse(BlueText.Text);

                    if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                    {
                        return;
                    }

                    Color color = new Color()
                    {
                        R = (Byte)r,
                        G = (Byte)g,
                        B = (Byte)b,
                    };

                    UpdateFromColor(color);
                }
                catch
                {
                    return;
                }
            }
        }
    }

    public class ColorConverter
    {
        public static void RGBtoHSL(Color color, out double hue, out double saturation, out double luminance)
        {
            hue = 1;
            saturation = 1;
            luminance = 1;

            double[] rgb = new double[3];
            rgb[0] = color.R;
            rgb[1] = color.G;
            rgb[2] = color.B;
            double max = rgb.Max();
            double min = rgb.Min();
            double mid = 0;
            bool bFindMax = false;
            bool bFindMin = false;
            foreach (double d in rgb)
            {
                if (d == max && !bFindMax)
                {
                    bFindMax = true;
                }
                else if (d == min && !bFindMin)
                {
                    bFindMin = true;
                }
                else
                {
                    mid = d;
                }
            }
           
            luminance = max / 255;
            saturation = 1 - min / luminance / 255;

            double q = (max - min) / 255;
            if (q != 0)
            {
                if (color.R > color.G && color.G >= color.B)
                {
                    hue = (mid - min) / 1530 / q;

                    return;
                }

                if (color.G >= color.R && color.R >= color.B)
                {
                    hue = (max - mid) / 1530 / q + (double)1 / 6;

                    return;
                }

                if (color.G >= color.B && color.B >= color.R)
                {
                    hue = (mid - min) / 1530 / q + (double)1 / 3;

                    return;
                }

                if (color.B >= color.G && color.G >= color.R)
                {
                    hue = (max - mid) / 1530 / q + 0.5;

                    return;
                }

                if (color.B >= color.R && color.R >= color.G)
                {
                    hue = (mid - min) / 1530 / q + (double)2 / 3;
                    
                    return;
                }

                if (color.R >= color.B && color.B >= color.G)
                {
                    hue = (max - mid) / 1530 / q + (double)5 / 6;

                    return;
                }
            }
        }

        public static Color HSLtoRGB(double hue, double saturation, double luminance)
        {
            int Max, Mid, Min;
            double q;

            Max = Round(luminance * 255);
            Min = Round((1.0 - saturation) * (luminance / 1.0) * 255);
            q = (double)(Max - Min) / 255;

            if (hue >= 0 && hue <= (double)1 / 6)
            {
                Mid = Round(((hue - 0) * q) * 1530 + Min);
                return Color.FromArgb(255, (byte)Max, (byte)Mid, (byte)Min);
            }
            else if (hue <= (double)1 / 3)
            {
                Mid = Round(-((hue - (double)1 / 6) * q) * 1530 + Max);
                return Color.FromArgb(255, (byte)Mid, (byte)Max, (byte)Min);
            }
            else if (hue <= 0.5)
            {
                Mid = Round(((hue - (double)1 / 3) * q) * 1530 + Min);
                return Color.FromArgb(255, (byte)Min, (byte)Max, (byte)Mid);
            }
            else if (hue <= (double)2 / 3)
            {
                Mid = Round(-((hue - 0.5) * q) * 1530 + Max);
                return Color.FromArgb(255, (byte)Min, (byte)Mid, (byte)Max);
            }
            else if (hue <= (double)5 / 6)
            {
                Mid = Round(((hue - (double)2 / 3) * q) * 1530 + Min);
                return Color.FromArgb(255, (byte)Mid, (byte)Min, (byte)Max);
            }
            else if (hue <= 1.0)
            {
                Mid = Round(-((hue - (double)5 / 6) * q) * 1530 + Max);
                return Color.FromArgb(255, (byte)Max, (byte)Min, (byte)Mid);
            }
            else
            {
                return Color.FromArgb(255, (byte)0, (byte)0, (byte)0);
            }
        }
        private static int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
            {
                ret_val += 1;
            }

            return ret_val;
        }
    }

    #region ColorChangedEvent stuff

    public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);
    
    public class ColorChangedEventArgs : EventArgs
    {
        public ColorChangedEventArgs(SolidColorBrush oldColor, SolidColorBrush newColor)
        {
            this.oldColor = oldColor;
            this.newColor = newColor;
        }

        // The fire event will have two pieces of information--
        // 1) Where the fire is, and 2) how "ferocious" it is.

        public SolidColorBrush oldColor;
        public SolidColorBrush newColor;

    }
    #endregion

    #region ClickTextBox class

    public class ClickTextBox : TextBox
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = false;
            this.SelectAll();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            e.Handled = false;
        }
    }
    #endregion
}
