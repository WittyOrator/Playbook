using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Lists
{
    public interface IFigureList :
        IFigure,
        IList<IFigure>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        IFigureList Clone();
    }

    public class FigureList : CollectionWithEvents<IFigure>, IFigureList
    {
        public FigureList()
        {
        }

        public FigureList(IEnumerable<IFigure> existingListToClone)
            : this()
        {
            this.AddRange(existingListToClone);   
        }

        public virtual void HReverse()
        {
            this.GetAllFiguresRecursive().ForEach(f => f.HReverse());
        }
        public virtual void BallHReverse(Point point)
        {
            this.GetAllFiguresRecursive().ForEach(f => f.BallHReverse(point));

        }
        public virtual void VReverse()
        {
            this.GetAllFiguresRecursive().ForEach(f => f.VReverse());
        }
        public virtual void BallVReverse(Point point)
        {
            this.GetAllFiguresRecursive().ForEach(f => f.BallVReverse(point));
        }

        public IFigure this[string name]
        {
            get
            {
                return this.GetAllFiguresRecursive().Where(f => f.Name == name).FirstOrDefault();
            }
        }

        private Drawing mDrawing = null;
        public Drawing Drawing
        {
            get
            {
                return mDrawing;
            }
            set
            {
                mDrawing = value;
                this.ForEach(f => f.Drawing = value);
            }
        }

        public void Add(params IFigure[] figures)
        {
            foreach (var figure in figures)
            {
                if (figure.Drawing == null)
                {
                    figure.Drawing = this.Drawing;
                }
                base.Add(figure);
            }
        }

        public void Remove(params IFigure[] figures)
        {
            foreach (var figure in figures)
            {
                base.Remove(figure);
            }
        }

        public void Remove(IEnumerable<IFigure> figures)
        {
            foreach (var figure in figures.ToArray())
            {
                base.Remove(figure);
            }
        }

        public virtual IEnumerable<IFigure> Dependencies { get; set; }

        private IFigureList mDependents;
        public virtual IFigureList Dependents
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

        public FigureList Clone()
        {
            return new FigureList(this);
        }

        IFigureList IFigureList.Clone()
        {
            return Clone();
        }

        #region HitTest
        /// <summary>
        /// Finds any figure at the point
        /// </summary>
        /// <param name="point">Hittest coordinates</param>
        /// <returns>A figure with topmost ZIndex or null if nothing found</returns>
        public IFigure HitTest(Point point)
        {
            return HitTest(point, figure => figure.Visible);
        }

        /// <summary>
        /// Finds a figure of a given type at the point
        /// </summary>
        /// <param name="point">Coordinates</param>
        /// <param name="figureType">A type (usually typeof(IPoint), typeof(ILine) or typeof(ICircle)
        /// but could be anything)</param>
        /// <returns>A figure or null if nothing was found</returns>
        public IFigure HitTest(Point point, Type figureType)
        {
            return HitTest(point, figure =>
                figure != null && figureType.IsAssignableFrom(figure.GetType()));
        }

        /// <summary>
        /// Finds a figure of a given type at the point
        /// </summary>
        /// <typeparam name="T">A type (usually IPoint, ILine or ICircle
        /// but could be anything)</typeparam>
        /// <param name="point">Coordinates</param>
        /// <returns>A figure or null if nothing was found</returns>
        public IFigure HitTest<T>(Point point)
        {
            return HitTest(point, typeof(T));
        }

        /// <summary>
        /// Finds a figure at a point
        /// </summary>
        /// <param name="point">Coordinates of a point where we want to find objects</param>
        /// <param name="filter">Determines whether a figure should be included in hit-testing</param>
        /// <returns>A figure with topmost ZIndex that is under the point
        /// and for which the filter is true. Returns null if nothing is found.</returns>
        public virtual IFigure HitTest(Point point, Predicate<IFigure> filter)
        {
            IFigure bestFoundSoFar = null;

            foreach (var item in this.Where(f => f is FigureList || filter(f)))
            {
                IFigure found = item.HitTest(point);

                if (found != null)
                {
                    if (bestFoundSoFar == null || bestFoundSoFar.ZIndex <= found.ZIndex)
                    {
                        bestFoundSoFar = found;
                    }
                }
            }
            return bestFoundSoFar;
        }

        // multi
        public ReadOnlyCollection<IFigure> HitTestMany(Point point)
        {
            List<IFigure> result = new List<IFigure>();

            foreach (var item in this)
            {
                IFigure found = item.HitTest(point);
                if (found != null)
                {
                    result.Add(found);
                }
            }

            if (result.Count > 0)
            {
                result.Sort((f1, f2) => f1.ZIndex.CompareTo(f2.ZIndex));
            }

            return result.AsReadOnly();
        }

        /// <summary>
        /// Finds a figure of a given type at the point. Collections are not searched.
        /// </summary>
        public IFigure HitTestNoCollections(Point point, Type figureType)
        {
            IFigure bestFoundSoFar = null;

            foreach (var item in this)
            {
                IFigure found = item.HitTest(point);
                
                if (found != null && figureType.IsAssignableFrom(found.GetType()))
                {
                    if (bestFoundSoFar == null || bestFoundSoFar.ZIndex <= found.ZIndex)
                    {
                        bestFoundSoFar = found;
                    }
                }
            }
            return bestFoundSoFar;
        }

        public virtual ReadOnlyCollection<IFigure> HitTestMany(Rect rect)
        {
            List<IFigure> result = new List<IFigure>();

            foreach (var item in this)
            {
                IFigure found = item.HitTest(rect);
                if (found != null)
                {
                    result.Add(found);
                }
            }

            if (result.Count > 0)
            {
                result.Sort((f1, f2) => f1.ZIndex.CompareTo(f2.ZIndex));
            }

            return result.AsReadOnly();
        }

        public virtual IFigure HitTest(Rect rect)
        {
            return null;
        }
        #endregion

        public void UpdateVisual()
        {
            this.ForEach(f => f.UpdateVisual());
        }

        public void Recalculate()
        {
            this.ForEach(f => f.Recalculate());
        }

        public virtual void OnAddingToCanvas(Canvas newContainer)
        {
            this.ForEach(f => f.OnAddingToCanvas(newContainer));
        }

        public virtual void OnRemovingFromCanvas(Canvas leavingContainer)
        {
            this.ForEach(f => f.OnRemovingFromCanvas(leavingContainer));
        }

        public int ZIndex { get; set; }

        private bool mVisible = true;
        public virtual bool Visible
        {
            get
            {
                return mVisible;
            }
            set
            {
                mVisible = value;
            }
        }

        private bool mPlaceHolder = false;
        public virtual bool PlaceHolder
        {
            get
            {
                return mPlaceHolder;
            }
            set
            {
                mPlaceHolder = value;
            }
        }

        public string Name { get; set; }
        public bool Selected { get; set; }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            DrawingSerializer.WriteFigureList(this, writer);
        }

        public void ReadXml(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFigure other)
        {
            return object.ReferenceEquals(this, other);
        }
    }
}
