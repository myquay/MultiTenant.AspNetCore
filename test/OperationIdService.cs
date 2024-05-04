namespace MultiTenant.AspNetCore.Tests
{
    public class OperationIdService
    {
        public readonly Guid Id;

        public OperationIdService()
        {
            Id = Guid.NewGuid();
        }
    }
}
