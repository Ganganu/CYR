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
    private IPlotController _plotController;

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
        if (salesData is null) return null;
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
        if (salesData is null) return;

        var plotModel = new PlotModel
        {
            Title = "Monatlicher Umsatz",
            TitleFont = "Segoe UI",
            TitleFontSize = 20,
            TitleFontWeight = FontWeights.Normal,
            TitleColor = OxyColor.FromRgb(30, 41, 59), 

            Background = OxyColors.White,
            PlotAreaBackground = OxyColors.White,
            PlotAreaBorderColor = OxyColors.Transparent, 
            PlotAreaBorderThickness = new OxyThickness(0),

            Padding = new OxyThickness(60, 40, 40, 60),
            PlotMargins = new OxyThickness(0),

            IsLegendVisible = false,
            ClipTitle = false
        };

        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Key = "MonthAxis",

            // Clean axis styling
            TitleFont = "Segoe UI",
            TitleFontSize = 0,
            FontSize = 11,
            TextColor = OxyColor.FromRgb(100, 116, 139),

            TickStyle = TickStyle.None,
            AxislineStyle = LineStyle.None,
            AxislineThickness = 0,

            MajorGridlineStyle = LineStyle.None,
            MinorGridlineStyle = LineStyle.None,

            MajorTickSize = 0,
            MinorTickSize = 0
        };

        var monthNames = new[] { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun",
                             "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };
        foreach (var m in monthNames) categoryAxis.Labels.Add(m);

        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Key = "ValueAxis",

            TitleFont = "Segoe UI",
            TitleFontSize = 0,
            FontSize = 11,
            TextColor = OxyColor.FromRgb(100, 116, 139),

            MinimumPadding = 0.05,
            MaximumPadding = 0.15,
            AbsoluteMinimum = 0,

            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(241, 245, 249),
            MajorGridlineThickness = 1,
            MinorGridlineStyle = LineStyle.None,

            TickStyle = TickStyle.None,
            AxislineStyle = LineStyle.None,
            AxislineThickness = 0,

            StringFormat = "€#,##0",
            IsPanEnabled = false,
            IsZoomEnabled = false
        };

        plotModel.Axes.Add(categoryAxis);
        plotModel.Axes.Add(valueAxis);

        var barSeries = new BarSeries
        {
            XAxisKey = "ValueAxis",
            YAxisKey = "MonthAxis",

            FillColor = OxyColor.FromRgb(59, 130, 246),
            StrokeColor = OxyColors.Transparent,
            StrokeThickness = 0,
            TrackerFormatString = "📅 {1}\n💰 Umsatz: €{2:N0}",

            BarWidth = 0.2,
        };

        var maxValue = (double)salesData.Max(s => s.Amount); // Convert to double
        var currentMonthIndex = DateTime.Now.Month - 1;

        var maxItems = Math.Min(salesData.Count, monthNames.Length);
        for (int i = 0; i < maxItems; i++)
        {
            var value = (double)salesData[i].Amount;
            var item = new BarItem { Value = value };

            if (i == currentMonthIndex || salesData[i].Amount == salesData.Max(s => s.Amount))
            {
                item.Color = OxyColor.FromRgb(245, 158, 11);
            }
            else if (value == 0)
            {
                item.Color = OxyColor.FromRgb(226, 232, 240);
            }
            else
            {
                item.Color = OxyColor.FromRgb(59, 130, 246);
            }

            barSeries.Items.Add(item);
        }

        plotModel.Series.Add(barSeries);

        if (currentMonthIndex < maxItems && salesData[currentMonthIndex].Amount > 0)
        {
            var currentValue = (double)salesData[currentMonthIndex].Amount;
            var maxValueDouble = (double)salesData.Max(s => s.Amount);
            var annotation = new TextAnnotation
            {
                Text = $"€{currentValue:N0}",
                TextPosition = new DataPoint(currentValue + (maxValueDouble * 0.02), currentMonthIndex),
                TextColor = OxyColor.FromRgb(100, 116, 139),
                Font = "Segoe UI",
                FontSize = 10,
                FontWeight = FontWeights.Normal,
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Middle,
                Background = OxyColors.Transparent,
                Stroke = OxyColors.Transparent
            };
            plotModel.Annotations.Add(annotation);
        }

        var controller = new PlotController();
        controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);
        PlotController = controller;

        SalesPerMonth = plotModel;
    }
}