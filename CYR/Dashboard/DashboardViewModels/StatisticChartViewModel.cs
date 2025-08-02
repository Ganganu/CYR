using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CYR.Dashboard.DashboardViewModels;

public partial class StatisticChartViewModel : ObservableRecipient
{
    private readonly StatisticChartRepository _statisticChartRepository;

    public StatisticChartViewModel(StatisticChartRepository statisticChartRepository)
    {
        _statisticChartRepository = statisticChartRepository;
        Initialize();
    }

    private async Task Initialize()
    {
        Years = [.. Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1).Reverse()];
        SelectedYear = Years.FirstOrDefault();
    }

    [ObservableProperty]
    private List<int> _years;

    [ObservableProperty]
    private int _selectedYear;

    [ObservableProperty]
    private PlotModel _salesPerMonth;

    async partial void OnSelectedYearChanged(int oldValue, int newValue)
    {
        List<SalesPerMonth> salesPerMonths = await _statisticChartRepository.GetSalesPerMonth(SelectedYear);
        var completeYearData = FillMissingMonths(salesPerMonths);
        CreateSalesPerMonthChart(completeYearData);
    }

    private List<SalesPerMonth> FillMissingMonths(List<SalesPerMonth> salesData)
    {
        var completeData = new List<SalesPerMonth>();

        var salesLookup = salesData.ToDictionary(s => s.Month, s => s.Amount);

        for (int month = 1; month <= 12; month++)
        {
            var amount = salesLookup.ContainsKey(month) ? salesLookup[month] : 0m;
            completeData.Add(new SalesPerMonth(month, amount));
        }

        return completeData;
    }

    private void CreateSalesPerMonthChart(List<SalesPerMonth> salesData)
    {
        var plotModel = new PlotModel
        {
            Title = "Umsatz Monatlich",
            TitleFont = "Segoe UI",
            TitleFontSize = 18,
            TitleFontWeight = FontWeights.Bold,
            TitleColor = OxyColor.FromRgb(51, 51, 51),
            Background = OxyColor.FromRgb(248, 249, 250),
            PlotAreaBackground = OxyColors.White,
            PlotAreaBorderColor = OxyColor.FromRgb(200, 200, 200),
            PlotAreaBorderThickness = new OxyThickness(1),
            Padding = new OxyThickness(80, 0, 20, 60),
            IsLegendVisible = true,
            PlotMargins = new OxyThickness(0),
            ClipTitle = false
        };

        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Monat",
            TitleFont = "Segoe UI",
            TitleFontSize = 14,
            TitleFontWeight = FontWeights.Bold,
            TitleColor = OxyColor.FromRgb(102, 102, 102),
            FontSize = 12,
            TextColor = OxyColor.FromRgb(102, 102, 102),
            TickStyle = TickStyle.Outside,
            AxislineStyle = LineStyle.Solid,
            AxislineColor = OxyColor.FromRgb(200, 200, 200),
            AxislineThickness = 1,
            MajorGridlineStyle = LineStyle.None,
            MinorGridlineStyle = LineStyle.None,
            //IsPanEnabled = false,
            //IsZoomEnabled = false
        };

        var monthNames = new[] { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun",
                                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        foreach (var monthName in monthNames)
        {
            categoryAxis.Labels.Add(monthName);
        }

        plotModel.Axes.Add(categoryAxis);

        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Umsatz (€)",
            TitleFont = "Segoe UI",
            TitleFontSize = 14,
            TitleFontWeight = FontWeights.Bold,
            TitleColor = OxyColor.FromRgb(102, 102, 102),
            FontSize = 12,
            TextColor = OxyColor.FromRgb(102, 102, 102),
            MinimumPadding = 0,
            MaximumPadding = 0.1,
            AbsoluteMinimum = 0,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(230, 230, 230),
            MajorGridlineThickness = 1,
            MinorGridlineStyle = LineStyle.Dot,
            MinorGridlineColor = OxyColor.FromRgb(245, 245, 245),
            TickStyle = TickStyle.Outside,
            AxislineStyle = LineStyle.Solid,
            AxislineColor = OxyColor.FromRgb(200, 200, 200),
            AxislineThickness = 1,
            StringFormat = "€#,##0",
            IsPanEnabled = false,
            IsZoomEnabled = false
        };
        plotModel.Axes.Add(valueAxis);

        var lineSeries = new LineSeries
        {
            Title = "Umsatz",
            Color = OxyColor.FromRgb(37, 116, 174), // Modern blue color
            StrokeThickness = 3,
            LineStyle = LineStyle.Solid,
            MarkerType = MarkerType.Circle,
            MarkerSize = 8,
            MarkerFill = OxyColor.FromRgb(37, 116, 174),
            MarkerStroke = OxyColors.White,
            MarkerStrokeThickness = 2
        };

        for (int i = 0; i < salesData.Count; i++)
        {
            var dataPoint = new DataPoint(i, (double)salesData[i].Amount);
            lineSeries.Points.Add(dataPoint);

            if (salesData[i].Amount > 0)
            {
                var annotation = new TextAnnotation
                {
                    Text = $"€{salesData[i].Amount:N2}",
                    TextPosition = new DataPoint(i, (double)salesData[i].Amount),
                    TextVerticalAlignment = VerticalAlignment.Bottom,
                    TextHorizontalAlignment = HorizontalAlignment.Center,
                    Stroke = OxyColors.Transparent,
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    TextColor = OxyColor.FromRgb(51, 51, 51),
                    Offset = new ScreenVector(0, -10)
                };
                plotModel.Annotations.Add(annotation);
            }
        }

        //var legend = new Legend
        //{
        //    LegendTitle = "Legende",
        //    LegendPlacement = LegendPlacement.Outside,
        //    LegendPosition = LegendPosition.BottomRight,
        //    LegendOrientation = LegendOrientation.Vertical,
        //    LegendFont = "Segoe UI",
        //    LegendFontSize = 12,
        //    LegendTitleFontSize = 14,
        //    LegendPadding = 25,
        //    LegendTextColor = OxyColor.FromRgb(102, 102, 102),
        //    LegendBorderThickness = 0,
        //};

        //plotModel.Legends.Add(legend);
        plotModel.Series.Add(lineSeries);
        SalesPerMonth = plotModel; // This will automatically trigger PropertyChanged
    }
}