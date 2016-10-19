using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lvcharts_Selection.Utils;
using LiveCharts.Wpf;
using LiveCharts;
using Lvcharts_Selection.ViewModel;

namespace Lvcharts_Selection.Views
{
    /// <summary>
    /// Interaction logic for ChartGrid.xaml
    /// </summary>
    public partial class ChartGrid : UserControl
    {
        public ChartGrid()
        {
            InitializeComponent();
        }

        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UpdateDragSelectionRect(ChartingManager.GetCurrent(), e.GetPosition(this), sender);
                var b = sender as Grid;

                //
                //  Clear selection immediately when starting drag selection.
                //
                listBox.SelectedItems.Clear();
                CartesianChart c = b.Children[1] as CartesianChart;
                ChartingManager.Mouse_Down(true, e.GetPosition(this), c.ActualWidth);
                e.Handled = true;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                bool wasDragSelectionApplied = false;
                ChartingManager.SetFinalPoint(e.GetPosition(this));

                if (ChartingManager.GetIsDragging())
                {
                    //
                    // Drag selection has ended, apply the 'selection rectangle'.
                    //
                    ChartingManager.SetIsDragging(false);
                    e.Handled = true;
                    wasDragSelectionApplied = true;
                    ReportMousePosition(sender);
                }

                if (ChartingManager.GetIsLeftMouseButtonDownOnWindow())
                {
                    ChartingManager.SetIsLeftMouseButtonDownOnWindow(false);
                    //    this.ReleaseMouseCapture();
                    e.Handled = true;
                }

                if (!wasDragSelectionApplied)
                {
                    //
                    // A click and release in empty space clears the selection.
                    //
                    listBox.SelectedItems.Clear();
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (ChartingManager.GetIsDragging())
            {
                //
                // Drag selection is in progress.
                //
                Point curMouseDownPoint = e.GetPosition(this);
                curMouseDownPoint.Y = 0;
                UpdateDragSelectionRect(ChartingManager.GetCurrent(), curMouseDownPoint, sender);

                e.Handled = true;
            }
            else if (ChartingManager.GetIsLeftMouseButtonDownOnWindow())
            {
                //
                // The user is left-dragging the mouse,
                // but don't initiate drag selection until
                // they have dragged past the threshold value.
                //
                Point curMouseDownPoint = e.GetPosition(this);
                var dragDelta = ChartingManager.CalculateDelta(curMouseDownPoint);
                double dragDistance = Math.Abs(dragDelta.Length);

                if (dragDistance > 2)
                {
                    //
                    // When the mouse has been dragged more than the threshold value commence drag selection.
                    //
                    ChartingManager.SetIsDragging(true);
                    InitDragSelectionRect(ChartingManager.GetCurrent(), curMouseDownPoint, sender);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Initialize the rectangle used for drag selection.
        /// </summary>
        private void InitDragSelectionRect(Point pt1, Point pt2, object sender)
        {
            UpdateDragSelectionRect(pt1, pt2, sender);
            var b = sender as Grid;
            Canvas dragSelectionCanvas = b.Children[0] as Canvas;
            dragSelectionCanvas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Update the position and size of the rectangle used for drag selection.
        /// </summary>
        private void UpdateDragSelectionRect(Point pt1, Point pt2, object sender)
        {
            double x, y, width, height;

            var b = sender as Grid;
            Canvas c = b.Children[0] as Canvas;
            Border dragSelectionBorder = c.Children[1] as Border;
            ChartingManager.UpdateDragSelectionRect(pt1, pt2, out x, out y, out width, out height);
            Canvas.SetLeft(dragSelectionBorder, x);
            Canvas.SetTop(dragSelectionBorder, y);
            dragSelectionBorder.Width = width;
            dragSelectionBorder.Height = height;
        }


        public void ReportMousePosition(object sender)
        {
            Point p = Mouse.GetPosition(null);
            Point origMouseDownPoint = ChartingManager.GetCurrent();
            var b = sender as Grid;
            CartesianChart chart = b.Children[1] as CartesianChart;
            int axisBegin;
            int axisEnd;
            List<IEnumerable<ChartPoint>> selected = ChartingManager.SelectedPoints(chart, p, out axisBegin, out axisEnd);

            if (axisBegin != -1 && axisEnd != -1)
            {
                var vm = DataContext as ChartGridViewModel;
                vm.LoadSelectedPoints.Execute(selected);
            }
        }
    }
}
