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
using System.Windows.Media.Animation;

namespace Webb.Playbook
{
    /// <summary>
    /// SplashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SplashWindow : Window
    {
        Storyboard story;

        public SplashWindow()
        {
            InitializeComponent();

            ball.Effect = new System.Windows.Media.Effects.DropShadowEffect();

            this.Loaded += new RoutedEventHandler(SplashWindow_Loaded);
        }

        void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0;

            Duration duration = new Duration(TimeSpan.FromSeconds(1));

            DoubleAnimation OpercityAnimation = new DoubleAnimation(0, 1.00, duration);

            OpercityAnimation.Completed += new EventHandler(OpercityAnimation_Completed);

            this.BeginAnimation(Window.OpacityProperty, OpercityAnimation);
        }

        void OpercityAnimation_Completed(object sender, EventArgs e)
        {
            DoubleAnimation StandAnimation = new DoubleAnimation(1.00, 1.00, new Duration(TimeSpan.FromSeconds(3)));

            StandAnimation.Completed += new EventHandler(StandAnimation_Completed);

            this.BeginAnimation(Window.OpacityProperty, StandAnimation);

            //ball animation
            story = new Storyboard();
            this.ball.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform scale = new ScaleTransform(1, 1);
            this.ball.RenderTransform = scale;
            NameScope.SetNameScope(ball, new NameScope());
            ball.RegisterName("scale", scale);

            double sec = 0.3;
            Duration duration = new Duration(TimeSpan.FromSeconds(sec));

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.From = 1;
            animationY.From = 1;
            animationX.To = 1.1;
            animationY.To = 1.1;
            animationX.Duration = duration;
            animationY.Duration = duration;

            story = new Storyboard();
            story.Duration = new Duration(TimeSpan.FromSeconds(sec));
            story.Children.Add(animationX);
            story.Children.Add(animationY);
            story.AutoReverse = true;
            story.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(animationX, "scale");
            Storyboard.SetTargetName(animationY, "scale");
            Storyboard.SetTargetProperty(animationX, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(ScaleTransform.ScaleYProperty));
            story.Begin(ball, true);
        }

        void StandAnimation_Completed(object sender, EventArgs e)
        {
            DoubleAnimation ReverseOpercityAnimation = new DoubleAnimation(1.00, 0.5, new Duration(TimeSpan.FromSeconds(0.5)));

            ReverseOpercityAnimation.Completed += new EventHandler(ReverseOpercityAnimation_Completed);

            this.BeginAnimation(Window.OpacityProperty, ReverseOpercityAnimation);
        }

        void ReverseOpercityAnimation_Completed(object sender, EventArgs e)
        {
            if (story != null)
            {
                story.Stop();
            }
            this.Close();
        }
    }
}
