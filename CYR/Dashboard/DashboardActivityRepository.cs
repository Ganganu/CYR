using System.Data.Common;
using CYR.Core;

namespace CYR.Dashboard;

public class DashboardActivityRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public DashboardActivityRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<ActivityModel>> GetUnifiedActivityFeedAsync(int userId, int limit = 50)
    {
        string query = @"
        SELECT 
                datetime(h.timestamp, 'localtime') as timestamp,
                CASE h.logging_type 
                    WHEN 1000 THEN 'UserCreated'
                    WHEN 1001 THEN 'UserDeleted' 
                    WHEN 1002 THEN 'UserUpdated'
                    WHEN 3000 THEN 'ClientCreated'
                    WHEN 3001 THEN 'ClientDeleted'
					WHEN 3002 THEN 'ClientUpdated'
					WHEN 4000 THEN 'InvoiceCreated'
					WHEN 4001 THEN 'InvoiceDeleted'
					WHEN 4002 THEN 'InvoiceUpdated'
					WHEN 5000 THEN 'OrderItemCreated'
					WHEN 5001 THEN 'OrderItemDeleted'
					WHEN 5002 THEN 'OrderItemUpdated'
                    ELSE 'Other'
                END as activity_type,
                COALESCE(h.message, 'Aktivität protokolliert') as title,
                COALESCE(h.message, 'Eine Aktivität wurde durchgeführt') as description,
                cast(COALESCE(h.invoice_id, CAST(h.client_id as TEXT), CAST(h.address_id as TEXT)) as TEXT) as entity_id,
                k.Name as client_name,
                cast(r.Bruttobetrag as TEXT) as amount,
                r.Status as status
            FROM his h
            LEFT JOIN Kunden k ON h.client_id = k.Kundennummer AND h.user_id = k.user_id
            LEFT JOIN Rechnungen r ON h.invoice_id = r.Rechnungsnummer AND h.user_id = r.user_id
            order by timestamp desc
		LIMIT @limit;";

        var parameters = new Dictionary<string, object>
        {
            ["@userId"] = userId,
            ["@limit"] = limit
        };

        List<ActivityModel> activityModels = [];

        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, parameters))
        {
            while (await reader.ReadAsync())
            {
                DateTime? timestamp = reader.IsDBNull(0) ? null : reader.GetDateTime(0);
                string? activityType = reader.IsDBNull(1) ? null : reader.GetString(1);
                string? title = reader.IsDBNull(2) ? null : reader.GetString(2);
                string? description = reader.IsDBNull(3) ? null : reader.GetString(3);
                string? entityId = reader.IsDBNull(4) ? null : reader.GetString(4);
                string? clientName = reader.IsDBNull(5) ? null : reader.GetString(5);
                string? amount = reader.IsDBNull(6) ? null : reader.GetString(6);
                string? status = reader.IsDBNull(7) ? null : reader.GetString(7);

                activityModels.Add(new ActivityModel(
                    timestamp,
                    activityType,
                    title,
                    description,
                    entityId,
                    clientName,
                    amount,
                    status
                ));
            }
        }

        return activityModels;
    }
}