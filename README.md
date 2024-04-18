# ![Project Logo](https://raw.githubusercontent.com/myquay/MultiTenant.AspNetCore/main/assets/logo-sm.png) MultiTenant.AspNetCore

Multi-tenancy support for ASP.NET Core 8

## About

A lightweight, easy to configure, open-source library which allows you to build multi-tenanted applications in ASP.NET Core 8.

It supports
* Tenant resolution
* Per-tenant service registration with the ASP.NET Dependency Injection
* Per-tenant options registration
* Per-tenant pipeline configuration

I've written a deep-dive on the library internals here: [https://michael-mckenna.com/multi-tenant-asp-dot-net-8-tenant-resolution/](https://michael-mckenna.com/multi-tenant-asp-dot-net-8-tenant-resolution/)

## Quickstart

The library is designed to follow common ASP.NET Core patterns for ease of configuration.

### Installation

The library is distributed as a NuGet package: [https://www.nuget.org/packages/MultiTenant.AspNetCore/](https://www.nuget.org/packages/MultiTenant.AspNetCore/) you can install it using your favourite package manager, or download the source and compile it locally.

### Define how your application manages tenants

Multi-tenant requirements vary widely by use-case, this library provides the following extension points to cater for a wide range of usecases
  * What information is required in the tenant context - `ITenantInfo`
  * How tenants are identified (e.g. by domain) - `ITenantResolutionStrategy`
  * How tenant information is stored (e.g. in a database) - `ITenantLookupService<>`

#### Tenant data (`ITenantInfo`)

Implement the `ITenantInfo` interface to define your tenant specific data.

```csharp
public class TenantInfo : ITenantInfo
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  ... other properties ...
}
```

#### Tenant identification (`ITenantResolutionStrategy`)

Implement the `ITenantResolutionStrategy` to define how tenants are identified in your system, a common pattern is to give each tenant a different subdomain making the host a good candidate as an identifier. Here is an exmaple of how to implement a resolution strategy based on hostname.

```csharp
public class HostResolutionStrategy(IHttpContextAccessor httpContextAccessor) : ITenantResolutionStrategy
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<string> GetTenantIdentifierAsync()
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new InvalidOperationException("HttpContext is not available");

        return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
    }
}
```

#### Tenant lookup (`ITenantLookupService<>`)

Implement the `ITenantLookupService<TenantInfo>` interface to define how your application loads tenant configuration. This could be from memory, configuration, a database, or other durable data store depending on your requirements. 

The lookup service accepts the identifier returned from your tenant resolution strategy to find the tenant.

```csharp
 public class TenantLookupService() : ITenantLookupService<TenantInfo>
 {
     public Task<TenantInfo> GetTenantAsync(string identifier)
     {
         ... your implementation ...
     }
 }
```

### Basic configuration

Configure your application to support multi-tenancy in the same place you register all your middleware and services. 

```csharp
//Add the library to your application and define the tenant data available in your tenant context 
builder.Services.AddMultiTenancy<TenantInfo>()
    //Specify your resolution strategy
    .WithResolutionStrategy<HostResolutionStrategy>()
    //Specify your tenant data provider 
    .WithTenantLookupService<TenantLookupService>();
```

You're done, whenever you want to access the current tenant just inject `IMultiTenantContextAccessor<TenantInfo>` using ASP.NET Core DI and you'll have access to the current tenant.

### Advanced configuration

The library supports configuring services or options differently for different tenants, this allows you to do things such as register a database context with a seperate connection string etc.

```csharp
    ///Add a service configured different per-tenant
    .WithTenantedServices((services, tenant) =>
    {
       if (tenant != null)
           services.AddSingleton<SomeService>(options =>
           {
               options.SomeSetting = tenant.SomeTenantSpecificSetting;
           });
    })
    ///Register different options per-tenant (e.g. different localisations)
    .WithTenantedConfigure<RequestLocalizationOptions>((options, tenant) =>
    {
        var supportedCultures = tenant?.CultureOptions ?? ["en-NZ"];
        options.SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    
    });
```

The library also supports modifying the middleware pipeline based on tenant. This is especially useful if a middleware captures its configuration on startup. 

For example the localisation middleware caches its configuration on start-up so by the time a request comes in we cannot alter it for that tenant's configuration. By having a tenant specific pipeline the configuration options captured by the middleware on start-up is now tenant specific. 

In the example below, the application will now respect the localisation of each tenant even though the middleware does not allow configuration to change after startup.

```csharp
app.UseMultiTenantPipeline<TenantOptions>((tenant, app) =>
{
    app.UseRequestLocalization();
});
```

This makes our library compatible with a wide range of existing middleware without the need of additional work-arounds.

