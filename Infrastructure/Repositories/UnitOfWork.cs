using Microsoft.EntityFrameworkCore.Storage;
using TaskManagementApi.Core.Interfaces;
using TaskManagementApi.Infrastructure.Data;

namespace TaskManagementApi.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; }
    public IProjectRepository Projects { get; }
    public ITaskRepository Tasks { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ITaskRepository taskRepository
        )
    {
        _context = context;
        Users = userRepository;
        Projects = projectRepository;
        Tasks = taskRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}