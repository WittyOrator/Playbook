﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

using Webb.Playbook.Geometry.Shapes.Creators;
using Webb.Playbook.Geometry.Lists;

namespace Webb.Playbook.Geometry.Shapes
{
    public class PBLineCreator : FigureCreator
    {
        protected override Webb.Playbook.Geometry.Lists.DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IFigure CreateFigure()
        {
            return Factory.CreateLine(Drawing, FoundDependencies);
        }

        public override string Name
        {
            get { return "Line"; }
        }

        /// <summary>
        /// Assumes coordinates are logical already
        /// </summary>
        /// <param name="coordinates">Logical coordinates of the click point</param>
        protected override void Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                        
                        Drawing.Figures.UpdateVisual();
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
    }

    public class ClassicPBArrowLineCreator : ClassicFigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IFigure CreateFigure()
        {
            return Factory.CreateArrowLine(Drawing, FoundDependencies);
        }

        public override string Name
        {
            get { return "ClassicArrowLine"; }
        }

        public override string HintText
        {
            get
            {
                return "Click (and release) twice to connect two points with a segment.";
            }
        }

        public override Color ButtonColor
        {
            get
            {
                return Color.FromArgb(255, 205, 255, 128);
            }
        }

        public override System.Windows.Controls.Panel CreateIcon()
        {
            return null;
        }
    }

    public class ClassicPBLineCreator : ClassicFigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IFigure CreateFigure()
        {
            return Factory.CreateLine(Drawing, FoundDependencies);
        }

        public override string Name
        {
            get { return "ClassicLine"; }
        }

        public override string HintText
        {
            get
            {
                return "Click (and release) twice to connect two points with a segment.";
            }
        }

        public override Color ButtonColor
        {
            get
            {
                return Color.FromArgb(255, 205, 255, 128);
            }
        }

        public override System.Windows.Controls.Panel CreateIcon()
        {
            return null;
        }
    }

    public class ClassicPBBlockLineCreator : ClassicFigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IFigure CreateFigure()
        {
            return Factory.CreatePassBlockAreaLine(Drawing, FoundDependencies);
        }

        public override string Name
        {
            get { return "ClassicBlockLine"; }
        }

        public override string HintText
        {
            get
            {
                return "Click (and release) twice to connect two points with a segment.";
            }
        }

        public override Color ButtonColor
        {
            get
            {
                return Color.FromArgb(255, 205, 255, 128);
            }
        }

        public override System.Windows.Controls.Panel CreateIcon()
        {
            return null;
        }
    }
}
