using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Webb.Playbook.Geometry.Shapes;
using Microsoft.Samples.CustomControls;

namespace Webb.Playbook.Geometry
{
    public abstract class Behavior
    {
        private static ReadOnlyCollection<Behavior> mSingletons;
        public static ReadOnlyCollection<Behavior> Singletons
        {
            get
            {
                if (mSingletons == null)
                {
                    mSingletons = InitializeBehaviors();
                }
                return mSingletons;
            }
        }

        private static Window owner;
        public static Window Owner
        {
            get
            {
                return owner == null ? Application.Current.MainWindow : owner; 
            }
            set { owner = value; }
        }

        public static bool DrawVideo
        {
            get { return owner != null; }
        }

        private static ReadOnlyCollection<Behavior> InitializeBehaviors()
        {
            List<Behavior> result = new List<Behavior>();
            Type basic = typeof(Behavior);

            foreach (Type t in basic.Assembly.GetTypes())
            {
                if (basic.IsAssignableFrom(t) && !t.IsAbstract)
                {
                    Behavior instance = Activator.CreateInstance(t) as Behavior;

                    result.Add(instance);
                }
            }

            //Set Order

            return result.AsReadOnly();
        }

        public static Behavior LookupByName(string name)
        {
            return Singletons.Where(x => x.Name == name).FirstOrDefault();
        }

        public abstract string Name
        {
            get;
        }

        public virtual string HintText
        {
            get
            {
                return "";
            }
        }

        public virtual Color ButtonColor
        {
            get
            {
                return Color.FromArgb(255, 200, 200, 200);
            }
        }

        public static double IconSize
        {
            get
            {
                return 25;
            }
        }

        public Panel Icon
        {
            get
            {
                return CreateIcon();
            }
        }

        public virtual Panel CreateIcon()
        {
            return null;
        }

        private Drawing mDrawing;
        public Drawing Drawing
        {
            get
            {
                return mDrawing;
            }
            set
            {
                //Detach
                if (mDrawing != null)
                {
                    mDrawing.OnAttachToCanvas -= mDrawing_OnAttachToCanvas;
                    mDrawing.OnDetachFromCanvas -= mDrawing_OnDetachFromCanvas;
                    Stopping();
                    Parent = null;
                }
                mDrawing = value;
                //Attach
                if (mDrawing != null)
                {
                    mDrawing.OnAttachToCanvas += mDrawing_OnAttachToCanvas;
                    mDrawing.OnDetachFromCanvas += mDrawing_OnDetachFromCanvas;
                    Started();
                    Parent = mDrawing.Canvas;
                }
            }
        }

        void mDrawing_OnAttachToCanvas(Canvas e)
        {
            Parent = e;
        }

        void mDrawing_OnDetachFromCanvas(Canvas e)
        {
            Parent = null;
        }

        private Canvas mParent;
        protected Canvas Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                if (mParent != null)
                {
                    mParent.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(MouseLeftButtonDown);
                    mParent.MouseMove -= new System.Windows.Input.MouseEventHandler(MouseMove);
                    mParent.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(MouseLeftButtonUp);
                    mParent.KeyDown -= new System.Windows.Input.KeyEventHandler(KeyDown);
                    mParent.KeyUp -= new System.Windows.Input.KeyEventHandler(KeyUp);
                    mParent.MouseRightButtonUp -= new System.Windows.Input.MouseButtonEventHandler(MouseRightButtonUp);
                }
                mParent = value;
                if (mParent != null)
                {
                    mParent.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MouseLeftButtonDown);
                    mParent.MouseMove += new System.Windows.Input.MouseEventHandler(MouseMove);
                    mParent.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MouseLeftButtonUp);
                    mParent.KeyDown += new System.Windows.Input.KeyEventHandler(KeyDown);
                    mParent.KeyUp += new System.Windows.Input.KeyEventHandler(KeyUp);
                    mParent.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(MouseRightButtonUp);
                }
            }
        }

        public virtual void Started()
        {
        }

        public virtual void Stopping()
        {
        }

        public virtual void AbortAndSetDefaultTool()
        {
            Reset();
            Drawing.SetDefaultBehavior();
        }

        protected virtual void Reset()
        {
        }

        // 09-21-2010 Scott
        public virtual void MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Drawing.SetDefaultBehavior();
            if (Drawing != null)
            {
                // 11-12-2010 Scott
                IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();
                ContextMenu contextMenu = null;

                // 03-09-2011 Scott
                Point offsetFromFigureLeftTopCorner = Coordinates(e);
                IFigure found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);
                if (figures != null && figures.Count() > 0 && figures.Contains(found))
                {
                }
                else if(found != null)
                {
                    Drawing.ClearSelectedFigures();
                    found.Selected = true;
                    Drawing.Figures.UpdateVisual();
                }

                if (figures != null && figures.Count() > 0)
                {
                    if (found is Game.PBPlayer)
                    {
                        Game.PBPlayer player = found as Game.PBPlayer;
                        contextMenu = CreatePlayerMenu();
                        contextMenu.PlacementTarget = player.Shape;
                    }
                    else if (found is LabelBase)
                    {
                        LabelBase label = found as LabelBase;
                        contextMenu = CreateLabelMenu();
                        contextMenu.PlacementTarget = label.Shape;
                    }
                    else if (found is LineBase)
                    {
                        LineBase line = found as LineBase;
                        contextMenu = CreateLineMenu();
                        contextMenu.PlacementTarget = line.Shape;
                    }
                    else if (found is Game.PBBall)
                    {
                        Game.PBBall ball = found as Game.PBBall;
                        contextMenu = CreateBallMenu();
                        contextMenu.PlacementTarget = ball.Shape;
                    }
                    else if (found is Game.Zone)
                    {
                        Game.Zone zone = found as Game.Zone;
                        contextMenu = CreateZoneMenu();
                        contextMenu.PlacementTarget = zone.Shape;
                    }
                    else if (found is Game.PBImage)
                    {
                        Game.PBImage pbImg = found as Game.PBImage;
                        contextMenu = CreateImageMenu();
                        contextMenu.PlacementTarget = pbImg.Shape;
                    }

                    if (contextMenu != null)
                    {
                        contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        contextMenu.IsOpen = true;
                    }
                }
                else
                {
                    contextMenu = CreateMainMenu();
                    contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    contextMenu.IsOpen = true;
                }
            }
        }

        public ContextMenu CreateMainMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            MenuItem miOffensivePlayer = new MenuItem()
            {
                Header = "Offensive Players",
            };
            miOffensivePlayer.Click += new RoutedEventHandler(miOffensivePlayer_Click);
            contextMenu.Items.Add(miOffensivePlayer);

            MenuItem miDefensivePlayer = new MenuItem()
            {
                Header = "Defensive Players",
            };
            miDefensivePlayer.Click += new RoutedEventHandler(miDefensivePlayer_Click);
            contextMenu.Items.Add(miDefensivePlayer);

            return contextMenu;
        }

        void miDefensivePlayer_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Figures.OfType<Game.PBPlayer>().ForEach(p => p.Selected = (p.ScoutType != 0));
            Drawing.Figures.UpdateVisual();
        }

        void miOffensivePlayer_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Figures.OfType<Game.PBPlayer>().ForEach(p => p.Selected = (p.ScoutType == 0));
            Drawing.Figures.UpdateVisual();
        }

        public void CreateLineMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            IEnumerable<LineBase> lines = Drawing.GetSelectedFigures().OfType<LineBase>();

            MenuItem miLineColor = new MenuItem()
            {
                Header = "Color...",
            };
            miLineColor.Click += new RoutedEventHandler(miLineColor_Click);
            menu.Items.Add(miLineColor);

            MenuItem miLineType = new MenuItem()
            {
                Header = "Line Type",
            };
            MenuItem miBeeLine = new MenuItem()
            {
                Header = "Bee Line",
            };
            miBeeLine.Click += new RoutedEventHandler(miBeeLine_Click);
            MenuItem miJaggedLine = new MenuItem()
            {
                Header = "Jagged Line",
            };
            miJaggedLine.Click += new RoutedEventHandler(miJaggedLine_Click);
            MenuItem miCurvyLine = new MenuItem()
            {
                Header = "Curvy Line",
            };
            miCurvyLine.Click += new RoutedEventHandler(miCurvyLine_Click);
            miLineType.Items.Add(miBeeLine);
            miLineType.Items.Add(miJaggedLine);
            miLineType.Items.Add(miCurvyLine);
            menu.Items.Add(miLineType);

            MenuItem miDashType = new MenuItem()
            {
                Header = "Dash Type",
            };
            MenuItem miSolidLine = new MenuItem()
            {
                Header = "Solid",
            };
            miSolidLine.Click += new RoutedEventHandler(miSolidLine_Click);
            MenuItem miDashedLine = new MenuItem()
            {
                Header = "Dashed",
            };
            miDashedLine.Click += new RoutedEventHandler(miDashedLine_Click);
            MenuItem miDottedLine = new MenuItem()
            {
                Header = "Dotted",
            };
            miDottedLine.Click += new RoutedEventHandler(miDottedLine_Click);
            miDashType.Items.Add(miSolidLine);
            miDashType.Items.Add(miDashedLine);
            miDashType.Items.Add(miDottedLine);
            menu.Items.Add(miDashType);
            MenuItem miThickness = new MenuItem()
            {
                Header = "Thickness",
            };
            Slider sliderLineThickness = new Slider()
            {
                Value = lines.Count() > 0 ? lines.First().StrokeThickness : 3,
                Minimum = 1,
                Maximum = 5,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.5,
                ToolTip = "Thickness",
                IsTabStop = false,
                Orientation = Orientation.Vertical,
                Width = 25,
                Height = 100,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                AutoToolTipPrecision = 1,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
                IsDirectionReversed = false,
                IsMoveToPointEnabled = false,
            };
            sliderLineThickness.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderLineThickness_ValueChanged);
            miThickness.Items.Add(sliderLineThickness);
            menu.Items.Add(miThickness);
        }

        void sliderLineThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IEnumerable<LineBase> lines = Drawing.GetSelectedFigures().OfType<LineBase>();
            lines.ForEach(f => f.StrokeThickness = e.NewValue);
            lines.ForEach(f => f.UpdateVisual());
        }

        void sliderPlayerBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IEnumerable<Game.PBPlayer> players = Drawing.GetSelectedFigures().OfType<Game.PBPlayer>();
            players.ForEach(f => f.StrokeThickness = e.NewValue);
            players.ForEach(f => f.UpdateVisual());
        }

        public ContextMenu CreateZoneMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreateZoneMenuItems(contextMenu);

            return contextMenu;
        }

        public ContextMenu CreateImageMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreateImageMenuItems(contextMenu);

            return contextMenu;
        }

        public void CreateImageMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() == 1 && figures.First() is Game.PBImage)
            {
                Game.PBImage pbImg = figures.First() as Game.PBImage;

                StackPanel spWidth = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                spWidth.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
                TextBlock tblkWidth = new TextBlock()
                {
                    Text = "Width:  ",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                tblkWidth.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
                tblkWidth.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
                TextBox tboxWidth = new TextBox()
                {
                    Width = 50,
                    Text = ToPhysical(pbImg.Width).ToString(),
                };
                tboxWidth.TextChanged += new TextChangedEventHandler(tboxWidth_TextChanged);
                tboxWidth.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
                tboxWidth.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
                spWidth.Children.Add(tblkWidth);
                spWidth.Children.Add(tboxWidth);
                menu.Items.Add(spWidth);

                StackPanel spHeight = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                spHeight.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
                TextBlock tblkHeight = new TextBlock()
                {
                    Text = "Height: ",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                tblkHeight.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
                tblkHeight.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
                TextBox tboxHeight = new TextBox()
                {
                    Width = 50,
                    Text = ToPhysical(pbImg.Height).ToString(),
                };
                tboxHeight.TextChanged += new TextChangedEventHandler(tboxHeight_TextChanged);
                tboxHeight.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
                tboxHeight.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
                spHeight.Children.Add(tblkHeight);
                spHeight.Children.Add(tboxHeight);
                menu.Items.Add(spHeight);
            }
        }

        public void CreateZoneMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() == 1 && figures.First() is Game.Zone)
            {
                Game.Zone zone = figures.First() as Game.Zone;

                StackPanel spWidth = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                spWidth.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
                TextBlock tblkWidth = new TextBlock()
                {
                    Text = "Width:  ",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                tblkWidth.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
                tblkWidth.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
                TextBox tboxWidth = new TextBox()
                {
                    Width = 50,
                    Text = zone.Width.ToString(),
                };
                tboxWidth.TextChanged += new TextChangedEventHandler(tboxWidth_TextChanged);
                tboxWidth.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
                tboxWidth.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
                spWidth.Children.Add(tblkWidth);
                spWidth.Children.Add(tboxWidth);
                menu.Items.Add(spWidth);

                StackPanel spHeight = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                spHeight.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
                TextBlock tblkHeight = new TextBlock()
                {
                    Text = "Height: ",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                tblkHeight.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
                tblkHeight.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
                TextBox tboxHeight = new TextBox()
                {
                    Width = 50,
                    Text = zone.Height.ToString(),
                };
                tboxHeight.TextChanged += new TextChangedEventHandler(tboxHeight_TextChanged);
                tboxHeight.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
                tboxHeight.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
                spHeight.Children.Add(tblkHeight);
                spHeight.Children.Add(tboxHeight);
                menu.Items.Add(spHeight);

                MenuItem miTransparency = new MenuItem()
                {
                    Header = "Transparency",
                };
                Slider sliderTransparency = new Slider()
                {
                    Value = zone.Transparency,
                    Minimum = 0.1,
                    Maximum = 1,
                    IsSnapToTickEnabled = true,
                    TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                    TickFrequency = 0.1,
                    ToolTip = "Border Thickness",
                    IsTabStop = false,
                    Orientation = Orientation.Vertical,
                    Width = 25,
                    Height = 100,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    AutoToolTipPrecision = 1,
                    AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
                    IsDirectionReversed = false,
                    IsMoveToPointEnabled = false,
                };
                sliderTransparency.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderTransparency_ValueChanged);
                miTransparency.Items.Add(sliderTransparency);
                menu.Items.Add(miTransparency);

                MenuItem miZoneFillColor = new MenuItem()
                {
                    Header = "Fill Color...",
                };
                miZoneFillColor.Click += new RoutedEventHandler(miZoneFillColor_Click);
                menu.Items.Add(miZoneFillColor);
            }
        }

        void miZoneFillColor_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() == 1 && figures.First() is Game.Zone)
            {
                Game.Zone zone = figures.First() as Game.Zone;

                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = zone.FillColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    zone.FillColor = cpd.SelectedColor;

                    zone.UpdateVisual();
                }
            }
        }

        void sliderTransparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() == 1 && figures.First() is Game.Zone)
            {
                Game.Zone zone = figures.First() as Game.Zone;

                zone.Transparency = e.NewValue;

                zone.UpdateVisual();
            }
        }

        void tboxHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();
            TextBox tb = sender as TextBox;

            if (tb != null && figures != null && figures.Count() == 1)
            {
                try
                {
                    double height = Convert.ToDouble(tb.Text);

                    if (figures.First() is Game.Zone)
                    {
                        if (height > 2 && height < 80)
                        {
                            Game.Zone zone = figures.First() as Game.Zone;

                            zone.Height = height;

                            zone.UpdateVisual();
                        }
                    }
                    else if (figures.First() is Game.PBImage)
                    {
                        if (height > 2 && height < 1000)
                        {
                            Game.PBImage pbImg = figures.First() as Game.PBImage;

                            pbImg.Height = ToLogical(height);

                            pbImg.UpdateVisual();
                        }
                    }
                }
                catch
                {

                }
            }
        }

        void tboxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();
            TextBox tb = sender as TextBox;

            if (tb != null && figures != null && figures.Count() == 1)
            {
                try
                {
                    double width = Convert.ToDouble(tb.Text);

                    if (figures.First() is Game.Zone)
                    {
                        if (width > 2 && width < 80)
                        {
                            Game.Zone zone = figures.First() as Game.Zone;

                            zone.Width = width;

                            zone.UpdateVisual();
                        }
                    }
                    else if (figures.First() is Game.PBImage)
                    {
                        if (width > 2 && width < 1000)
                        {
                            Game.PBImage pbImg = figures.First() as Game.PBImage;

                            pbImg.Width = ToLogical(width);

                            pbImg.UpdateVisual();
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public ContextMenu CreateBallMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreateBallMenuItems(contextMenu);

            return contextMenu;
        }

        public void CreateBallMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            MenuItem miBallSize = new MenuItem()
            {
                Header = "Size",
            };
            menu.Items.Add(miBallSize);

            MenuItem miSmallBall = new MenuItem()
            {
                Header = "Small",
            };
            miSmallBall.Click += new RoutedEventHandler(miSmallBall_Click);
            MenuItem miMediumBall = new MenuItem()
            {
                Header = "Medium",
            };
            miMediumBall.Click += new RoutedEventHandler(miMediumBall_Click);
            MenuItem miLargeBall = new MenuItem()
            {
                Header = "Large",
            };
            miLargeBall.Click += new RoutedEventHandler(miLargeBall_Click);
            miBallSize.Items.Add(miSmallBall);
            miBallSize.Items.Add(miMediumBall);
            miBallSize.Items.Add(miLargeBall);
        }

        void miLargeBall_Click(object sender, RoutedEventArgs e)
        {
            Webb.Playbook.Data.GameSetting.Instance.BallSize = Webb.Playbook.Data.BallSize.Large;

            Drawing.Figures.UpdateVisual();
        }

        void miMediumBall_Click(object sender, RoutedEventArgs e)
        {
            Webb.Playbook.Data.GameSetting.Instance.BallSize = Webb.Playbook.Data.BallSize.Medium;

            Drawing.Figures.UpdateVisual();
        }

        void miSmallBall_Click(object sender, RoutedEventArgs e)
        {
            Webb.Playbook.Data.GameSetting.Instance.BallSize = Webb.Playbook.Data.BallSize.Small;

            Drawing.Figures.UpdateVisual();
        }

        public ContextMenu CreateLineMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreateLineMenuItems(contextMenu);

            return contextMenu;
        }

        void miDottedLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.DashType = DashType.Dotted);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miDashedLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.DashType = DashType.Dashed);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miSolidLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.DashType = DashType.Solid);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miCurvyLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.LineType = LineType.CurvyLine);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miJaggedLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.LineType = LineType.JaggedLine);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miBeeLine_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is PBLine) && figures.Count() > 0)
            {
                figures.OfType<PBLine>().ForEach(f => f.LineType = LineType.BeeLine);

                figures.OfType<PBLine>().ForEach(f => f.UpdateVisual());
            }
        }

        void miLineColor_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is LineBase) && figures.Count() > 0)
            {
                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = (figures.OfType<LineBase>().Last() as LineBase).StrokeColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    figures.OfType<LineBase>().ForEach(f => f.StrokeColor = cpd.SelectedColor);

                    figures.OfType<LineBase>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        public void CreateLabelMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            double angle = 0;

            PBLabel selLabel = null;

            if (figures != null && figures.Any(f => f is PBLabel) && figures.Count() > 0)
            {
                selLabel = figures.OfType<PBLabel>().Last();

                angle = selLabel.Angle;
            }

            MenuItem miTextColor = new MenuItem()
            {
                Header = "Text Color...",
            };
            miTextColor.Click += new RoutedEventHandler(miTextColor_Click);
            menu.Items.Add(miTextColor);

            MenuItem miTextFillColor = new MenuItem()
            {
                Header = "Fill Color...",
            };
            miTextFillColor.Click += new RoutedEventHandler(miTextFillColor_Click);
            menu.Items.Add(miTextFillColor);

            MenuItem miFont = new MenuItem()
            {
                Header = "Font...",
            };
            miFont.Click += new RoutedEventHandler(miFont_Click);
            menu.Items.Add(miFont);

            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            sp.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
            TextBlock tblk = new TextBlock()
            {
                Text = "Angle: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            tblk.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
            tblk.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
            TextBox tbox = new TextBox()
            {
                Width = 50,
                Text = angle.ToString(),
            };
            tbox.KeyDown += new System.Windows.Input.KeyEventHandler(tboxLabelAngle_KeyDown);
            tbox.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
            tbox.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
            sp.Children.Add(tblk);
            sp.Children.Add(tbox);
            menu.Items.Add(sp);

            MenuItem miThickness = new MenuItem()
            {
                Header = "Thickness",
            };
            Slider sliderborderThickness = new Slider()
            {
                Value = selLabel != null ? selLabel.BorderWidth : 0,
                Minimum = 0,
                Maximum = 5,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.5,
                ToolTip = "Border Thickness",
                IsTabStop = false,
                Orientation = Orientation.Vertical,
                Width = 25,
                Height = 100,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                AutoToolTipPrecision = 1,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
                IsDirectionReversed = false,
                IsMoveToPointEnabled = false,
            };
            sliderborderThickness.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderborderThickness_ValueChanged);
            miThickness.Items.Add(sliderborderThickness);
            menu.Items.Add(miThickness);

            MenuItem miBorderColor = new MenuItem()
            {
                Header = "Border Color...",
            };
            miBorderColor.Click += new RoutedEventHandler(miBorderColor_Click);
            menu.Items.Add(miBorderColor);
        }

        void miBorderColor_Click(object sender, RoutedEventArgs e)
        {
             IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            PBLabel selLabel = null;

            if (figures != null && figures.Any(f => f is PBLabel) && figures.Count() > 0)
            {
                selLabel = figures.OfType<PBLabel>().Last();

                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = selLabel.BorderColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    figures.OfType<PBLabel>().ForEach(f => f.BorderColor = cpd.SelectedColor);

                    figures.OfType<PBLabel>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        void sliderborderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Count() > 0 && figures.Any(f => f is PBLabel))
            {
                figures.OfType<PBLabel>().ForEach(f => f.BorderWidth = e.NewValue);

                figures.OfType<PBLabel>().ForEach(f => f.UpdateVisual());
            }
        }

        public ContextMenu CreateLabelMenu()   // 10-20-2010 Scott
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreateLabelMenuItems(contextMenu);

            return contextMenu;
        }

        void miTextFillColor_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is LabelBase) && figures.Count() > 0)
            {
                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = figures.OfType<LabelBase>().Last().FillColor == Colors.Transparent ? Colors.Black : figures.OfType<LabelBase>().Last().FillColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    figures.OfType<LabelBase>().ForEach(f => f.FillColor = cpd.SelectedColor);

                    figures.OfType<LabelBase>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        void tboxText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;

                IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

                if (figures != null && figures.Any(f => f is Game.PBPlayer) && figures.Count() > 0)
                {
                    figures.OfType<Game.PBPlayer>().ForEach(f => f.Text = tb.Text);

                    Drawing.Figures.UpdateVisual();
                }
            }
        }

        void tboxLabelAngle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox tb = sender as TextBox;

                if (tb.Text.ToCharArray().All(c => char.IsDigit(c)))
                {
                    double angle = tb.Text.ToDouble(0);

                    IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

                    if (figures != null && figures.Any(f => f is PBLabel) && figures.Count() > 0)
                    {
                        figures.OfType<PBLabel>().ForEach(f => f.Angle = angle);

                        Drawing.Figures.UpdateVisual();
                    }
                }
            }
        }

        void tboxPlayerAngle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox tb = sender as TextBox;

                if (tb.Text.ToCharArray().All(c => char.IsDigit(c)))
                {
                    double angle = tb.Text.ToDouble(0);

                    IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

                    if (figures != null && figures.Any(f => f is Game.PBPlayer) && figures.Count() > 0)
                    {
                        figures.OfType<Game.PBPlayer>().ForEach(f => f.Angle = angle);

                        Drawing.Figures.UpdateVisual();
                    }
                }
            }
        }

        void miFont_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is LabelBase) && figures.Count() > 0)
            {
                LabelBase selLabel = figures.OfType<LabelBase>().Last();

                TextBlock tb = selLabel.Shape;

                FontChooser fontChooser = new FontChooser();
                fontChooser.Owner = Behavior.Owner;

                selLabel.Shape.FontSize = selLabel.FromFontSize(selLabel.Shape.FontSize);
                fontChooser.SetPropertiesFromObject(tb);
                selLabel.Shape.FontSize = selLabel.ToFontSize(selLabel.Shape.FontSize);

                fontChooser.PreviewSampleText = tb.Text;

                if (fontChooser.ShowDialog().Value)
                {
                    figures.OfType<LabelBase>().ForEach(f => fontChooser.ApplyPropertiesToObject(f.Shape));

                    figures.OfType<LabelBase>().ForEach(f => f.FontFamily = f.Shape.FontFamily.Source);

                    figures.OfType<LabelBase>().ForEach(f => f.FontSize = f.Shape.FontSize);

                    figures.OfType<LabelBase>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        void miTextColor_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is LabelBase) && figures.Count() > 0)
            {
                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = figures.OfType<LabelBase>().Last().TextColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    figures.OfType<LabelBase>().ForEach(f => f.TextColor = cpd.SelectedColor);

                    figures.OfType<LabelBase>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        public void CreatePlayerMenuItems(ItemsControl menu)
        {
            menu.Items.Clear();

            Game.PBPlayer selectedPlayer = null;

            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            double angle = 0;

            if (figures != null && figures.Any(f => f is Game.PBPlayer) && figures.Count() > 0)
            {
                selectedPlayer = figures.OfType<Game.PBPlayer>().Last();

                angle = selectedPlayer.Angle;
            }

            Slider sliderSize = new Slider()
            {
                Value = selectedPlayer != null ? selectedPlayer.Radius : 1,
                Minimum = 1.0,
                Maximum = 2.0,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.25,
                ToolTip = "Size",
                IsTabStop = false,
            };
            sliderSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderSize_ValueChanged);
            menu.Items.Add(sliderSize);

            MenuItem miTextVisible = new MenuItem()
            {
                Header = "Hide letters",
            };
            if (selectedPlayer != null)
            {
                miTextVisible.Header = selectedPlayer.TextVisible ? "Hide letters" : "Show letters";
            }
            miTextVisible.Click += new RoutedEventHandler(miTextVisible_Click);
            menu.Items.Add(miTextVisible);

            MenuItem miDash = new MenuItem()
            {
                Header = "Dash",
            };
            if (selectedPlayer != null)
            {
                miDash.Header = selectedPlayer.Dash ? "Solid" : "Dash";
            }
            miDash.Click += new RoutedEventHandler(miDash_Click);
            menu.Items.Add(miDash);

            MenuItem miPlayerTextColor = new MenuItem()
            {
                Header = "Text Color...",
            };
            miPlayerTextColor.Click += new RoutedEventHandler(miPlayerTextColor_Click);
            menu.Items.Add(miPlayerTextColor);

            MenuItem miFillColor = new MenuItem()
            {
                Header = "Fill Color...",
            };
            miFillColor.Click += new RoutedEventHandler(miFillColor_Click);
            menu.Items.Add(miFillColor);

            MenuItem miSymbols = new MenuItem()
            {
                Header = "Symbols",
            };
            menu.Items.Add(miSymbols);

            MenuItem miCircle = new MenuItem()
            {
                Header = "Circle",
            };
            miCircle.Click += new RoutedEventHandler(miCircle_Click);
            miSymbols.Items.Add(miCircle);

            MenuItem miSquare = new MenuItem()
            {
                Header = "Square",
            };
            miSquare.Click += new RoutedEventHandler(miSquare_Click);
            miSymbols.Items.Add(miSquare);

            MenuItem miTriangle = new MenuItem()
            {
                Header = "Triangle",
            };
            miTriangle.Click += new RoutedEventHandler(miTriangle_Click);
            miSymbols.Items.Add(miTriangle);

            MenuItem miText = new MenuItem()
            {
                Header = "Letters",
            };
            miText.Click += new RoutedEventHandler(miText_Click);
            miSymbols.Items.Add(miText);

            MenuItem miRemove = new MenuItem()
            {
                Header = "Remove",
            };
            miRemove.Click += new RoutedEventHandler(miRemove_Click);
            menu.Items.Add(miRemove);

            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            sp.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
            TextBlock tblk = new TextBlock()
            {
                Text = "Angle: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            tblk.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
            tblk.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
            TextBox tbox = new TextBox()
            {
                Width = 50,
                Text = angle.ToString(),
            };
            tbox.KeyDown += new System.Windows.Input.KeyEventHandler(tboxPlayerAngle_KeyDown);
            tbox.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
            tbox.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
            sp.Children.Add(tblk);
            sp.Children.Add(tbox);
            menu.Items.Add(sp);

            MenuItem miThickness = new MenuItem()
            {
                Header = "Thickness",
            };
            Slider sliderLineThickness = new Slider()
            {
                Value = selectedPlayer != null ? selectedPlayer.StrokeThickness : 1,
                Minimum = 1,
                Maximum = 5,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft,
                TickFrequency = 0.5,
                ToolTip = "Thickness",
                IsTabStop = false,
                Orientation = Orientation.Vertical,
                Width = 25,
                Height = 100,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                AutoToolTipPrecision = 1,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight,
                IsDirectionReversed = false,
                IsMoveToPointEnabled = false,
            };
            sliderLineThickness.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderPlayerBorderThickness_ValueChanged);
            miThickness.Items.Add(sliderLineThickness);
            menu.Items.Add(miThickness);

            StackPanel spText = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            spText.SetResourceReference(StackPanel.BackgroundProperty, "ControllBackgroundBrush");
            TextBlock tblkText = new TextBlock()
            {
                Text = "Text:  ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            tblkText.SetResourceReference(TextBlock.BackgroundProperty, "ControllBackgroundBrush");
            tblkText.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
            TextBox tboxText = new TextBox()
            {
                Width = 50,
                Text = selectedPlayer != null ? selectedPlayer.Text : string.Empty,
            };
            tboxText.KeyDown += new KeyEventHandler(tboxText_KeyDown);
            tboxText.SetResourceReference(TextBox.BackgroundProperty, "NormalBackgroundBrush");
            tboxText.SetResourceReference(TextBox.ForegroundProperty, "OutsideTextBrush");
            spText.Children.Add(tblkText);
            spText.Children.Add(tboxText);
            menu.Items.Add(spText);
        }

        public ContextMenu CreatePlayerMenu()   // 10-20-2010 Scott
        {
            System.Windows.Controls.ContextMenu contextMenu = new ContextMenu();

            CreatePlayerMenuItems(contextMenu);

            return contextMenu;
        }

        void miEditText_Click(object sender, RoutedEventArgs e)
        {
            
        }

        void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Drawing.GetSelectedPlayers().ForEach(p => p.Radius = e.NewValue);

            Drawing.GetSelectedPlayers().ForEach(p => p.UpdateVisual());
        }

        void miDash_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            switch (mi.Header.ToString())
            {
                case "Dash":
                    Drawing.GetSelectedPlayers().ForEach(p => p.Dash = true);
                    break;
                case "Solid":
                    Drawing.GetSelectedPlayers().ForEach(p => p.Dash = false);
                    break;
            }

            Drawing.GetSelectedPlayers().ForEach(p => p.UpdateVisual());
        }

        void miTextVisible_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            switch (mi.Header.ToString())
            {
                case "Show letters":
                    Drawing.GetSelectedPlayers().ForEach(p => p.TextVisible = true);
                    break;
                case "Hide letters":
                    Drawing.GetSelectedPlayers().ForEach(p => p.TextVisible = false);
                    break;
            }

            Drawing.GetSelectedPlayers().ForEach(p => p.UpdateVisual());
        }

        void miPlayerTextColor_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IFigure> figures = Drawing.GetSelectedFigures();

            if (figures != null && figures.Any(f => f is Game.PBPlayer) && figures.Count() > 0)
            {
                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = figures.OfType<Game.PBPlayer>().Last().TextColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    figures.OfType<Game.PBPlayer>().ForEach(f => f.TextColor = cpd.SelectedColor);

                    figures.OfType<Game.PBPlayer>().ForEach(f => f.UpdateVisual());
                }
            }
        }

        void miText_Click(object sender, RoutedEventArgs e)
        {
            Drawing.GetSelectedPlayers().ForEach(p => p.ChangeAppearance(Webb.Playbook.Geometry.Game.PlayerAppearance.Text));
        }

        void miTriangle_Click(object sender, RoutedEventArgs e)
        {
            Drawing.GetSelectedPlayers().ForEach(p => p.ChangeAppearance(Webb.Playbook.Geometry.Game.PlayerAppearance.Triangle));
        }

        void miSquare_Click(object sender, RoutedEventArgs e)
        {
            Drawing.GetSelectedPlayers().ForEach(p => p.ChangeAppearance(Webb.Playbook.Geometry.Game.PlayerAppearance.Square));
        }

        void miCircle_Click(object sender, RoutedEventArgs e)
        {
            Drawing.GetSelectedPlayers().ForEach(p => p.ChangeAppearance(Webb.Playbook.Geometry.Game.PlayerAppearance.Circle));
        }

        void miFillColor_Click(object sender, RoutedEventArgs e)
        {
            Game.PBPlayer player = Drawing.GetSelectedPlayer();
            if (player != null)
            {
                Microsoft.Samples.CustomControls.ColorPickerDialog cpd = new Microsoft.Samples.CustomControls.ColorPickerDialog()
                {
                    Owner = Behavior.Owner,

                    StartingColor = player.FillColor,
                };

                if (cpd.ShowDialog().Value)
                {
                    Drawing.GetSelectedPlayers().ForEach(p => p.FillColor = cpd.SelectedColor);

                    Drawing.GetSelectedPlayers().ForEach(p => p.UpdateVisual());
                }
            }
        }

        void miRemove_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Delete();
        }

        public virtual void KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
        }

        public virtual void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (Drawing != null)
                    {
                        Drawing.Delete();
                    }
                    break;
            }
        }

        protected virtual void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Behavior.DrawVideo)
            {
                return;
            }

            if ((this is Dragger || this is Selector) && (Parent != null && Parent.Children.Contains(SelectRect)))
            {
                Point ptStart = new Point(Canvas.GetLeft(SelectRect), Canvas.GetTop(SelectRect));

                Point ptEnd = new Point(ptStart.X + SelectRect.Width, ptStart.Y + SelectRect.Height);

                PointPair pp = Drawing.CoordinateSystem.ToLogical(new PointPair(ptStart, ptEnd));

                ReadOnlyCollection<IFigure> selectedFigures = Drawing.Figures.HitTestMany(new Rect(pp.P1, pp.P2));

                if (selectedFigures != null && selectedFigures.Count > 0)
                {
                    if (!IsCtrlPressed())
                    {
                        Drawing.ClearSelectedFigures();
                    }

                    selectedFigures.ForEach(f => f.Selected = true);
                }

                Parent.Children.Remove(SelectRect);
            }
        }

        public virtual IntPtr WndProc(UIElement ui, UIElement uiRelatedTo, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            return IntPtr.Zero;
        }

        protected virtual void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Behavior.DrawVideo)
            {
                return;
            }

            if ((this is Dragger || this is Selector) && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Point phyPoint = System.Windows.Input.Mouse.GetPosition(Parent);

                double width = System.Math.Max(0, phyPoint.X - Canvas.GetLeft(SelectRect));
                double height = System.Math.Max(0, phyPoint.Y - Canvas.GetTop(SelectRect));

                SelectRect.Width = width;
                SelectRect.Height = height;
            }
        }

        protected virtual void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point offsetFromFigureLeftTopCorner = Coordinates(e);

            if (e.ClickCount > 1)
            {
                PBLabel label = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner, typeof(PBLabel)) as PBLabel;

                if (label != null)
                {
                    Webb.Playbook.Geometry.Text textBehavior = Webb.Playbook.Geometry.Behavior.LookupByName("Text") as Webb.Playbook.Geometry.Text;

                    Drawing.Behavior = textBehavior;

                    textBehavior.Label = label;

                    textBehavior.StartEdit(new Point(Canvas.GetLeft(label.Shape), Canvas.GetTop(label.Shape)));

                    e.Handled = true;
                }

                if (!Behavior.DrawVideo)
                {
                    if (Drawing != null)
                    {
                        IEnumerable<IFigure> figure = Drawing.GetSelectedFigures();

                        if (figure.Count() == 1 && figure.First() is LineBase)
                        {
                            LineBase line = figure.First() as LineBase;

                            // continue
                            IEnumerable<IFigure> pathFigures = line.GetPathFigures();

                            if (pathFigures != null)
                            {
                                pathFigures.ForEach(f => f.Selected = true);
                            }
                        }
                    }
                }
            }

            if (Behavior.DrawVideo)
            {
                return;
            }

            if ((this is Dragger || this is Selector) && Drawing != null)
            {
                IFigure selectedFigure = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);
                if (selectedFigure == null)
                {
                    if (Parent != null && !Parent.Children.Contains(SelectRect))
                    {
                        if (SelectRect.Parent != null)  // 12-17-2010
                        {
                            (SelectRect.Parent as Canvas).Children.Remove(SelectRect);
                        }
                        Parent.Children.Add(SelectRect);
                    }
                    Point phyPoint = e.GetPosition(Parent);
                    Canvas.SetLeft(SelectRect, phyPoint.X);
                    Canvas.SetTop(SelectRect, phyPoint.Y);
                    SelectRect.Width = 0;
                    SelectRect.Height = 0;
                    Panel.SetZIndex(SelectRect, 100000);
                }
            }
        }

        private static System.Windows.Shapes.Rectangle SelectRect = new System.Windows.Shapes.Rectangle() { Stroke = Brushes.Blue, Fill = Brushes.AliceBlue, Opacity = 0.3 };

        protected virtual void AddFigure(
            IFigure figureToAdd,
            bool recordAction)
        {
            if (recordAction)
            {
                Drawing.Add(figureToAdd);
            }
            else
            {
                Drawing.Figures.Add(figureToAdd);
            }
        }

        protected virtual System.Windows.Point Coordinates(System.Windows.Input.MouseEventArgs e)
        {
            return ToLogical(e.GetPosition(Parent));
        }

        #region Coordinates
        protected double CursorTolerance
        {
            get
            {
                return Drawing.CoordinateSystem.CursorTolerance;
            }
        }

        protected double ToLogical(double physicalLength)
        {
            return Drawing.CoordinateSystem.ToLogical(physicalLength);
        }

        protected Point ToLogical(Point physicalPoint)
        {
            return Drawing.CoordinateSystem.ToLogical(physicalPoint);
        }

        protected double ToPhysical(double logicalLength)
        {
            return Drawing.CoordinateSystem.ToPhysical(logicalLength);
        }

        protected Point ToPhysical(Point logicalPoint)
        {
            return Drawing.CoordinateSystem.ToPhysical(logicalPoint);
        }
        #endregion

        protected virtual PBSquare CreateSquareAtCurrentPosition(System.Windows.Point coordinates, bool recordAction)
        {
            var result = Factory.CreateSquareShape(Drawing, coordinates);

            AddFigure(result, recordAction);

            return result;
        }

        protected virtual Webb.Playbook.Geometry.Game.PBPlayer CreatePlayerAtCurrentPosition(System.Windows.Point coordinates, bool recordAction)
        {
            var result = Factory.CreatePlayerShape(Drawing, coordinates);

            AddFigure(result, recordAction);

            return result;
        }

        protected virtual PBCircle CreateCircleAtCurrentPosition(System.Windows.Point coordinates, bool recordAction)
        {
            var result = Factory.CreateCircleShape(Drawing, coordinates);

            AddFigure(result, recordAction);

            return result;
        }

        protected virtual FreePoint CreatePointAtCurrentPosition(
            System.Windows.Point coordinates,
            bool recordAction)
        {
            var result = Factory.CreateFreePoint(Drawing, coordinates);
            AddFigure(result, recordAction);
            return result;
        }

        private static bool IsCtrlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }
    }
}