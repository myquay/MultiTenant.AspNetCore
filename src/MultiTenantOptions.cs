namespace MultiTenant.AspNetCore
{
    /// <summary>
    /// Configuration options
    /// </summary>
    public class MultiTenantOptions<T> where T : ITenantInfo
    {
        /// <summary>
        /// Disable the automatic registration of the multitenant pipeline
        /// </summary>
        public bool DisableAutomaticPipelineRegistration { get; set; } = false;

        /// <summary>
        /// The behavior when a tenant is not found
        /// </summary>
       public MissingTenantBehavior MissingTenantBehavior { get; set; } = MissingTenantBehavior.ThrowException;

        /// <summary>
        /// Default tenant when no matching tenant is found
        /// </summary>
        public T? DefaultTenant { get; set; }
    }

    public enum MissingTenantBehavior
    {
        /// <summary>
        ///  Throw an exception when a tenant is not found
        /// </summary>
        ThrowException,
        ///                      
        /// <summary>
        /// Use the default tenant when a tenant is not found
        /// </summary>
        UseDefault
    }
}