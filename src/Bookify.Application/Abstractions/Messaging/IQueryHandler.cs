using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;

public interface IQueryHandler<IOuery, TResponse> : IRequestHandler<IOuery, Result<TResponse>>
    where IOuery : IQuery<TResponse>
{
}
