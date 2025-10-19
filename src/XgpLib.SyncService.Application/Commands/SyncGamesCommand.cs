using MediatR;

namespace XgpLib.SyncService.Application.Commands;

public class SyncGamesCommand : IRequest<Unit>
{
    public CancellationToken CancellationToken { get; }

    public SyncGamesCommand(CancellationToken cancellationToken = default) => CancellationToken = cancellationToken;
}
