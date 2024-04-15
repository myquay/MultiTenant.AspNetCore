namespace MultiTenant.AspNetCore
{
    /// <summary>
    /// Resolves the current tenant
    /// </summary>
    public interface ITenantResolutionStrategy
    {
        /// <summary>
        /// Get the current tenant identifier
        /// </summary>
        /// <returns></returns>
        Task<string> GetTenantIdentifierAsync();
    }
}
