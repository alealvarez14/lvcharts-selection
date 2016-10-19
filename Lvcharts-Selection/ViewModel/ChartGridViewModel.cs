using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LiveCharts;
using LiveCharts.Wpf;
using Lvcharts_Selection.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Lvcharts_Selection.ViewModel
{
    public class ChartGridViewModel : ViewModelBase
    {
        private ObservableCollection<double[]> seriesList;
        private int chartCounter;
        private List<ChartPoint> selectedPoints;
        private List<double> points;
        public ICommand AddNewRowCommand { get; private set; }
        public ICommand LoadSelectedPoints { get; private set; }
        

        public ChartGridViewModel()
        {
            chartCounter = 0;
            seriesList = new ObservableCollection<double[]>();
            SetDummyData();
            Data = new ObservableCollection<ChartGridItem>();
            DrawVars();
            AddNewRowCommand = new RelayCommand(() => AddRow());
            LoadSelectedPoints = new RelayCommand<List<IEnumerable<ChartPoint>>>((selectedPts) => LoadPoints(selectedPts));

        }

        public ObservableCollection<ChartGridItem> Data { get; set; }
        public ObservableCollection<double[]> SeriesList { get { return seriesList; } }

        public List<ChartPoint> SelectedPoints
        {
            get { return selectedPoints; }
            set { selectedPoints = value; RaisePropertyChanged("SelectedPoints"); }
        }

        public void LoadPoints(List<IEnumerable<ChartPoint>> selectedPts)
        {
            selectedPoints = new List<ChartPoint>();

            points = new List<double>();

            foreach (var enumerable in selectedPts)
            {
                foreach (var serie in enumerable)
                {
                    selectedPoints.Add(serie);
                    points.Add(Convert.ToDouble(serie.Instance));
                }
            }
            
            RaisePropertyChanged("SelectedPoints");
        }

        private void SetDummyData()
        {
            double[] aux = new double[100];
            for (int i = 0; i < 100; i++)
            {
                aux[i] = i;
            }
            seriesList.Add(aux);
        }

        public void DrawVars()
        {
            foreach (var item in SeriesList)
            {
                ChartGridItem c = new ChartGridItem();
                c.Chart = new CartesianChart();
                c.Chart.Name = "Chart" + chartCounter;
                c.Title = "Series " + chartCounter;
                c.DataSeries = GetDataSeries();
                c.DragSelectionBorder = new System.Windows.Controls.Border();
                c.DragSelectionCanvas = new System.Windows.Controls.Canvas();

                Data.Add(c);
                chartCounter++;
            }
        }

        public void AddRow()
        {
            ChartGridItem c = new ChartGridItem();
            c.Chart = new CartesianChart();
            c.Chart.Name = "Chart" + SeriesList.Count;
            c.Title = "Series " + chartCounter;
            c.DataSeries = GetDataSeries();
            c.DragSelectionBorder = new System.Windows.Controls.Border();
            c.DragSelectionCanvas = new System.Windows.Controls.Canvas();

            Data.Add(c);
        }

        private SeriesCollection GetDataSeries()
        {
            SeriesCollection series = new SeriesCollection();

            foreach (var item in seriesList)
            {
                LineSeries lineSeries = new LineSeries();
                var chartValues = new ChartValues<double>();

                for (int i = 0; i < item.Length; i++)
                {
                    chartValues.Add(item[i]);
                }

                lineSeries.Title = "Series>" + Data.Count;
                lineSeries.Fill = Brushes.Transparent;
                lineSeries.Values = chartValues;
                series.Add(lineSeries);
            }

            return series;
        }
    }
}
