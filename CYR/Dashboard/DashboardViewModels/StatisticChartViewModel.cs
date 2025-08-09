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

        // Category (months) on the bottom — give it a Key
        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Key = "MonthAxis",
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
            MinorGridlineStyle = LineStyle.None
        };

        var monthNames = new[] { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun",
                             "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };
        foreach (var m in monthNames) categoryAxis.Labels.Add(m);

        // Value axis on the left — give it a Key
        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Key = "ValueAxis",
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

        plotModel.Axes.Add(categoryAxis);
        plotModel.Axes.Add(valueAxis);

        var barSeries = new BarSeries
        {
            Title = "Umsatz",
            XAxisKey = "ValueAxis",
            YAxisKey = "MonthAxis",
            FillColor = OxyColor.FromRgb(37, 116, 174),
            StrokeColor = OxyColors.White,
            StrokeThickness = 1,
            BarWidth = 0.6,
            LabelPlacement = LabelPlacement.Outside,
            LabelFormatString = "€{0:N2}"
        };

        var maxItems = Math.Min(salesData.Count, monthNames.Length);
        for (int i = 0; i < maxItems; i++)
        {
            barSeries.Items.Add(new BarItem { Value = (double)salesData[i].Amount });
        }

        plotModel.Series.Add(barSeries);
        SalesPerMonth = plotModel;
    }

}