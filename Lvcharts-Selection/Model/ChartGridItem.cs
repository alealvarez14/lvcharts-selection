using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lvcharts_Selection.Model
{
    public class ChartGridItem
    {
        public CartesianChart Chart { get; set; }
        public string Title { get; set; }
        public string ChartName { get; set; }
        public SeriesCollection DataSeries { get; set; }
        public Canvas DragSelectionCanvas { get; set; }
        public Border DragSelectionBorder { get; set; }
    }
}
