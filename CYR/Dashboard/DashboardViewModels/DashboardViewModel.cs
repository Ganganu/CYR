using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;
    private readonly StatisticChartViewModel _statisticChartViewModel;

    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel, StatisticChartViewModel statisticChartViewModel)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
        _statisticChartViewModel = statisticChartViewModel;
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;
    public StatisticChartViewModel StatisticChartVM => _statisticChartViewModel;

}
