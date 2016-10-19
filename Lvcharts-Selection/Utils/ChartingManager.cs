using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lvcharts_Selection.Utils
{
    public static class ChartingManager
    {
        /// <summary>
        /// Set to 'true' when the left mouse-button is down.
        /// </summary>
        private static bool isLeftMouseButtonDownOnWindow = false;

        /// <summary>
        /// Set to 'true' when dragging the 'selection rectangle'.
        /// Dragging of the selection rectangle only starts when the left mouse-button is held down and the mouse-cursor
        /// is moved more than a threshold distance.
        /// </summary>
        private static bool isDraggingSelectionRect = false;

        /// <summary>
        /// Records the location of the mouse (relative to the window) when the left-mouse button has pressed down.
        /// </summary>
        private static Point origMouseDownPoint;

        private static Point destinoMouseUpPoint;

        /// <summary>
        /// The threshold distance the mouse-cursor must move before drag-selection begins.
        /// </summary>
        private static readonly double DragThreshold = 2;

        /// <summary>
        /// Set to 'true' when the left mouse-button is held down on a rectangle.
        /// </summary>
        private static bool isLeftMouseDownOnRectangle = false;

        /// <summary>
        /// Set to 'true' when the left mouse-button and control are held down on a rectangle.
        /// </summary>
        private static bool isLeftMouseAndControlDownOnRectangle = false;

        /// <summary>
        /// Set to 'true' when dragging a rectangle.
        /// </summary>
        private static bool isDraggingRectangle = false;

        public static void Mouse_Down(bool buttonDownOnWindow, Point p, double height)
        {
            isLeftMouseButtonDownOnWindow = buttonDownOnWindow;
            origMouseDownPoint = p;
            origMouseDownPoint.Y = height;
        }

        public static void SetFinalPoint(Point p)
        {
            destinoMouseUpPoint = p;
        }

        public static bool GetIsDragging()
        {
            return isDraggingSelectionRect;
        }

        public static void SetIsDragging(bool isDragging)
        {
            isDraggingSelectionRect = isDragging;
        }

        public static void SetIsLeftMouseButtonDownOnWindow(bool isLeftDown)
        {
            isLeftMouseButtonDownOnWindow = isLeftDown;
        }

        public static bool GetIsLeftMouseButtonDownOnWindow()
        {
            return isLeftMouseButtonDownOnWindow;
        }

        public static Vector CalculateDelta(Point currentPoint)
        {
            return (currentPoint - origMouseDownPoint);
        }

        public static void UpdateDragSelectionRect(Point pt1, Point pt2,
                                                     out double x, out double y,
                                                     out double width, out double height)
        {


            //
            // Determine x,y,width and height of the rect inverting the points if necessary.
            // 
            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }
        }

        public static Point GetCurrent()
        {
            return origMouseDownPoint;
        }

        public static List<IEnumerable<ChartPoint>> SelectedPoints(CartesianChart chart, Point p, out int axisBegin, out int axisEnd)
        {
            var beginning = new Point(ChartFunctions.FromPlotArea(origMouseDownPoint.X, AxisOrientation.X, chart.Model), ChartFunctions.FromPlotArea(origMouseDownPoint.Y, AxisOrientation.Y, chart.Model));
            var final = new Point(ChartFunctions.FromPlotArea(p.X, AxisOrientation.X, chart.Model), ChartFunctions.FromPlotArea(p.Y, AxisOrientation.Y, chart.Model));

            List<IEnumerable<ChartPoint>> res = new List<IEnumerable<ChartPoint>>();
            var counter = 0;
            while (counter < chart.Series.Count)
            {
                var pts = chart.Series[counter].Values.GetPoints(chart.Series[0]).Where(pt => pt.X >= beginning.X && pt.X <= final.X);
                res.Add(pts);
                counter++;
            }

            try
            {
                axisBegin = chart.Series[0].Values.GetPoints(chart.Series[0]).Where(pt => pt.X >= beginning.X && pt.X <= final.X).First().Key;
                axisEnd = chart.Series[0].Values.GetPoints(chart.Series[0]).Where(pt => pt.X >= beginning.X && pt.X <= final.X).Last().Key;
            }
            catch (Exception e)
            {
                axisBegin = -1;
                axisEnd = -1;
            }
            return res;
        }
    }
}
