namespace MultiTenant.AspNetCore.Services
{
    internal class InMemoryLookupService<T>(IEnumerable<T> Tenants) : ITenantLookupService<T> where T : ITenantInfo
    {
        public Task<T> GetTenantAsync(string identifier)
        {
            return Task.FromResult(Tenants.Single(t => t.Identifier == identifier));
        }
    }
}
