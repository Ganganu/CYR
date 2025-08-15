using System.Data.Common;
using CYR.Core;
using CYR.OrderItems;
using CYR.User;

namespace CYR.UnitOfMeasure;

public class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;

    public UnitOfMeasureRepository(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
    }
    public async Task<bool> DeleteAsync(UnitOfMeasureModel unitOfMeasure)
    {
        bool succes = false;
        string query = "DELETE FROM uom WHERE id = @id and user_id = @user_id";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "@id", unitOfMeasure.Id},
            { "user_id", _userContext.CurrentUser.Id}
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        succes = affectedRows > 0;
        return succes;
    }

    public async Task<IEnumerable<UnitOfMeasureModel>> GetAllAsync()
    {
        List<UnitOfMeasureModel> orderItems = new List<UnitOfMeasureModel>();
        UnitOfMeasureModel unitOfMeasure;
        string query = $"SELECT * FROM uom where user_id = {_userContext.CurrentUser.Id}";

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

    public async Task InsertAsync(UnitOfMeasureModel unitOfMeasure)
    {
        string query = "INSERT INTO uom (name,description,user_id) VALUES (@name,@description,@user_id)";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "@name", unitOfMeasure.Name },
            { "@description", unitOfMeasure.Description },
            { "@user_id", _userContext.CurrentUser.Id }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
    }

    public async Task<bool> UpdateAsync(UnitOfMeasureModel unitOfMeasure)
    {
        string query = "update uom  set name = @name, description = @description where id = @id and user_id = @user_id";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "@id", unitOfMeasure.Id },
            { "@name", unitOfMeasure.Name },
            { "@description", unitOfMeasure.Description },
            { "@user_id", _userContext.CurrentUser.Id }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        return affectedRows > 0;
    }
}
