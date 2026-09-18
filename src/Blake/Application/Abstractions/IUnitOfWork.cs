namespace Dynasty.Carrington.Blake.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken cancellationToken);
}
