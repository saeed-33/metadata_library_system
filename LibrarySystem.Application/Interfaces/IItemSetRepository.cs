using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Interfaces;

public interface IItemSetRepository
{
    Task<ItemSet?> GetByIdAsync(int id);
    Task<IEnumerable<ItemSet>> GetAllAsync();
    Task AddAsync(ItemSet itemSet);
    Task<bool> UpdateAsync(ItemSet itemSet);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddItemAsync(int itemSetId, int itemId);
    Task<bool> RemoveItemAsync(int itemSetId, int itemId);
}
