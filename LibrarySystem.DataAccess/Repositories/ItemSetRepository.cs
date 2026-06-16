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
        if (model == null) return null;

        var itemSet = _mapper.Map<ItemSet>(model);

        // Inject items manually since mapping ignores them
        foreach (var itemModel in model.Items)
        {
            var item = _mapper.Map<Item>(itemModel);
            itemSet.Items.Add(item);
        }

        return itemSet;
    }

    public async Task<IEnumerable<ItemSet>> GetAllAsync()
    {
        var models = await ItemSetQuery().ToListAsync();

        return models.Select(model =>
        {
            var itemSet = _mapper.Map<ItemSet>(model);

            // Inject items manually since mapping ignores them
            foreach (var itemModel in model.Items)
            {
                var item = _mapper.Map<Item>(itemModel);
                itemSet.Items.Add(item);
            }

            return itemSet;
        }).ToList();
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
    // أضف هذه الدالة داخل كلاس ItemSetRepository
    public async Task<IEnumerable<ItemSet>> GetAllWithDeletedAsync()
    {
        // نستخدم IgnoreQueryFilters لجلب المحذوف
        // وأضفنا Include للـ Owner أيضاً لتفادي أخطاء الـ OwnerName
        var models = await _context.ItemSets
            .IgnoreQueryFilters()
            .Include(iset => iset.Owner)
            .Include(iset => iset.Items)
            .AsNoTracking()
            .ToListAsync();

        return models.Select(model =>
        {
            var itemSet = _mapper.Map<ItemSet>(model);

            // Inject items manually since mapping ignores them
            foreach (var itemModel in model.Items)
            {
                var item = _mapper.Map<Item>(itemModel);
                itemSet.Items.Add(item);
            }

            return itemSet;
        }).ToList();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var model = await _context.ItemSets.FirstOrDefaultAsync(itemSet => itemSet.Id == id);
        if (model == null) return false;

        _context.ItemSets.Remove(model);
        return true;
    }
    public async Task<ItemSet?> GetByIdWithDeletedAsync(int id)
    {
        var model = await _context.ItemSets
            .IgnoreQueryFilters()
            .Include(iset => iset.Owner)
            .Include(iset => iset.Items)
            .FirstOrDefaultAsync(iset => iset.Id == id);

        if (model == null) return null;

        var itemSet = _mapper.Map<ItemSet>(model);
        foreach (var itemModel in model.Items)
        {
            var item = _mapper.Map<Item>(itemModel);
            itemSet.Items.Add(item);
        }
        return itemSet;
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
    public async Task<bool> RestoreAsync(int id)
    {
        // Fetch the model including soft-deleted ones (IgnoreQueryFilters)
        var model = await _context.ItemSets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(iset => iset.Id == id);

        if (model == null) return false;

        // Restore
        model.IsDeleted = false;
        model.DeletedAt = null;

        // No need to call Update – change tracking will handle it
        return true;
    }

}
