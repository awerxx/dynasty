using Dynasty.Carrington.Blake.Application.Abstractions;

namespace Dynasty.Carrington.Blake.Web.Services;

/// <summary>
///     The current user of one handler scope. <see cref="HandlerDispatcher" /> fills it in from the
///     circuit's authentication state before resolving a handler.
/// </summary>
internal sealed class ScopedCurrentUser : ICurrentUser
{
    private string? userId;

    public string UserId
    {
        get => userId ?? throw new InvalidOperationException("Brak zalogowanego użytkownika.");
        set => userId = value;
    }
}
