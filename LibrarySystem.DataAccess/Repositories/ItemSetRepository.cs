using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.DataAccess.Repositories;

public class ItemSetRepository : IItemSetRepository, ISyncGeneratedKeys
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly List<(ItemSet Domain, ItemSetModel Persistence)> _pendingAdds = new();

    public ItemSetRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ItemSet?> GetByIdAsync(int id)
    {
        var model = await ItemSetQuery().FirstOrDefaultAsync(itemSet => itemSet.Id == id);
        return model == null ? null : _mapper.Map<ItemSet>(model);
    }

    public async Task<IEnumerable<ItemSet>> GetAllAsync()
    {
        var models = await ItemSetQuery().ToListAsync();
        return _mapper.Map<IEnumerable<ItemSet>>(models);
    }

    public async Task AddAsync(ItemSet itemSet)
    {
        var model = _mapper.Map<ItemSetModel>(itemSet);
        await _context.ItemSets.AddAsync(model);
        _pendingAdds.Add((itemSet, model));
    }

    public async Task<bool> UpdateAsync(ItemSet itemSet)
    {
        var model = await _context.ItemSets.FirstOrDefaultAsync(existing => existing.Id == itemSet.Id);
        if (model == null) return false;

        model.Title = itemSet.Title;
        model.Description = itemSet.Description;
        model.IsPublic = itemSet.IsPublic;
        model.OwnerId = itemSet.OwnerId;

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var model = await _context.ItemSets.FirstOrDefaultAsync(itemSet => itemSet.Id == id);
        if (model == null) return false;

        _context.ItemSets.Remove(model);
        return true;
    }

    public async Task<bool> AddItemAsync(int itemSetId, int itemId)
    {
        var itemSet = await _context.ItemSets
            .Include(existing => existing.Items)
            .FirstOrDefaultAsync(existing => existing.Id == itemSetId);

        var item = await _context.Items.FirstOrDefaultAsync(existing => existing.Id == itemId);
        if (itemSet == null || item == null) return false;

        if (itemSet.Items.All(existing => existing.Id != itemId))
        {
            itemSet.Items.Add(item);
        }

        return true;
    }

    public async Task<bool> RemoveItemAsync(int itemSetId, int itemId)
    {
        var itemSet = await _context.ItemSets
            .Include(existing => existing.Items)
            .FirstOrDefaultAsync(existing => existing.Id == itemSetId);

        if (itemSet == null) return false;

        var item = itemSet.Items.FirstOrDefault(existing => existing.Id == itemId);
        if (item == null) return false;

        itemSet.Items.Remove(item);
        return true;
    }

    public void SyncGeneratedKeys()
    {
        foreach (var (domain, persistence) in _pendingAdds)
        {
            _mapper.Map(persistence, domain);
        }

        _pendingAdds.Clear();
    }

    private IQueryable<ItemSetModel> ItemSetQuery()
    {
        return _context.ItemSets
            .Include(itemSet => itemSet.Owner)
            .Include(itemSet => itemSet.Items);
    }
}
