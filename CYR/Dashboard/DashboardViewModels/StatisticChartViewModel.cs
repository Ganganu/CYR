using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Series;

namespace CYR.Dashboard.DashboardViewModels;

public partial class StatisticChartViewModel : ObservableRecipient
{
    public StatisticChartViewModel()
    {
        MyModel = new PlotModel
        {
            Title = "Example",
            TitleFont = "Segoe UI",
            TitleFontSize = 18,
            Background = OxyColors.WhiteSmoke,
            TextColor = OxyColors.DimGray,
            PlotAreaBorderColor = OxyColors.Gray,
        };

        // Customize Axes
        MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
        {
            Position = OxyPlot.Axes.AxisPosition.Bottom,
            Title = "X Axis",
            TitleFont = "Segoe UI",
            TitleFontSize = 14,
            AxislineColor = OxyColors.Black,
            AxislineStyle = LineStyle.Solid,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColors.LightGray,
            MinorGridlineColor = OxyColors.Gainsboro
        });

        MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
        {
            Position = OxyPlot.Axes.AxisPosition.Left,
            Title = "Y Axis",
            TitleFont = "Segoe UI",
            TitleFontSize = 14,
            AxislineColor = OxyColors.Black,
            AxislineStyle = LineStyle.Solid,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColors.LightGray,
            MinorGridlineColor = OxyColors.Gainsboro
        });

        // Enhance FunctionSeries
        var series = new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")
        {
            Color = OxyColors.SteelBlue,
            StrokeThickness = 2,
            MarkerType = MarkerType.Circle,
            MarkerSize = 3,
            MarkerStroke = OxyColors.DarkBlue,
            MarkerFill = OxyColors.White
        };
        MyModel.Series.Add(series);
    }
    public PlotModel MyModel { get; private set; }
}
