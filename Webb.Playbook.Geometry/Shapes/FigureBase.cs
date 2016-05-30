using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

using System.Windows.Shapes;
using Webb.Playbook.Geometry.Lists;
using Webb.Playbook.Geometry.Actions;

namespace Webb.Playbook.Geometry.Shapes
{
    public interface ILinearFigure : IFigure
    {
        double GetNearestParameterFromPoint(Point point);
        Point GetPointFromParameter(double parameter);
        Tuple<double, double> GetParameterDomain();
    }

    public interface IFigure : IEquatable<IFigure>
    {
        Drawing Drawing { get; set; }
        string Name { get; set; }
        bool Selected { get; set; }
        IFigure HitTest(Point point);
        IFigure HitTest(Rect rect);
        void UpdateVisual();
        void Recalculate();

        IEnumerable<IFigure> Dependencies { get; set; }
        IFigureList Dependents { get; }

        int ZIndex { get; set; }
        bool Visible { get; set; }
        bool PlaceHolder { get; set; }

        void WriteXml(XmlWriter writer);
        void ReadXml(XElement element);

        void OnAddingToCanvas(Canvas newContainer);
        void OnRemovingFromCanvas(Canvas leavingContainer);

        void VReverse();
        void HReverse();
        void BallHReverse(Point point);
        void BallVReverse(Point point);
    }

    public abstract class FigureBase : IFigure
    {
        public FigureBase()
        {
            ID = ++nextID;
            Name = this.GetType().Name + FigureBase.ID;
            Visible = true;
            PlaceHolder = false;
        }

        public override string ToString()
        {
            return Name;
        }

        public Drawing Drawing { get; set; }

        protected Canvas Canvas
        {
            get
            {
                return Drawing.Canvas;
            }
        }

        public string Name { get; set; }

        public virtual bool Selected { get; set; }

        public virtual bool Visible { get; set; }

        public virtual bool PlaceHolder { get; set; }

        public virtual Point Coordinates { get; set; }

        private bool enableColor = true;
        public bool EnableColor 
        {
            get
            {
                return enableColor;
            }
            set
            {
                enableColor = value;
            }
        }

        public double angle = 0;
        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public virtual void UpdateVisual() 
        {
            
        }

        public virtual void OnAddingToCanvas(Canvas newContainer)
        {
            Canvas.SetZIndex(newContainer, this.ZIndex);
        }

        public virtual void OnRemovingFromCanvas(Canvas leavingContainer)
        {

        }

        public virtual void WriteXml(XmlWriter writer)
        {

        }

        public virtual void ReadXml(XElement element)
        {

        }

        public virtual void HReverse()
        {
            
        }
        public virtual void BallHReverse(Point point)
        {

        }
        public virtual void VReverse()
        {

        }
        public virtual void BallVReverse(Point point)
        {
        
        }
        private static int nextID = 0;
        public static int ID
        { 
            get { return nextID; }
            set { nextID = value; }
        }

        public virtual int ZIndex { get; set; }

        public virtual void Recalculate() { }

        public abstract IFigure HitTest(Point point);

        public abstract IFigure HitTest(Rect rect);

        public bool Equals(IFigure other)
        {
            return object.ReferenceEquals(this, other);
        }

        #region Coordinates
        protected double CursorTolerance
        {
            get
            {
                return Drawing.CoordinateSystem.CursorTolerance;
            }
        }

        protected double ToPhysical(double logicalLength)
        {
            return Drawing.CoordinateSystem.ToPhysical(logicalLength);
        }

        protected Point ToPhysical(Point point)
        {
            return Drawing.CoordinateSystem.ToPhysical(point);
        }

        protected PointPair ToPhysical(PointPair pointPair)
        {
            return Drawing.CoordinateSystem.ToPhysical(pointPair);
        }

        protected double ToLogical(double pixelLength)
        {
            return Drawing.CoordinateSystem.ToLogical(pixelLength);
        }

        protected Point ToLogical(Point pixel)
        {
            return Drawing.CoordinateSystem.ToLogical(pixel);
        }

        protected PointPair ToLogical(PointPair pointPair)
        {
            return Drawing.CoordinateSystem.ToLogical(pointPair);
        }
        #endregion

        private IEnumerable<IFigure> mDependencies;
        public IEnumerable<IFigure> Dependencies
        {
            get
            {
                return mDependencies;
            }
            set
            {
                mDependencies = value;
            }
        }

        private IFigureList mDependents;
        public IFigureList Dependents
        {
            get
            {
                if (mDependents == null)
                {
                    mDependents = new FigureList();
                }
                return mDependents;
            }
            protected set
            {
                mDependents = value;
            }
        }

        public virtual void Delete()
        {
            var allDependents = this.AsEnumerable<IFigure>().TopologicalSort(f => f.Dependents);

            using (var transaction = Transaction.Create(Drawing.ActionManager, false))
            {
                foreach (var figure in allDependents)
                {
                    RemoveFigureAction action = new RemoveFigureAction(Drawing, figure);
                    Drawing.ActionManager.RecordAction(action);
                }
            }

            Drawing.RaiseSelectionChanged(new Drawing.SelectionChangedEventArgs());
        }

        public Point Point(int index)
        {
            //add by scott 01-19-2010
            IFigure f = Dependencies.ElementAt(index);
            IPoint ip = f as IPoint;
            System.Windows.Point pt = new Point(ip.Coordinates.X, ip.Coordinates.Y);
            if ((f is Game.Zone || f is ImmovablePoint))
            {
                pt.X += CoordinateSystem.LogicalMidPos.X;
                return pt;
            }
            //end
            return (Dependencies.ElementAt(index) as IPoint).Coordinates;
        }

        public PointPair Line(int index)
        {
            return (Dependencies.ElementAt(index) as ILine).Coordinates;
        }
    }
}
