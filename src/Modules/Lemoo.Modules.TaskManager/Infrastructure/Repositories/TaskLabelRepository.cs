using Lemoo.Modules.TaskManager.Application.Repositories;
using Lemoo.Modules.TaskManager.Domain.Entities;
using Lemoo.Modules.TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lemoo.Modules.TaskManager.Infrastructure.Repositories;

/// <summary>
/// 任务标签仓储实现
/// </summary>
public class TaskLabelRepository : ITaskLabelRepository
{
    private readonly TaskManagerDbContext _context;

    public TaskLabelRepository(TaskManagerDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskLabel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TaskLabels
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskLabel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TaskLabels
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<TaskLabel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.TaskLabels
            .FirstOrDefaultAsync(l => l.Name == name, cancellationToken);
    }

    public async Task AddAsync(TaskLabel label, CancellationToken cancellationToken = default)
    {
        await _context.TaskLabels.AddAsync(label, cancellationToken);
    }

    public void Update(TaskLabel label)
    {
        _context.TaskLabels.Update(label);
    }

    public void Delete(TaskLabel label)
    {
        _context.TaskLabels.Remove(label);
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.TaskLabels
            .AnyAsync(l => l.Name == name, cancellationToken);
    }

    public async Task<List<TaskLabel>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.TaskLabelLinks
            .Where(link => link.TaskId == taskId)
            .Select(link => link.Label!)
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }
}
