using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Webb.Playbook.Geometry.Shapes;
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Actions;
using Webb.Playbook.Geometry.Shapes.Creators;


namespace Webb.Playbook.Geometry
{
    public class Drawing : IDisposable
    {
        public bool DrawingMode
        {
            get
            {
                if (Behavior is Pen)
                {
                    return true;
                }
                return false;
            }
        }

        private bool title = false;
        public bool Title
        {
            get { return title; }
            set { title = value; }
        }

        private string backgroundPath = string.Empty;
        public string BackgroundPath
        {
            get { return backgroundPath; }
            set { backgroundPath = value; }
        }

        // 01-20-2012 Scott
        private double yardLine = 0;
        public double YardLine
        {
            get { return yardLine; }
            set { yardLine = value; }
        }

        public Webb.Playbook.Data.ScoutTypes ScoutType
        {
            get
            {
                if (Figures.OfType<Game.PBPlayer>().All(p => p.ScoutType == 0))
                {
                    return Webb.Playbook.Data.ScoutTypes.Offensive;
                }

                if (Figures.OfType<Game.PBPlayer>().All(p => p.ScoutType == 1))
                {
                    return Webb.Playbook.Data.ScoutTypes.Defensive;
                }

                return Webb.Playbook.Data.ScoutTypes.Offensive;
            }
        }

        private bool placeHolder = false;
        public bool PlaceHolder
        {
            get { return placeHolder; }
            set { placeHolder = value; }
        }

        private string filePath = string.Empty;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        private bool modified = false;
        public bool Modified
        {
            get
            {
                modified = ActionManager.CanUndo && (ActionManager.History.CurrentState.PreviousAction != CurrentAction);

                return modified;
            }
            set
            {
                modified = value;
            }
        }

        private IAction currentAction;
        public IAction CurrentAction
        {
            get { return currentAction; }
            set { currentAction = value; }
        }

        public void Dispose()
        {
            if (Canvas != null)
            {
                Drawing_OnDetachFromCanvas(Canvas);

                Playground.Dispose();

                Canvas = null;
            }
        }

        private Point pointdef;
        public Point PointDef
        {
            get { return pointdef; }
            set { pointdef = value; }
        }
        private Point pointoff;
        public Point PointOff
        {
            get { return pointoff; }
            set { pointoff = value; }
        }
        private CoordinateSystem coordinateSystem;
        public CoordinateSystem CoordinateSystem
        {
            get { return coordinateSystem; }
            set { coordinateSystem = value; }
        }

        public FigureList Figures { get; set; }

        private Webb.Playbook.Geometry.Conrdinates.Playground playground;
        public Webb.Playbook.Geometry.Conrdinates.Playground Playground
        {
            get { return playground; }
            set { playground = value; }
        }

        public void SetPlaygroundType(Webb.Playbook.Data.PlaygroundTypes pt)
        {
            Playground.SetPlaygroundType(pt);
        }

        public void SetPlaygroundColor(Webb.Playbook.Data.PlaygroundColors pc)
        {
            Playground.SetPlaygroundColor(pc);
        }

        public void SetFiguresColor(bool bEnableColor)
        {
            this.Figures.ForEach(f =>
            {
                if (f is FigureBase)
                {
                    (f as FigureBase).EnableColor = bEnableColor;
                }
            });
            this.Figures.UpdateVisual();
        }

        public void Clear()
        {
            Remove(Figures as IEnumerable<IFigure>);
        }

        public void SetTitleBackground(Brush brush)
        {
            Playground.UCPlayground.SetTitleBackground(brush);
        }

        public void HideTitleBorder()
        {
            Playground.UCPlayground.borderBackground.Visibility = Visibility.Hidden;
        }

        public void SetPlaygroundVisible(Visibility visibility)
        {
            Playground.UCPlayground.Visibility = visibility;
        }

        public void UpdatePlayground()
        {
            if (Playground != null)
            {
                Playground.UpdatePlayground();
            }
        }

        private IList<IFigure> figures;

        private Canvas mCanvas;

        public Canvas Canvas
        {
            get { return mCanvas; }
            set
            {
                if (mCanvas != null && OnDetachFromCanvas != null)
                {
                    OnDetachFromCanvas(mCanvas);
                }
                mCanvas = value;
                if (mCanvas != null && OnAttachToCanvas != null)
                {
                    OnAttachToCanvas(mCanvas);
                }
            }
        }

        private Behavior mBehavior;
        public Behavior Behavior
        {
            get
            {
                return mBehavior;
            }
            set
            {
                Behavior oldBehavior = mBehavior;

                if (mBehavior == value)
                {
                    return;
                }
                if (mBehavior != null)
                {
                    Behavior.Drawing = null;
                }
                mBehavior = value;
                if (mBehavior != null)
                {
                    mBehavior.Drawing = this;
                }
                if (BehaviorChanged != null)
                {
                    BehaviorChanged(oldBehavior, value);
                }
            }
        }

        public GridLines GridLines;

        public ActionManager ActionManager { get; private set; }

        public event Action<Canvas> OnAttachToCanvas;
        public event Action<Canvas> OnDetachFromCanvas;

        public event Action<Behavior, Behavior> BehaviorChanged;

        public Drawing(Canvas newCanvas)
        {
            ActionManager = new ActionManager(this);
            this.SetDefaultBehavior();

            Figures = new RootFigureList() { Drawing = this };

            // create playground
            Playground = new Webb.Playbook.Geometry.Conrdinates.Playground(this);

            OnAttachToCanvas += Drawing_OnAttachToCanvas;
            OnDetachFromCanvas += Drawing_OnDetachFromCanvas;
            Canvas = newCanvas;

            // set coordinate system
            Point ptOrigin = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
            double lUnit = Canvas.ActualWidth / CoordinateSystem.LogicalPlaygroundWidth;
            coordinateSystem = new CoordinateSystem(ptOrigin, lUnit);

            // create grid
            GridLines = new GridLines(this);
        }

        public double GetScrimmageLine()
        {
            double scrimmage = 0;

            if (Figures.Any(f => f is Game.PBBall))
            {
                scrimmage = Figures.OfType<Game.PBBall>().First().Coordinates.Y;
            }

            return scrimmage;
        }

        public void CreateDrawingImage()
        {

        }

        public bool HasGridLines
        {
            get
            {
                return GridLines.HasGridLines;
            }
        }

        public void ShowCenterLine()
        {
            Playground.UCPlayground.ShowCenterLine();
        }

        public void ShowGridLines()
        {
            GridLines.Show();
        }

        public void HideGridLines()
        {
            GridLines.Hide();
        }

        void Drawing_OnAttachToCanvas(Canvas e)
        {
            e.SizeChanged += mCanvas_SizeChanged;
            Playground.OnAddingToCanvas(e); // 09-16-2010 Scott
            Figures.OnAddingToCanvas(e);
        }

        void Drawing_OnDetachFromCanvas(Canvas e)
        {
            e.SizeChanged -= mCanvas_SizeChanged;
            Playground.OnRemovingFromCanvas(e); // 09-16-2010 Scott
            Figures.OnRemovingFromCanvas(e);
        }

        void mCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            RaiseSizeChanged(e);
            //Recalculate();
        }

        public event SizeChangedEventHandler SizeChanged;
        internal void RaiseSizeChanged(SizeChangedEventArgs args)
        {
            if (SizeChanged != null)
            {
                SizeChanged(this, args);
            }
        }

        public void InitialEventsHandle()
        {
            Canvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
            Canvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
            Canvas.MouseMove += new System.Windows.Input.MouseEventHandler(Canvas_MouseMove);
        }

        public void SetDefaultBehavior()
        {
            Behavior = Webb.Playbook.Geometry.Behavior.LookupByName("Dragger");
        }

        void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        void Canvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        public bool CheckPlayer(string name)
        {
            if (this.Figures.OfType<Game.PBPlayer>().Any(p => p.Name == name))
            {
                return false;
            }

            return true;
        }

        public void Add(IFigure figure)
        {
            AddFigureAction action = new AddFigureAction(this, figure);

            ActionManager.RecordAction(action);

            if (PlaceHolder)
            {
                figure.PlaceHolder = PlaceHolder;
            }
        }

        public void Add(IFigure figure, bool record)
        {
            if (record)
            {
                AddFigureAction action = new AddFigureAction(this, figure);
                ActionManager.RecordAction(action);
            }
            else
            {
                figures.Add(figure);
            }
        }

        public void AddWithoutRecord(IFigure figure)
        {
            Figures.Add(figure);

            if (PlaceHolder)
            {
                figure.PlaceHolder = PlaceHolder;
            }
        }

        public void Remove(IFigure figure)
        {
            RemoveFigureAction action = new RemoveFigureAction(this, figure);
            ActionManager.RecordAction(action);
        }

        //public void RemoveAll()
        //{
        //    this.Figures.ForEach(f => Remove(f));
        //}

        public void Recalculate()
        {
            Figures.Recalculate();
        }

        public void Add(System.Collections.Generic.IEnumerable<IFigure> figures)
        {
            using (Transaction.Create(ActionManager, true))
            {
                figures.ForEach(Add);
            }
        }
        public void Add(FigureList figures)
        {
            using (Transaction.Create(ActionManager, true))
            {
                figures.ForEach(Add);
            }
        }
        public void Add(System.Collections.Generic.IEnumerable<IFigure> figures, bool record)
        {
            if (record)
            {
                using (Transaction.Create(ActionManager, true))
                {
                    figures.ForEach(Add);
                }
            }
            else
            {
                figures.ForEach(AddWithoutRecord);
            }
        }
        public void Add(FigureList figures, bool record)
        {
            if (record)
            {
                using (Transaction.Create(ActionManager, true))
                {
                    figures.ForEach(Add);
                }
            }
            else
            {
                figures.ForEach(AddWithoutRecord);
            }
        }
        public void Remove(System.Collections.Generic.IEnumerable<IFigure> figures)
        {
            using (Transaction.Create(ActionManager, true))
            {
                figures.ForEach(Remove);
            }
        }

        public IEnumerable<IFigure> GetSelectedFigures()
        {
            foreach (IFigure figure in Figures)
            {
                if (figure.Selected)
                {
                    yield return figure;
                }
            }
        }

        public IFigure GetSelectedFigure()
        {
            foreach (IFigure figure in Figures)
            {
                if (figure.Selected)
                {
                    return figure;
                }
            }

            return null;
        }

        #region Events
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public class SelectionChangedEventArgs : EventArgs
        {
            public IEnumerable<IFigure> SelectedFigures { get; set; }

            public SelectionChangedEventArgs()
            {
                SelectedFigures = Enumerable.Empty<IFigure>();
            }

            public SelectionChangedEventArgs(IEnumerable<IFigure> selection)
                : this()
            {
                SelectedFigures = selection;
            }

            public SelectionChangedEventArgs(IFigure singleSelection)
                : this(singleSelection.AsEnumerable())
            {
            }
        }

        internal void ClearSelectedFigures()
        {
            foreach (IFigure figure in this.Figures)
            {
                if (figure.Selected)
                {
                    figure.Selected = false;
                }
            }
        }

        internal void RaiseSelectionChanged(SelectionChangedEventArgs args)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, args);
            }
        }

        public event EventHandler<ConstructionStepCompleteEventArgs> ConstructionStepComplete;
        public class ConstructionStepCompleteEventArgs : EventArgs
        {
            public bool ConstructionComplete { get; set; }
            public Type FigureTypeNeeded { get; set; }
        }
        internal void RaiseConstructionStepComplete(ConstructionStepCompleteEventArgs args)
        {
            if (ConstructionStepComplete != null)
            {
                ConstructionStepComplete(this, args);
            }
        }

        public event EventHandler<ConstructionFeedbackEventArgs> ConstructionFeedback;
        public class ConstructionFeedbackEventArgs : EventArgs
        {
            public bool IsMouseButtonDown { get; set; }
            public Type FigureTypeNeeded { get; set; }
        }
        internal void RaiseConstructionFeedback(ConstructionFeedbackEventArgs args)
        {
            if (ConstructionFeedback != null)
            {
                ConstructionFeedback(this, args);
            }
        }

        #endregion

        public void Delete()
        {
            if (Behavior.DrawVideo)
            {
                List<IFigure> selFigures = this.GetSelectedFigures().ToList();

                var actionRemoveFigure = new RemoveFiguresAction(
                        this,
                        selFigures);

                ActionManager.RecordAction(actionRemoveFigure);

                Figures.UpdateVisual();

                return;
            }

            List<IFigure> selectedFigures = this.GetSelectedFigures().ToList();

            if (selectedFigures.Any(f => f is Game.PBBall))
            {
                selectedFigures.RemoveAll(f => f is Game.PBBall);
            }

            if (selectedFigures.Any(f => f is LineBase))
            {
                if (selectedFigures.All(f => f is LineBase))
                {
                    for (int i = 0; i < selectedFigures.Count(); i++)
                    {
                        if (selectedFigures.ElementAt(i).Dependencies != null && selectedFigures.ElementAt(i).Dependencies.Any(f => f is Game.Zone))
                        {
                            selectedFigures.Add(selectedFigures.ElementAt(i).Dependencies.First(f => f is Game.Zone));
                        }
                    }

                    var actionRemovePlayer = new RemoveFiguresAction(
                        this,
                        selectedFigures);

                    ActionManager.RecordAction(actionRemovePlayer);

                    Figures.UpdateVisual();
                }
            }

            if (selectedFigures.Count() == 1)
            {
                IFigure figure = selectedFigures.ElementAt(0);

                if (figure is Game.PBPlayer)
                {
                    IEnumerable<IFigure> figuresPBPlayer = (figure as Game.PBPlayer).GetDeleteFigure(true);

                    var actionRemovePlayer = new RemoveFiguresAction(
                        this,
                        figuresPBPlayer);

                    ActionManager.RecordAction(actionRemovePlayer);

                    Figures.UpdateVisual();

                    return;
                }

                if (figure is IPoint && figure.Dependents.Count > 1)
                {
                    return;
                }
            }

            List<IFigure> figures = selectedFigures.TopologicalSort(f => f.Dependents).ToList();
            List<IFigure> playersLine = new List<IFigure>();
            foreach (Game.PBPlayer player in figures.OfType<Game.PBPlayer>())
            {
                playersLine.AddRange(player.GetDeleteFigure(false));
            }
            figures.AddRange(playersLine);
            var action = new RemoveFiguresAction(
                this,
                figures);
            ActionManager.RecordAction(action);

            Figures.UpdateVisual();
        }

        public void Run()
        {
            //IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> players = GetSelectedFigures().OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            //if (players != null && players.Count() > 0)
            //{
            //    players.ForEach(f => f.Run());
            //}
            //else
            //{
            //    Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Run());
            //}
            IEnumerable<Game.PBPlayer> players = Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            playerCount = players.Count();
            completeCount = 0;
            players.ForEach(f => f.RunPreRoute(RunPreRouteComplete));
            bPlay = true;

            //DispatcherTimer tm = new DispatcherTimer();
            //tm.Tick += new EventHandler(tm_Tick);
            //tm.Interval = TimeSpan.FromMilliseconds(100);
            //tm.Start();
        }

        int playerCount = 0;
        int completeCount = 0;
        public void RunPreRouteComplete(object sender, EventArgs e)
        {
            completeCount++;

            if (completeCount == playerCount)
            {
                Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Run());
            }
        }

        int i = 0;
        void tm_Tick(object sender, EventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)((Canvas.Parent as FrameworkElement).ActualWidth), (int)((Canvas.Parent as FrameworkElement).ActualHeight), 96, 96, PixelFormats.Default);
            DrawingVisual drawingvisual = new DrawingVisual();

            using (DrawingContext drawingcontext = drawingvisual.RenderOpen())
            {
                VisualBrush visualbrush = new VisualBrush((Canvas.Parent as FrameworkElement));

                drawingcontext.DrawRectangle(visualbrush, null, new Rect(0, 0, (Canvas.Parent as FrameworkElement).ActualWidth, (Canvas.Parent as FrameworkElement).ActualHeight));
            }

            bmp.Render(drawingvisual);

            PngBitmapEncoder enc = new PngBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(string.Format(@"D:\Animation\Animation {0:D4}.PNG",i)))
            {
                enc.Save(stm);
            }

            i++;
        }

        public void Stop()
        {
            //IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> players = GetSelectedFigures().OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            //if (players != null && players.Count() > 0)
            //{
            //    players.ForEach(f => f.Stop());
            //}
            //else
            //{
            //    Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Stop());
            //}
            Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Stop());
        }

        private bool bPlay = false;
        public void Still()
        {
            if (bPlay)
            {
                Pause();
            }
            else
            {
                Resume();
            }
            bPlay = !bPlay;
        }

        public void Pause()
        {
            //IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> players = GetSelectedFigures().OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            //if (players != null && players.Count() > 0)
            //{
            //    players.ForEach(f => f.Pause());
            //}
            //else
            //{
            //    Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Pause());
            //}
            Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Pause());
        }

        public void Resume()
        {
            //IEnumerable<Webb.Playbook.Geometry.Game.PBPlayer> players = GetSelectedFigures().OfType<Webb.Playbook.Geometry.Game.PBPlayer>();
            //if (players != null && players.Count() > 0)
            //{
            //    players.ForEach(f => f.Resume());
            //}
            //else
            //{
            //    Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Resume());
            //}
            Figures.GetAllFiguresRecursive().OfType<Webb.Playbook.Geometry.Game.PBPlayer>().ForEach(f => f.Resume());
        }

        public IEnumerable<IFigure> GetSerializableFigures()
        {
            // first points
            foreach (IFigure figure in Figures)
            {
                if (figure is IPoint)
                {
                    yield return figure;
                }
            }

            // not point
            foreach (IFigure figure in Figures)
            {
                if (!(figure is IPoint))
                {
                    yield return figure;
                }
            }
        }
        public IEnumerable<IFigure> GetPBplayer()
        {
            foreach (IFigure figure in Figures)
            {
                if (figure is Game.PBPlayer)
                {
                    yield return figure;
                }
            }
        }
        // 09-20-2010 Scott
        public Game.PBPlayer GetSelectedPlayer()
        {
            Game.PBPlayer player = null;

            IEnumerable<IFigure> fs = GetSelectedFigures();

            if (fs.Count() > 0 && fs.Any(f => f is Game.PBPlayer))
            {
                IEnumerable<Game.PBPlayer> players = fs.OfType<Game.PBPlayer>();

                if (players.Count() > 0)
                {
                    player = players.Last();
                }
            }

            return player;
        }
        public List<Game.PBPlayer> GetSelectedPlayers()
        {
            List<Game.PBPlayer> players = new List<Webb.Playbook.Geometry.Game.PBPlayer>();

            IEnumerable<IFigure> fs = GetSelectedFigures();

            if (fs.Count() > 0 && fs.Any(f => f is Game.PBPlayer))
            {
                players = fs.OfType<Game.PBPlayer>().ToList();
            }

            return players;
        }
        public Point GetBallPoint()
        {
            Point point = new Point(0, 0);
            foreach (IFigure figure in Figures)
            {
                if (figure is Game.PBBall)
                {
                    point = (figure as Game.PBBall).Coordinates;

                }

            }
            return point;

        }
        public void MoveBall()
        {
            foreach (IFigure figure in Figures)
            {
                if (figure is Game.PBBall)
                {
                    Figures.Remove(figure);
                    break;
                }
            }
        }

        public void TransparentRoute(double opacity)    // 11-04-2010 Scott
        {
            foreach (Game.PBPlayer player in GetPBplayer())
            {
                player.GetAllPathFigure().OfType<Webb.Playbook.Geometry.Shapes.LineBase>().ForEach(f => f.Opacity = opacity);
            }

            this.Figures.UpdateVisual();
        }

        public FigureList GetDefFigures()
        {
            FigureList deffigurelist = new FigureList();
            List<IFigure> pathFigures;
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 1)
                {

                    pathFigures = pbplayer.GetPatheExceptEndPlayer(pbplayer, null, 0).ToList();
                    deffigurelist.Add(pbplayer as IFigure);
                    foreach (IFigure ifigure in pathFigures)
                    {
                        deffigurelist.Add(ifigure);
                    }

                }
            }

            return deffigurelist;
        }
        public FigureList GetDefPBPlayer()
        {
            FigureList defpbplayer = new FigureList();
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 1)
                {
                    defpbplayer.Add(pbplayer);
                }
            }
            Point point = new Point(0, 0);
            foreach (IFigure figure in Figures)
            {
                if (figure is Game.PBBall)
                {
                    defpbplayer.Add(figure);
                    break;
                }
            }
            return defpbplayer;

        }
        public FigureList GetOffPBPlayer()
        {
            FigureList defpbplayer = new FigureList();
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 0)
                {
                    defpbplayer.Add(pbplayer);
                }
            }
            Point point = new Point(0, 0);
            foreach (IFigure figure in Figures)
            {
                if (figure is Game.PBBall)
                {
                    defpbplayer.Add(figure);
                    break;
                }
            }
            return defpbplayer;
        }

        public void ResetDefFigures(Point oldpoint, Point newpoint)
        {
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 1)
                {
                    pbplayer.Coordinates = new Point(pbplayer.Coordinates.X + newpoint.X - oldpoint.X,
                        pbplayer.Coordinates.Y + newpoint.Y - oldpoint.Y);
                }
            }
        }

        public void ResetOffFigures(Point newpoint, Point oldpoint)
        {
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 0)
                {
                    pbplayer.Coordinates = new Point(pbplayer.Coordinates.X + newpoint.X - oldpoint.X,
                        pbplayer.Coordinates.Y + newpoint.Y - oldpoint.Y);
                }
            }
        }

        public FigureList GetOffFigures()
        {
            FigureList offfigurelist = new FigureList();
            List<IFigure> pathFigures;
            IEnumerable<IFigure> PbFigure = this.GetPBplayer();
            foreach (Game.PBPlayer pbplayer in PbFigure)
            {
                if (pbplayer.ScoutType == 0)
                {
                    pathFigures = pbplayer.GetPatheExceptEndPlayer(pbplayer, null, 0).ToList();
                    offfigurelist.Add(pbplayer as IFigure);
                    foreach (IFigure ifigure in pathFigures)
                    {
                        offfigurelist.Add(ifigure);
                    }

                }
            }
            return offfigurelist;
        }
        public void VReverse()
        {
            if (Figures.Any(f => f is Game.PBBall))
            {
                Figures.ForEach(f => f.BallVReverse(GetBallPoint()));
            }
            else
            {
                Figures.ForEach(f => f.VReverse());
            }
            Figures.UpdateVisual();
        }

        public void HReverse()
        {
            // 08-19-2011 Scott
            string strDir = AppDomain.CurrentDomain.BaseDirectory + "LineMen";
            
            if (System.IO.Directory.Exists(strDir))
            {
                System.IO.Directory.Delete(strDir, true);
            }

            System.IO.Directory.CreateDirectory(strDir);

            foreach(Game.PBPlayer player in Figures.OfType<Game.PBPlayer>().Where(p => p.Name.StartsWith("OL",false,null)))
            {
                player.SaveRoute(strDir + @"\" + player.Name);
            }

            List<Game.PBPlayer> lmPlayers = new List<Game.PBPlayer>();
            // end

            if (Figures.Any(f => f is Game.PBBall))
            {
                // 07-27-2011 Scott
                Figures.OfType<Game.PBPlayer>().ForEach(p =>
                {
                    if (!p.Name.StartsWith("OL"))   // 01-17-2011 Scott
                    {
                        p.GetAllPathPointFigure().ForEach(f => f.BallHReverse(GetBallPoint()));
                        p.BallHReverse(GetBallPoint());
                    }
                    else
                    {
                        // 08-19-2011 Scott
                        switch (p.Name)
                        {
                            case "OL1":
                                p.GetAllPathPointFigure().ForEach(f => f.BallHReverse(GetBallPoint()));
                                p.BallHReverse(GetBallPoint());
                                break;
                            default:
                                lmPlayers.Add(p);
                                break;
                        }
                        // end
                    }
                });

                // 08-19-2011 Scott
                foreach (Game.PBPlayer lmPlayer in lmPlayers)
                {
                    switch (lmPlayer.Name)
                    {
                        case "OL2":
                            if (System.IO.File.Exists(strDir + @"\OL3"))
                            {
                                lmPlayer.LoadRoute(strDir + @"\OL3");
                                lmPlayer.GetAllPathPointFigure().ForEach(f => f.BallHReverse(lmPlayer.Coordinates));
                            }
                            break;
                        case "OL3":
                            if (System.IO.File.Exists(strDir + @"\OL2"))
                            {
                                lmPlayer.LoadRoute(strDir + @"\OL2");
                                lmPlayer.GetAllPathPointFigure().ForEach(f => f.BallHReverse(lmPlayer.Coordinates));
                            }
                            break;
                        case "OL4":
                            if (System.IO.File.Exists(strDir + @"\OL5"))
                            {
                                lmPlayer.LoadRoute(strDir + @"\OL5");
                                lmPlayer.GetAllPathPointFigure().ForEach(f => f.BallHReverse(lmPlayer.Coordinates));
                            }
                            break;
                        case "OL5":
                            if (System.IO.File.Exists(strDir + @"\OL4"))
                            {
                                lmPlayer.LoadRoute(strDir + @"\OL4");
                                lmPlayer.GetAllPathPointFigure().ForEach(f => f.BallHReverse(lmPlayer.Coordinates));
                            }
                            break;
                    }
                }
                // end

                Figures.OfType<PBLabel>().ForEach(f => f.BallHReverse(GetBallPoint()));
                //Figures.ForEach(f =>
                //    {
                //        //if (!f.Name.StartsWith("OL"))   // 01-17-2011 Scott
                //        //{
                //            f.BallHReverse(GetBallPoint());
                //        //}
                //    });
            }
            else
            {
                Figures.ForEach(f => f.HReverse());
            }

            Figures.UpdateVisual();
        }

        public void VReverse(FigureList figures)
        {
            figures.ForEach(f => f.VReverse());
            Figures.UpdateVisual();
        }

        public void HReverse(FigureList figures)
        {
            if (Figures.Any(f => f is Game.PBBall))
            {
                figures.ForEach(f => f.BallHReverse(GetBallPoint()));
            }
            else
            {
                figures.ForEach(f => f.HReverse());
            }

            Figures.UpdateVisual();

        }

        public bool OnEdit
        {
            get
            {
                bool bRet = false;

                if (Canvas != null && Canvas.Children.Contains(Text.Edit))
                {
                    bRet = true;
                }

                return bRet;
            }
        }

#if !SILVERLIGHT
        //public static string FromFileName(string fileName)  //07-19-2010 scott
        //{
        //    string strReturn = fileName;+

        //    if (fileName.Contains("%s"))
        //    {
        //        strReturn = fileName.Replace("%s","/");
        //    }

        //    return strReturn;
        //}

        //public static string ToFileName(string fileName)    //07-19-2010 scott
        //{
        //    string strReturn = fileName;

        //    if (fileName.Contains("/"))
        //    {
        //        strReturn = fileName.Replace("/", "%s");
        //    }

        //    return strReturn; 
        //}

        public void SetCoordinateByBall()
        {
            IEnumerable<Game.PBBall> balls = this.Figures.OfType<Game.PBBall>();

            if (balls != null && balls.Count() > 0)
            {
                //Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.Y = balls.First().Coordinates.Y;
                //Webb.Playbook.Geometry.CoordinateSystem.LogicalMidPos.X = balls.First().Coordinates.X;
            }
        }

        public void LoadTitleBackground()
        {
            if (Title)
            {
                HideGridLines();

                if (System.IO.File.Exists(BackgroundPath))
                {
                    ImageBrush imgBrush = new ImageBrush()
                    {
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(BackgroundPath, UriKind.RelativeOrAbsolute)),
                    };

                    SetTitleBackground(imgBrush);
                }
                else
                {
                    SetTitleBackground(Brushes.White);
                }
            }

        }

        // 01-16-2012 Scott
        public void SetStartYard(double canvasWidth, double yard)
        {
            if (Canvas != null)
            {
                double canvasHeight = CoordinateSystem.LogicalPlaygroundHeight * canvasWidth / CoordinateSystem.LogicalPlaygroundWidth;
                Point ptBallLogical = new Point(0, yard);

                double UnitLength = canvasWidth / CoordinateSystem.LogicalPlaygroundWidth;
                Point Origin = new Point(canvasWidth / 2, canvasHeight / 2);

                Point ptBallPhysical =  new Point(
                Origin.X + ptBallLogical.X * UnitLength,
                Origin.Y - ptBallLogical.Y * UnitLength);

                double offset = canvasHeight / 2 - ptBallPhysical.Y;

                Canvas.Margin = new Thickness(Canvas.Margin.Left, offset, Canvas.Margin.Right, - offset);
            }
        }

        // 01-16-2012 Scott
        public void SetStartYardByBall()
        {
            if (Canvas != null)
            {
                Point ptBallLogical = GetBallPoint();

                Point ptBallPhysical = CoordinateSystem.ToPhysical(ptBallLogical);

                double offset = Canvas.ActualHeight / 2 - ptBallPhysical.Y;

                Canvas.Margin = new Thickness(Canvas.Margin.Left, offset, Canvas.Margin.Right, - offset);
            }
        }

        public static Drawing Load(string path, Canvas canvas)
        {
            Drawing drawing = new Drawing(canvas);
            if (System.IO.File.Exists(path))
            {
                string text = System.IO.File.ReadAllText(path);
                DrawingDeserializer.ReadDrawing(drawing, text);
                drawing.ShowBall();

                drawing.LoadTitleBackground();
                
                drawing.Figures.UpdateVisual(); //07-23-2009 scott
            }
            return drawing;
        }

        //// 09-15-2010 Scott
        //public static Drawing LoadByScoutType(string path, int scoutType, Canvas canvas)
        //{
        //    Drawing drawing = new Drawing(canvas);
        //    string text = System.IO.File.ReadAllText(path);
        //    DrawingDeserializer.ReadDrawing(drawing, text);
        //    drawing.Figures.UpdateVisual(); //07-23-2009 scott
        //    return drawing;
        //}

        public static Drawing Load(string pathObj, string pathOpp, Canvas canvas)
        {
            Drawing drawing = new Drawing(canvas);

            // 08-18-2011 Scott
            if (System.IO.File.Exists(pathOpp))
            {
                string textOpp = System.IO.File.ReadAllText(pathOpp);
                DrawingDeserializer.ReadDrawingUpTeam(drawing, textOpp);
                drawing.PointDef = drawing.GetBallPoint();
            }

            if (System.IO.File.Exists(pathObj))
            {
                drawing.MoveBall();
                string textObj = System.IO.File.ReadAllText(pathObj);
                DrawingDeserializer.ReadDrawingBottomTeam(drawing, textObj);
                drawing.PointOff = drawing.GetBallPoint();
                drawing.ResetDefFigures(drawing.pointdef, drawing.PointOff);
            }

            drawing.ShowBall();

            drawing.LoadTitleBackground();

            drawing.Figures.UpdateVisual(); //07-23-2009 scott
            return drawing;
        }

        public static Drawing Load(string pathObj, string pathOpp, Canvas canvas, int scoutType)
        {
            Drawing drawing = new Drawing(canvas);

            if (scoutType == 0)
            {
                if (System.IO.File.Exists(pathOpp))
                {
                    if (scoutType == 0)
                    {
                        drawing.placeHolder = true;
                    }

                    string textOpp = System.IO.File.ReadAllText(pathOpp);
                    DrawingDeserializer.ReadDrawingReverse(drawing, textOpp);
                    drawing.PointDef = drawing.GetBallPoint();

                    drawing.PlaceHolder = false;
                }

                if (System.IO.File.Exists(pathObj))
                {
                    if (scoutType == 1)
                    {
                        drawing.PlaceHolder = true;
                    }

                    drawing.MoveBall();

                    string textObj = System.IO.File.ReadAllText(pathObj);
                    DrawingDeserializer.ReadDrawing(drawing, textObj);
                    drawing.PointOff = drawing.GetBallPoint();

                    drawing.PlaceHolder = false;
                }
            }
            else if(scoutType == 1)
            {
                if (System.IO.File.Exists(pathObj))
                {
                    if (scoutType == 1)
                    {
                        drawing.PlaceHolder = true;
                    }

                    string textObj = System.IO.File.ReadAllText(pathObj);
                    DrawingDeserializer.ReadDrawing(drawing, textObj);
                    drawing.PointOff = drawing.GetBallPoint();

                    drawing.PlaceHolder = false;
                }

                if (System.IO.File.Exists(pathOpp))
                {
                    if (scoutType == 0)
                    {
                        drawing.placeHolder = true;
                    }

                    drawing.MoveBall();

                    string textOpp = System.IO.File.ReadAllText(pathOpp);
                    DrawingDeserializer.ReadDrawingReverse(drawing, textOpp);
                    drawing.PointDef = drawing.GetBallPoint();

                    drawing.PlaceHolder = false;
                }
            }

            if (scoutType == 0)
            {
                drawing.ResetDefFigures(drawing.PointDef, drawing.PointOff);
            }

            if (scoutType == 1)
            {
                drawing.ResetOffFigures(drawing.PointDef, drawing.PointOff);
            }

            drawing.ShowBall();

            drawing.LoadTitleBackground();

            drawing.Figures.UpdateVisual(); //07-23-2009 scott
            return drawing;
        }

        public void ShowBall()
        {
            if (Figures.OfType<Game.PBBall>().Count() > 0)
            {
                Game.PBBall ball = Figures.OfType<Game.PBBall>().First();

                ball.Visible = Webb.Playbook.Data.GameSetting.Instance.ShowBall;

                ball.UpdateVisual();
            }
        }

        // add for Set
        public void SaveDef(string path)
        {
            GridLines.Remove();
            DrawingSerializer.SaveDefToXml(this, path);
            GridLines.Add();
        }

        public void SaveOff(string path)
        {
            GridLines.Remove();
            DrawingSerializer.SaveOffToXml(this, path);
            GridLines.Add();
        }

        public void SaveKick(string path)
        {
            GridLines.Remove();
            DrawingSerializer.SaveKickToXml(this, path);
            GridLines.Add();
        }

        public void SaveToDiagram(string path, bool bPlayground)
        {
            // for test 08-12-2010 Scott
            try
            {
                Diagram diagram = new Diagram(this);
                System.IO.FileInfo fi = new System.IO.FileInfo(path);
                diagram.Scale(Webb.Playbook.Data.GameSetting.Instance.Scaling / 200);
                diagram.TranslateToDiagram(path, bPlayground);
            }
            catch
            {
                MessageBox.Show("Can't register webbgamedata.dll !");
            }
        }

        public void RemovePlaceHolder()
        {
            IEnumerable<IFigure> figuresPlaceHolder = Figures.SkipWhile(f => f.PlaceHolder == false);
            for (int i = figuresPlaceHolder.Count() - 1; i >= 0; i--)
            {
                if (figuresPlaceHolder.ElementAt(i).PlaceHolder)
                {
                    Figures.Remove(figuresPlaceHolder.ElementAt(i));
                }
            }
        }

        public void Save(string path)
        {
            //path = ToFileName(path); //07-19-2010 scott

            CurrentAction = ActionManager.History.CurrentState.PreviousAction;
            RemovePlaceHolder();
            GridLines.Remove();
            DrawingSerializer.Save(this, path);
            GridLines.Add();
        }

        public void TempSave(string path)
        {
            //path = ToFileName(path); //07-19-2010 scott

            GridLines.Remove();
            DrawingSerializer.Save(this, path);
            GridLines.Add();
        }

        public void SaveToXml(string path)
        {
            GridLines.Remove();
            DrawingSerializer.SaveToXml(this, path);
            GridLines.Add();
        }

        public void SaveToXml(string gameName, string path, bool ourTeamOffensive)
        {
            GridLines.Remove();
            DrawingSerializer.SaveToXml(this, path, gameName, ourTeamOffensive);
            GridLines.Add();
        }
#endif
    }
}