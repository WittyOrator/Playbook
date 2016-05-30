using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Webb.Playbook.Geometry
{
    public class CoordinateSystem
    {
        public static double LogicalPlaygroundWidth = 53.5;
        public static double LogicalPlaygroundHeight = 120;
        public static double PhysicalPlaygroundWidth = 300;
        public static Point LogicalMidPos = new Point();
        public static Point ViewMidPos = new Point();
        public static double LogicalPlayerSize = 1;

        public CoordinateSystem()
        {
            mOrigin = new Point(0, 0);
            unitLength = 20;
        }

        public CoordinateSystem(Point ptOrigin, double lUnit)
        {
            mOrigin = new Point(ptOrigin.X, ptOrigin.Y);

            unitLength = lUnit;
        }

        private Point mOrigin;
        /// <summary>
        /// Origin is in physical coordinates (for 800x600 it will usually be (400;300))
        /// </summary>
        public Point Origin
        {
            get
            {
                return mOrigin;
            }
            set
            {
                mOrigin = value;
            }
        }

        private double unitLength;
        /// <summary>
        /// How many pixels are in a logical unit?
        /// </summary>
        public double UnitLength
        {
            get
            {
                return unitLength;
            }
            set
            {
                if (value < 2 || value > 1000)
                {
                    return;
                }
                unitLength = value;
            }
        }

        private double mCursorTolerance = 0.1;
        public double CursorTolerance
        {
            get { return this.mCursorTolerance; }
            set { this.mCursorTolerance = value; }
        }

        public virtual Point ToLogical(Point physicalPoint)
        {
            return new Point(
                 (physicalPoint.X - Origin.X) / UnitLength,
                -(physicalPoint.Y - Origin.Y) / UnitLength);
        }

        public IEnumerable<Point> ToLogical(IEnumerable<Point> physicalPoints)
        {
            return physicalPoints.Select(p => ToLogical(p));
        }

        public PointPair ToLogical(PointPair pointPair)
        {
            var result = new PointPair(ToLogical(pointPair.P1), ToLogical(pointPair.P2));
            if (result.P1.X > result.P2.X)
            {
                result = result.Reverse;
            }
            if (result.P1.Y > result.P2.Y)
            {
                var temp = result.P2.Y;
                result.P2.Y = result.P1.Y;
                result.P1.Y = temp;
            }
            return result;
        }

        public virtual double ToLogical(double length)
        {
            return length / UnitLength;
        }

        public virtual Point ToPhysical(Point logicalPoint)
        {
            return new Point(
                Origin.X + logicalPoint.X * UnitLength,
                Origin.Y - logicalPoint.Y * UnitLength);
        }

        public PointPair ToPhysical(PointPair logicalPointPair)
        {
            return new PointPair(ToPhysical(logicalPointPair.P1), ToPhysical(logicalPointPair.P2));
        }

        public IEnumerable<Point> ToPhysical(IEnumerable<Point> logicalPoints)
        {
            return logicalPoints.Select(p => ToPhysical(p));
        }

        public virtual double ToPhysical(double length)
        {
            return length * UnitLength;
        }
    }
}
