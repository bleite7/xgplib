using MediatR;

namespace XgpLib.SyncService.Application.Commands;

public class SyncGenresCommand : IRequest<Unit>
{
    public CancellationToken CancellationToken { get; }

    public SyncGenresCommand(CancellationToken cancellationToken = default) => CancellationToken = cancellationToken;
}
