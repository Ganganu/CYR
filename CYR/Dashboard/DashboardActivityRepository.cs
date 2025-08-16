using System.Collections;
using CYR.Core;

namespace CYR.Dashboard;

public class DashboardActivityRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public DashboardActivityRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    //public async IEnumerable<Task<ActivityModel>> GetActivities()
    //{
    //    string query = @"select h.logging_type, h.user_id, h.invoice_id, h.client_id
    //                    inner join ";
    //} 
}
