using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Abstractions.Caching;

// ganeric interface  
public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

// Marker interface 
public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }
}
