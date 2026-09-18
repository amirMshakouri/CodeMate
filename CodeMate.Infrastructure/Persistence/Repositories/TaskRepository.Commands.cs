
using CodeMate.Domain.Entities;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class TaskRepository
{
    public async Task AddAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);
    }

    public Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

