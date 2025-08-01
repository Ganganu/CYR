using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
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
        List<SalesPerMonth> salesPerMonths = await _statisticChartRepository.GetSalesPerMonth();
        var completeYearData = FillMissingMonths(salesPerMonths);
        CreateSalesPerMonthChart(completeYearData);
    }

    private List<SalesPerMonth> FillMissingMonths(List<SalesPerMonth> salesData)
    {
        var completeData = new List<SalesPerMonth>();

        // Create a dictionary for quick lookup of existing data
        var salesLookup = salesData.ToDictionary(s => s.Month, s => s.Amount);

        // Add all 12 months, using 0 for missing months
        for (int month = 1; month <= 12; month++)
        {
            var amount = salesLookup.ContainsKey(month) ? salesLookup[month] : 0m;
            completeData.Add(new SalesPerMonth(month, amount));
        }

        return completeData;
    }

    public PlotModel SalesPerMonth { get; private set; }

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
            Padding = new OxyThickness(60, 40, 20, 60),
            // Disable zooming and panning
            IsLegendVisible = true,
            PlotMargins = new OxyThickness(0),
            ClipTitle = false
        };
                

        // Create categorical axis for months with enhanced styling (disable zooming/panning)
        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Month",
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
            // Disable axis interactions
            IsPanEnabled = false,
            IsZoomEnabled = false
        };

        // Add all month names to the axis (all 12 months)
        var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        foreach (var monthName in monthNames)
        {
            categoryAxis.Labels.Add(monthName);
        }

        plotModel.Axes.Add(categoryAxis);

        // Create value axis with enhanced styling and formatting (disable zooming/panning)
        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Amount (€)",
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
            StringFormat = "€#,##0",  // Format numbers with euro symbol and thousands separator
            // Disable axis interactions
            IsPanEnabled = false,
            IsZoomEnabled = false
        };
        plotModel.Axes.Add(valueAxis);

        var lineSeries = new LineSeries
        {
            Title = "Sales Amount",
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
                    Text = $"€{salesData[i].Amount:N0}", // Format with thousands separator
                    TextPosition = new DataPoint(i, (double)salesData[i].Amount),
                    TextVerticalAlignment = VerticalAlignment.Bottom,
                    TextHorizontalAlignment = HorizontalAlignment.Center,
                    Stroke = OxyColors.Transparent,
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    TextColor = OxyColor.FromRgb(51, 51, 51),
                    Offset = new ScreenVector(0, -10) // Move label 10 pixels above the point
                };
                plotModel.Annotations.Add(annotation);
            }
        }

        plotModel.Series.Add(lineSeries);
        SalesPerMonth = plotModel;
    }
}