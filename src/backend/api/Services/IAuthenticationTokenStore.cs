namespace api.Services;

public interface IAuthenticationTokenStore
{
    Task DeleteToken(string id);
    Task<AuthToken> GetToken(string id);
    Task SetTokenAsync(string id, AuthToken token);
}

public class InMemoryAuthenticationTokenStore : IAuthenticationTokenStore
{
    private Dictionary<string, AuthToken> _store = new Dictionary<string, AuthToken>();

    public Task DeleteToken(string id)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }

    public Task<AuthToken> GetToken(string id)
    {
        return Task.FromResult(_store[id]);
    }

    public Task SetTokenAsync(string id, AuthToken token)
    {
        _store.Add(id, token);
        return Task.CompletedTask;
    }
}