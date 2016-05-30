using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

using Webb.Playbook.Geometry.Actions;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry.Shapes.Creators
{
    public abstract class FigureCreator : Behavior
    {
        #region State machine

        public override void Started()
        {
            Reset();
        }

        public override void Stopping()
        {
            //Rollback();
        }

        protected override void Reset()
        {
            TempResult = null;
            RemoveIntermediateFigureIfNecessary();
            RemoveTempPointIfNecessary();
            Transaction = CreateTransaction();
            ExpectedDependencies = InitExpectedDependencies();
            InitFoundDependencies();
        }

        protected void Finish()
        {
            if (TempResult == null)
            {
                CreateAndAddFigure();
            }
            TempResult.RecalculateAndUpdateVisual();
            Commit();
            Drawing.RaiseConstructionStepComplete(new Drawing.ConstructionStepCompleteEventArgs()
            {
                ConstructionComplete = true
            });
            Reset();
        }

        #endregion

        #region Transaction

        public Transaction Transaction { get; set; }

        public Transaction CreateTransaction()
        {
            return Transaction.Create(Drawing.ActionManager, false);
        }

        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }
            RemoveTempPointIfNecessary();
            RemoveTempResultIfNecessary();
            RemoveIntermediateFigureIfNecessary();
        }

        #endregion

        #region Intermediate results

        public IFigure TempResult { get; set; }
        public FreePoint TempPoint { get; set; }
        public IFigure IntermediateFigure { get; set; }

        protected FreePoint CreateTempPoint(System.Windows.Point coordinates)
        {
            var result = CreatePointAtCurrentPosition(coordinates, false);

            result.Shape.Fill = new SolidColorBrush(Colors.Cyan);
            result.ZIndex = int.MinValue;
            return result;
        }

        protected void AddIntermediateFigureIfNecessary()
        {
            if (TempResult == null)
            {
                IntermediateFigure = CreateIntermediateFigure();
                if (IntermediateFigure != null)
                {
                    Drawing.Figures.Add(IntermediateFigure);
                }
            }
        }

        protected virtual IFigure CreateIntermediateFigure()
        {
            return null;
        }

        protected void RemoveIntermediateFigureIfNecessary()
        {
            if (IntermediateFigure != null)
            {
                Drawing.Figures.Remove(IntermediateFigure);
                IntermediateFigure = null;
            }
        }

        private void RemoveTempResultIfNecessary()
        {
            if (TempResult != null)
            {
                Drawing.Figures.Remove(TempResult);
                TempResult = null;
            }
        }

        protected void RemoveTempPointIfNecessary()
        {
            if (TempPoint != null)
            {
                Drawing.Figures.Remove(TempPoint);
                TempPoint = null;
            }
        }

        #endregion

        #region Creating the figure

        protected abstract IFigure CreateFigure();

        protected virtual void CreateAndAddFigure()
        {
            TempResult = CreateFigure();
            Drawing.Add(TempResult);
        }

        #endregion

        #region Found & next dependencies

        protected void InitFoundDependencies()
        {
            FoundDependencies = new FigureList();
        }

        private DependencyList ExpectedDependencies { get; set; }
        protected FigureList FoundDependencies { get; set; }

        protected virtual Type ExpectedDependency
        {
            get
            {
                if (FoundDependencies.Count < ExpectedDependencies.Count)
                {
                    return ExpectedDependencies[FoundDependencies.Count];
                }
                return null;
            }
        }

        protected virtual bool ExpectingAPoint()
        {
            return typeof(IPoint).IsAssignableFrom(ExpectedDependency);
        }

        protected void AdvertiseNextDependency()
        {
            var nextDependency = ExpectedDependency;
            if (nextDependency == null && TempPoint != null)
            {
                nextDependency = typeof(IPoint);
            }
            Drawing.RaiseConstructionStepComplete(new Drawing.ConstructionStepCompleteEventArgs()
            {
                ConstructionComplete = false,
                FigureTypeNeeded = nextDependency
            });
        }

        protected virtual bool CanReuseDependency()
        {
            return false;
        }

        protected abstract DependencyList InitExpectedDependencies();

        protected virtual void AddFoundDependency(IFigure figure)
        {
            if (figure != null && ExpectedDependency.IsAssignableFrom(figure.GetType()))
            {
                FoundDependencies.Add(figure);
            }
        }

        #endregion

        #region State machine transition on clicking

        /// <summary>
        /// Assumes coordinates are logical already
        /// </summary>
        /// <param name="coordinates">Logical coordinates of the click point</param>
        protected virtual void Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point coordinates = Coordinates(e);

            IFigure underMouse = null;
            Type expectedType = ExpectedDependency;

            if (TempPoint != null)
            {
                underMouse = Drawing.Figures.HitTest(
                    coordinates,
                    typeof(IPoint));
            }
            else if (expectedType != null)
            {
                underMouse = Drawing.Figures.HitTest(coordinates, expectedType);
            }
            else
            {
                underMouse = Drawing.Figures.HitTest(coordinates);
            }

            if (underMouse != null
                && underMouse != TempPoint
                && ((FoundDependencies.Contains(underMouse) && !CanReuseDependency())
                    || underMouse == TempResult))
            {
                return;
            }

            if (ExpectingAPoint())
            {
                if (underMouse == null)
                {
                    //underMouse = CreatePointAtCurrentPosition(coordinates, true);
                    return;
                }
                else
                {
                    // one branch only
                    if (underMouse is Webb.Playbook.Geometry.Game.PBPlayer && underMouse.Dependents.Count > 0)
                    {
                        return;
                    }
                    // at most two branch
                    if (underMouse is IPoint && underMouse.Dependents.Count > 1)
                    {
                        return;
                    }
                }
            }

            RemoveIntermediateFigureIfNecessary();

            if (TempPoint != null)
            {
                //if (underMouse == TempPoint || underMouse == TempResult || underMouse == null)
                //{
                    underMouse = CreatePointAtCurrentPosition(coordinates, true);
                //}
                TempPoint.SubstituteWith(underMouse);
                FoundDependencies.Remove(TempPoint);
                Drawing.Figures.Remove(TempPoint);
                TempPoint = null;
            }

            if (ExpectedDependency != null)
            {
                AddFoundDependency(underMouse);
            }

            if (ExpectedDependency != null)
            {
                if (ExpectingAPoint())
                {
                    TempPoint = CreateTempPoint(coordinates);
                    AddFoundDependency(TempPoint);
                    if (ExpectedDependency == null)
                    {
                        CreateAndAddFigure();
                    }
                }
                AddIntermediateFigureIfNecessary();
                AdvertiseNextDependency();
            }
            else
            {
                Finish();

                //07-22-2009 scott
                if (IsMouseButtonDown)
                {// click mode
                    IFigure endFigure = Drawing.Figures.HitTest(coordinates, typeof(Webb.Playbook.Geometry.Game.PBPlayer));
                    if (endFigure is Webb.Playbook.Geometry.Game.PBPlayer)
                    {
                        Drawing.ActionManager.Undo();
                    }
                    else
                    {
                        MouseLeftButtonDown(sender, e as System.Windows.Input.MouseButtonEventArgs);
                    }
                }
                else
                {// drag mode

                }
            }

            Drawing.Figures.CheckConsistency();
        }

        #endregion

        #region MouseDown, MouseMove, MouseUp

        Point MouseDownCoordinates;
        public bool IsMouseButtonDown { get; set; }

        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsMouseButtonDown = true;
            MouseDownCoordinates = Coordinates(e);
            Click(sender,e);
        }

        protected override void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TempPoint != null)
            {
                TempPoint.MoveTo(Coordinates(e));
                TempPoint.RecalculateAllDependents();
            }
            Drawing.RaiseConstructionFeedback(new Drawing.ConstructionFeedbackEventArgs()
            {
                FigureTypeNeeded = ExpectedDependency,
                IsMouseButtonDown = IsMouseButtonDown
            });
        }

        protected override void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var coordinates = Coordinates(e);
            IsMouseButtonDown = false;
            if (TempPoint != null && coordinates.Distance(MouseDownCoordinates) > CursorTolerance)
            {
                Click(sender,e);
            }
        }

        #endregion
    }

    public abstract class ClassicFigureCreator : Behavior
    {
        #region State machine

        public override void Started()
        {
            Reset();
        }

        public override void Stopping()
        {
            Rollback();
        }

        protected override void Reset()
        {
            TempResult = null;
            RemoveIntermediateFigureIfNecessary();
            RemoveTempPointIfNecessary();
            Transaction = CreateTransaction();
            ExpectedDependencies = InitExpectedDependencies();
            InitFoundDependencies();
        }

        protected void Finish()
        {
            if (TempResult == null)
            {
                CreateAndAddFigure();
            }
            TempResult.RecalculateAndUpdateVisual();
            Commit();
            Drawing.RaiseConstructionStepComplete(new Drawing.ConstructionStepCompleteEventArgs()
            {
                ConstructionComplete = true
            });
            Reset();
        }

        #endregion

        #region Transaction

        public Transaction Transaction { get; set; }

        public Transaction CreateTransaction()
        {
            return Transaction.Create(Drawing.ActionManager, false);
        }

        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }
            RemoveTempPointIfNecessary();
            RemoveTempResultIfNecessary();
            RemoveIntermediateFigureIfNecessary();
        }

        #endregion

        #region Intermediate results

        public IFigure TempResult { get; set; }
        public FreePoint TempPoint { get; set; }
        public IFigure IntermediateFigure { get; set; }

        private FreePoint CreateTempPoint(System.Windows.Point coordinates)
        {
            var result = CreatePointAtCurrentPosition(coordinates, false);
            result.Shape.Fill = new SolidColorBrush(Colors.Cyan);
            result.ZIndex = int.MinValue;
            return result;
        }

        private void AddIntermediateFigureIfNecessary()
        {
            if (TempResult == null)
            {
                IntermediateFigure = CreateIntermediateFigure();
                if (IntermediateFigure != null)
                {
                    Drawing.Figures.Add(IntermediateFigure);
                }
            }
        }

        protected virtual IFigure CreateIntermediateFigure()
        {
            return null;
        }

        protected void RemoveIntermediateFigureIfNecessary()
        {
            if (IntermediateFigure != null)
            {
                Drawing.Figures.Remove(IntermediateFigure);
                IntermediateFigure = null;
            }
        }

        private void RemoveTempResultIfNecessary()
        {
            if (TempResult != null)
            {
                Drawing.Figures.Remove(TempResult);
                TempResult = null;
            }
        }

        protected void RemoveTempPointIfNecessary()
        {
            if (TempPoint != null)
            {
                Drawing.Figures.Remove(TempPoint);
                TempPoint = null;
            }
        }

        #endregion

        #region Creating the figure

        protected abstract IFigure CreateFigure();

        protected virtual void CreateAndAddFigure()
        {
            TempResult = CreateFigure();
            Drawing.Add(TempResult);
        }

        #endregion

        #region Found & next dependencies

        protected void InitFoundDependencies()
        {
            FoundDependencies = new FigureList();
        }

        private DependencyList ExpectedDependencies { get; set; }
        protected FigureList FoundDependencies { get; set; }

        protected virtual Type ExpectedDependency
        {
            get
            {
                if (FoundDependencies.Count < ExpectedDependencies.Count)
                {
                    return ExpectedDependencies[FoundDependencies.Count];
                }
                return null;
            }
        }

        protected virtual bool ExpectingAPoint()
        {
            return typeof(IPoint).IsAssignableFrom(ExpectedDependency);
        }

        private void AdvertiseNextDependency()
        {
            var nextDependency = ExpectedDependency;
            if (nextDependency == null && TempPoint != null)
            {
                nextDependency = typeof(IPoint);
            }
            Drawing.RaiseConstructionStepComplete(new Drawing.ConstructionStepCompleteEventArgs()
            {
                ConstructionComplete = false,
                FigureTypeNeeded = nextDependency
            });
        }

        protected virtual bool CanReuseDependency()
        {
            return false;
        }

        protected abstract DependencyList InitExpectedDependencies();

        protected virtual void AddFoundDependency(IFigure figure)
        {
            if (figure != null && ExpectedDependency.IsAssignableFrom(figure.GetType()))
            {
                FoundDependencies.Add(figure);
            }
        }

        #endregion

        #region State machine transition on clicking

        /// <summary>
        /// Assumes coordinates are logical already
        /// </summary>
        /// <param name="coordinates">Logical coordinates of the click point</param>
        protected virtual void Click(System.Windows.Point coordinates)
        {
            IFigure underMouse = null;
            Type expectedType = ExpectedDependency;

            if (TempPoint != null)
            {
                underMouse = Drawing.Figures.HitTest(
                    coordinates,
                    typeof(IPoint));
            }
            else if (expectedType != null)
            {
                underMouse = Drawing.Figures.HitTest(coordinates, expectedType);
            }
            else
            {
                underMouse = Drawing.Figures.HitTest(coordinates);
            }

            if (underMouse != null
                && underMouse != TempPoint
                && ((FoundDependencies.Contains(underMouse) && !CanReuseDependency())
                    || underMouse == TempResult))
            {
                return;
            }

            if (ExpectingAPoint())
            {
                if (underMouse == null)
                {
                    underMouse = CreatePointAtCurrentPosition(coordinates, true);
                }
            }

            RemoveIntermediateFigureIfNecessary();

            if (TempPoint != null)
            {
                if (underMouse == TempPoint || underMouse == TempResult || underMouse == null)
                {
                    underMouse = CreatePointAtCurrentPosition(coordinates, true);
                }
                TempPoint.SubstituteWith(underMouse);
                FoundDependencies.Remove(TempPoint);
                Drawing.Figures.Remove(TempPoint);
                TempPoint = null;
            }

            if (ExpectedDependency != null)
            {
                AddFoundDependency(underMouse);
            }

            if (ExpectedDependency != null)
            {
                if (ExpectingAPoint())
                {
                    TempPoint = CreateTempPoint(coordinates);
                    AddFoundDependency(TempPoint);
                    if (ExpectedDependency == null)
                    {
                        CreateAndAddFigure();
                    }
                }
                AddIntermediateFigureIfNecessary();
                AdvertiseNextDependency();
            }
            else
            {
                Finish();
            }

            Drawing.Figures.CheckConsistency();
        }

        #endregion

        #region MouseDown, MouseMove, MouseUp

        Point MouseDownCoordinates;
        public bool IsMouseButtonDown { get; set; }

        protected override void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Parent);

            LButtonDown(pt);
        }

        private void LButtonDown(Point pt)
        {
            IsMouseButtonDown = true;

            MouseDownCoordinates = ToLogical(pt);

            Click(MouseDownCoordinates);
        }

        protected override void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point pt = e.GetPosition(Parent);

            MouseMove(pt);
        }

        private void MouseMove(Point pt)
        {
            Point logPt = ToLogical(pt);

            if (TempPoint != null)
            {
                TempPoint.MoveTo(logPt);
                TempPoint.RecalculateAllDependents();
            }
            Drawing.RaiseConstructionFeedback(new Drawing.ConstructionFeedbackEventArgs()
            {
                FigureTypeNeeded = ExpectedDependency,
                IsMouseButtonDown = IsMouseButtonDown
            });
        }

        protected override void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Parent);

            LButtonUp(pt);
        }

        private void LButtonUp(Point pt)
        {
            var coordinates = ToLogical(pt);

            IsMouseButtonDown = false;
            
            if (TempPoint != null && coordinates.Distance(MouseDownCoordinates) > CursorTolerance)
            {
                Click(coordinates);
            }
        }
        #endregion

        public override IntPtr WndProc(UIElement ui, UIElement uiRelatedTo, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool b)
        {
            switch (msg)
            {
                case (int)Data.MessageCommands.WM_LBUTTONDOWN:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        LButtonDown(pt);
                    }
                    break;
                case (int)Data.MessageCommands.WM_LBUTTONUP:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        LButtonUp(pt);
                    }
                    break;
                case (int)Data.MessageCommands.WM_RBUTTONDOWN:
                    break;
                case (int)Data.MessageCommands.WM_RBUTTONUP:
                    break;
                case (int)Data.MessageCommands.WM_MOUSEMOVE:
                    {
                        Point pt = Data.MessageHelper.GetPosition(lParam);
                        pt = ui.TranslatePoint(pt, uiRelatedTo);
                        MouseMove(pt);
                    }
                    break;
                default:
                    break;
            }

            return base.WndProc(ui, uiRelatedTo, hwnd, msg, wParam, lParam, ref b);
        }
    }
}
