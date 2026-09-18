using System.Security.Claims;

using Dynasty.Carrington.Core.Abstractions;

using Microsoft.AspNetCore.Components.Authorization;

namespace Dynasty.Carrington.Blake.Web.Services;

/// <summary>
///     Runs one use case in a fresh service scope. In interactive server rendering a scoped service
///     would otherwise live as long as the circuit, which for a <c>DbContext</c> means stale tracked
///     entities for the whole session. A new scope also knows nothing about the circuit's user, so the
///     id is copied over explicitly.
/// </summary>
public sealed class HandlerDispatcher(AuthenticationStateProvider authentication, IServiceScopeFactory scopes)
{
    public Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
    {
        return Run(provider => provider
            .GetRequiredService<IQueryHandler<TQuery, TResult>>()
            .Handle(query, cancellationToken));
    }

    public Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
    {
        return Run(async provider =>
        {
            await provider.GetRequiredService<ICommandHandler<TCommand>>().Handle(command, cancellationToken);

            return true;
        });
    }

    public Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
    {
        return Run(provider => provider
            .GetRequiredService<ICommandHandler<TCommand, TResult>>()
            .Handle(command, cancellationToken));
    }

    private async Task<T> Run<T>(Func<IServiceProvider, Task<T>> action)
    {
        string userId = await CurrentUserId();

        await using AsyncServiceScope scope = scopes.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ScopedCurrentUser>().UserId = userId;

        return await action(scope.ServiceProvider);
    }

    private async Task<string> CurrentUserId()
    {
        AuthenticationState state = await authentication.GetAuthenticationStateAsync();

        return state.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new InvalidOperationException("Brak zalogowanego użytkownika.");
    }
}
