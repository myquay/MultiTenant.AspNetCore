namespace MultiTenant.AspNetCore.Tests
{
    internal class TestTenant : ITenantInfo
    {
        public required string Id { get; set; }
        public required string Identifier { get; set; }
    }
}
