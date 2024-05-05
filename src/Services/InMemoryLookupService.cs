namespace MultiTenant.AspNetCore.Services
{
    internal class InMemoryLookupService<T>(IEnumerable<T> Tenants) : ITenantLookupService<T> where T : ITenantInfo
    {
        public Task<T?> GetTenantAsync(string identifier)
        {
            return Task.FromResult(Tenants.SingleOrDefault(t => t.Identifier == identifier));
        }
    }
}
