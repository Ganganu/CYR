using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;
    private readonly StatisticChartViewModel _statisticChartViewModel;
    private readonly DashboardUserViewModel _dashboardUserViewModel;

    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel, StatisticChartViewModel statisticChartViewModel, DashboardUserViewModel dashboardUserViewModel)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
        _statisticChartViewModel = statisticChartViewModel;
        _dashboardUserViewModel = dashboardUserViewModel;
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;
    public StatisticChartViewModel StatisticChartVM => _statisticChartViewModel;
    public DashboardUserViewModel DashboardUserVM => _dashboardUserViewModel;

}
