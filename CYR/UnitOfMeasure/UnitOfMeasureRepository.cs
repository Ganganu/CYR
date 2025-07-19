using CYR.Core;
using System.Data.Common;

namespace CYR.UnitOfMeasure;

public class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly IDatabaseConnection _databaseConnection;

    public UnitOfMeasureRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }
    public Task DeleteAsync(UnitOfMeasureModel unitOfMeasure)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UnitOfMeasureModel>> GetAllAsync()
    {
        List<UnitOfMeasureModel> orderItems = new List<UnitOfMeasureModel>();
        UnitOfMeasureModel unitOfMeasure;
        string query = "SELECT * FROM uom";

        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
        {
            while (await reader.ReadAsync())
            {
                unitOfMeasure = new UnitOfMeasureModel();
                unitOfMeasure.Id = Convert.ToInt32(reader["id"]);
                unitOfMeasure.Name = reader["name"].ToString();
                unitOfMeasure.Description = reader["description"].ToString();
                orderItems.Add(unitOfMeasure);
            }
            return orderItems;
        }
    }

    public Task<IEnumerable<UnitOfMeasureModel>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(UnitOfMeasureModel unitOfMeasure)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UnitOfMeasureModel unitOfMeasure)
    {
        throw new NotImplementedException();
    }
}
