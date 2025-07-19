namespace CYR.UnitOfMeasure;

public interface IUnitOfMeasureRepository
{
    Task DeleteAsync(UnitOfMeasureModel unitOfMeasure);
    Task<IEnumerable<UnitOfMeasureModel>> GetAllAsync();
    Task<IEnumerable<UnitOfMeasureModel>> GetByIdAsync(int id);
    Task InsertAsync(UnitOfMeasureModel unitOfMeasure);
    Task UpdateAsync(UnitOfMeasureModel unitOfMeasure);
}
