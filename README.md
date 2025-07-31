# DrsBasicDI Dependency Injection Container Class Library
## Overview
The ***DrsBasicDI*** class library is a simple and lightweight dependency injection container designed for .NET applications. It provides a
straightforward way to manage object lifetimes and dependencies, making it easier to build maintainable and testable applications.

## Background
I started this project as a way to learn more about dependency injection and how to implement it in a .NET application. I wanted to create a simple
and lightweight container that could be used in small to medium-sized projects without the overhead of larger frameworks. The goal was to provide a
basic implementation that could meet the needs of moderately complex projects.

## Features
- **Simple API**: The fluent API is designed to be easy to use and understand, making it accessible for developers of all skill levels.
- **Lightweight**: The library is lightweight and has minimal dependencies, making it suitable for small to medium-sized projects.
- **Flexible**: The container supports various lifetimes (transient, singleton, and scoped) to accommodate different use cases.
- **Constructor Injection**: The container uses constructor injection to resolve dependencies, promoting best practices in software design.
- **Keyed Services**: The container allows for the registration of keyed services, enabling the resolution of specific implementations based on keys.
- **Thread-Safe**: The container is designed to be thread-safe, ensuring that it can be used in multi-threaded environments without issues.
- **Unit Tested**: The library includes unit tests to ensure the correctness and reliability of the implementation.

## The ***ContainerBuilder*** Class
The ***ContainerBuilder*** class is the main entry point for configuring the dependency injection container. It provides methods for registering
services, specifying their lifetimes, and building the container. The builder pattern is used to create a fluent API that allows for easy chaining of
methods. The ***ContainerBuilder*** class is designed to be simple and intuitive, making it easy for developers to configure their services without
the need for extensive documentation or examples. The builder pattern allows for a clean and readable syntax, enabling developers to focus on the
configuration of their services rather than the underlying implementation details.

### Retrieving an Instance of the ***ContainerBuilder*** Class
To retrieve an instance of the ***ContainerBuilder*** class, you can use the static method ***Instance***. This method returns a singleton instance of
the ***ContainerBuilder*** class, which can then be used to configure the container. The ***Instance*** method is a static factory method that
provides a convenient way to get the ***ContainerBuilder*** class without the need for a constructor. This approach allows for a more streamlined and
readable syntax when configuring the container, as developers can chain method calls directly after calling ***Instance***.

> [!NOTE]
> *The **Instance** method actually returns an **IContainerBuilder** interface object. This provides a level of abstraction between the user's source
> code and the internals of the **DrsBasicDI** class library.*

Example:

```csharp
using DrsBasicDI;
IContainerBuilder builder = ContainerBuilder.Instance;
```

### Registering Services
The ***ContainerBuilder*** class provides methods for registering services with different lifetimes. The following methods are available for
registering services that don't require the use of a resolving key or a factory method:

- ***AddTransient<TDependency, TResolving>()***: Registers a service with a transient lifetime. A new instance of the service will be created each
  time it is requested.
- ***AddSingleton<TDependency, TResolving>()***: Registers a service with a singleton lifetime. A single instance of the service will be created and
  shared across all requests.
- ***AddScoped<TDependency, TResolving>()***: Registers a service with a scoped lifetime. A new instance of the service will be created for each
  scope, and the same instance will be shared within that scope.

The *TDependency* type parameter is often referred to as the "service type" or "interface type," while the *TResolving* type parameter is
referred to as the "implementation type" or "concrete type." The *TDependency* type parameter represents the type that clients will depend on, while
the *TResolving* type parameter represents the actual implementation of that type. This separation allows for better abstraction and decoupling of
dependencies, making it easier to swap out implementations or create mock objects for testing purposes.

The following code snippet illustrates the use of these methods:

```csharp
using DrsBasicDI;
IContainer container = ContainerBuilder.Instance
    .AddTransient<IService, Service>()
    .AddSingleton<IRepository, Repository>()
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .Build();
```

There are a few things to take note of in this example:

- The fluent API allows these methods to be chained together. All of your services can be registered in a single statement.
- The chain of method calls ends with a call to the ***Build()*** method. This method creates an instance of the ***Container*** class, which is the
  actual dependency injection container that will be used to resolve dependencies. (The class is returned to the user as an **IContainer** interface
  object.)
- Once the container is built it can't be altered. This means that you can't add or remove services after the container has been built. This is a
  design decision that helps to ensure the integrity of the container and prevents accidental changes to the configuration after it has been set up.
- The ***ContainerBuilder*** is of no further use after the container has been built. In other words, you can't use the ***ContainerBuilder*** to
  build another container or to alter the existing one.

### Specifying Factory Methods and Resolving Keys
The ***ContainerBuilder*** class also provides methods for registering services that require a factory method and/or a resolving key. The following
methods are available for registering these types of services:

- ***AddTransient<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddSingleton<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddScoped<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)***

Note that each of these methods takes a function that accepts a ***DependencyBuilder*** object and returns a ***DependencyBuilder*** object. This
allows you to further configure the service using the ***DependencyBuilder*** class. The ***DependencyBuilder*** class is a helper class that provides
a fluent API for configuring the properties of the service being registered. The ***DependencyBuilder*** class contains two methods that are pertinent
to our discussion:

- ***WithFactory(Func\<object> factory)***: Specifies a factory method that will be used to create the instance of the service. This allows for more
  complex initialization logic or the use of external resources when creating the service.
- ***WithResolvingKey(string resolvingKey)***: Specifies a resolving key that can be used to resolve the service. This allows for the registration of
  multiple implementations of the same service type, enabling the use of different implementations based on the resolving key.

The ***DependencyBuilder*** class isn't accessible directly by the end user. Instead, it must be accessed through the use of a lambda expression which
is passed to the ***AddTransient***, ***AddSingleton***, or ***AddScoped*** methods. This is done to ensure that the ***DependencyBuilder*** is only
used in the context of service registration and to prevent accidental misuse or confusion with other parts of the API. By requiring the use of a
lambda expression, we can ensure that the ***DependencyBuilder*** is only used in the appropriate context and that it is properly disposed of after
the service has been registered. This helps to maintain the integrity of the container and prevents accidental changes to the configuration after it
has been set up.

The following code snippet illustrates the use of these methods:

```csharp
using DrsBasicDI;

public interface IMyService
{
    string Name { get; }
}

public class MyService : IMyService
{
    public MyService(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public const string Key1 = "Test1";
public const string Key2 = "Test2";

IContainer container = ContainerBuilder.Instance
    .AddTransient<IMyService, MyService>(builder => builder
        .WithFactory(() => new MyService("Transient"))
        .WithResolvingKey(Key1))
    .AddSingleton<IMyService, MyService>(builder => builder
        .WithResolvingKey(Key2))
    .AddScoped<IMyService, MyService>(builder => builder
        .WithFactory(() => new MyService("Scoped"))
    .Build();
```

In this example, we register three different implementations of the ***IMyService*** interface using different lifetimes and resolving keys. Two of
the implementations use the ***WithFactory*** method to specify a factory method that creates the instance of the service, and two use the
***WithResolvingKey*** method to specify a resolving key that can be used to resolve the service. Also, note that the factory method must not take any
parameters and it must return an object whose type is assignable to the service type. This is a design decision that helps to ensure that the factory
method is simple and easy to use, while still providing the flexibility needed to create complex objects.

One other thing to note is that services that are registered without the ***WithResolvingKey*** method will default to a key that is an empty string.
That is why in the example we don't need to call the ***WithResolvingKey*** method for the ***AddScoped*** method even though we are registering
another implementation of the ***IMyService*** interface. The ***AddScoped*** method will automatically use the empty string as the resolving key
unless we specify otherwise. This allows for a more streamlined and readable syntax, as we don't need to specify the resolving key for every
implementation of the service.

### Other Methods for Registering Services
The methods mentioned above should be able to meet most all of your service registration needs. However, there are a few other methods that are
provided for convenience. These methods are not as commonly used, but they can be helpful in certain situations. The following methods are available:

- ***AddDependency(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddDependency\<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddDependency\<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddScoped(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddScoped\<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddSingleton(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddSingleton\<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddTransient(Func<DependencyBuilder, DependencyBuilder> builder)***
- ***AddTransient\<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder)***

In addition, the ***DependencyBuilder*** class provides additional methods for defining the properties of the service being registered. The complete
list of methods includes:

- ***WithDependencyType(Type dependencyType)***: Specifies the dependency type (service type) for the service being registered.
- ***WithDependencyType\<TDependency>()***: Specifies the dependency type (service type) for the service being registered. (Same as the previous
  method, but using a generic type parameter.)
- ***WithFactory(Func\<object> factory)***: Specifies a factory method that will be used to create the instance of the service.
- ***WithLifetime(DependencyLifetime lifetime)***: Specifies the lifetime of the service being registered. The lifetime can be one of the following:
  - ***DependencyLifetime.Transient***: A new instance of the service will be created each time it is requested.
  - ***DependencyLifetime.Singleton***: A single instance of the service will be created and shared across all requests.
  - ***DependencyLifetime.Scoped***: A new instance of the service will be created for each scope, and the same instance will be shared within that
    scope.
- ***WithResolvingKey(string resolvingKey)***: Specifies a resolving key that can be used to resolve the service, allowing multiple implementations of
  the same service type.
- ***WithResolvingType(Type resolvingType)***: Specifies the resolving type (implementation type) for the service being registered.
- ***WithResolvingType\<TResolving>()***: Specifies the resolving type (implementation type) for the service being registered. (Same
  as the previous method, but using a generic type parameter.)

The following example illustrates the use of these methods:

```csharp
using DrsBasicDI;

public interface IMyService
{
    string Name { get; }
}

public class MyService : IMyService
{
    public MyService(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

private Type _myServiceType = typeof(IMyService);
private Type _implementationType = typeof(MyService);

IContainer container = ContainerBuilder.Instance
    .AddDependency(builder => builder
        .WithDependencyType(_myServiceType)
        .WithLifetime(DependencyLifetime.Transient)
        .WithResolvingType<MyService>())
    .AddSingleton<IMyService>(builder => builder
        .WithResolvingKey("Test1")
        .WithResolvingType(_implementationType))
    .Build();
```

### Dependency Build Rules
Each dependency (service) that is registered must conform to the following rules:

- Each service must have a defined dependency type, resolving type, and lifetime. Optionally, they may also specify a factory method and/or a
  resolving key.
- Each property of a service must be specified only once. For example, the following code would result in an exception because the dependency type is
  specified both by the generic type parameter on the ***AddSingleton*** method and by the ***WithDependencyType*** method:

  ```csharp
    using DrsBasicDI;
    IContainer container = ContainerBuilder.Instance
        .AddSingleton<IMyService>(builder => builder
            .WithDependencyType(typeof(IMyService))
            .WithResolvingType<MyService>())
        .Build();
  ```

- No two services can have the same dependency type and resolving key.
- The dependency type of the service must be an interface type, a concrete class type, or a fully-constructed generic type (valid type values must be
  assigned to all generic type parameters). Abstract class types are not supported.
- If specified, the return type of the factory method must be assignable to the dependency type.
- The dependency lifetime must not be undefined. It must be explicitly set to one of the valid values (***DependencyLifetime.Transient***,
  ***DependencyLifetime.Singleton***, or ***DependencyLifetime.Scoped***).
- The resolving key, if specified, must be a valid string value. It must not be null or empty. (The empty string is reserved for internal use by the
  class library.)
- The resolving type of the service must be a concrete class type or a fully-constructed generic type (valid type values must be assigned to all
  generic type parameters). Abstract class types are not supported.
- The resolving type of the service must be assignable to the dependency type. This means that the resolving type must implement the dependency type
  either through an interface or through inheritance.

Any violation of the above rules will result in a ***DependencyBuildException*** being thrown.

## The ***Container*** Class
Compared to the ***ContainerBuilder*** class, the ***Container*** class is much simpler. It is the actual dependency injection container that is used
to resolve dependencies. The class consists of only three public methods:
- ***Resolve\<TDependency>(string key = "")***: Resolves an instance of the specified dependency type. The optional *key* parameter can be used to
  specify a resolving key for the service. If no key is specified, the empty string will be used as the default key.
- ***CreateScope()***: Creates a new scope for resolving scoped services. This method returns an instance of the ***IScope*** interface, which can be
  used to resolve scoped services within the scope.
- ***Dispose()***: Disposes of the container and releases any resources held by it. This method should be called when the container is no longer
  needed.

### The ***Resolve*** Method
The ***Resolve*** method is the method used to resolve dependencies from the container. It takes a generic type parameter that specifies the
dependency type to be resolved. The method also takes an optional *key* parameter that can be used to specify a resolving key for the service. If no
key is specified, the empty string will be used as the default key. The method returns an instance of the specified dependency type.

The following steps are taken when resolving a dependency:

1. The method checks if the requested dependency type is registered in the container with the specified resolving key (or empty string if no key was
   specified). If not, a ***DependencyInjectionException*** is thrown.
1. If the dependency type is registered, the method checks if the dependency is a singleton. If it is, the method returns the cached instance of the
   resolving type if it was previously created.
1. If the dependency is a transient, or no cached resolving instance was found, then a check is made to determine if a factory method was specified
   for the dependency. If a factory method was specified, it is invoked to create the instance of the resolving type and the instance is returned.
   (For singleton dependencies, the resolving instance is also cached for future use.)
1. If no factory method was specified, the method uses reflection to create an instance of the resolving type using its constructor. Any dependencies
   required by the constructor are resolved recursively using the same process. The instance is then returned. (For singleton dependencies, the
   resolving instance is also cached for future use.)

> [!NOTE]
> *The resolving instance is always cast to the dependency type before being returned.*

> [!IMPORTANT]
> *Any errors detected while resolving a dependency will result in a **DependencyInjectionException** being thrown.*

The following example illustrates the use of the ***Resolve*** method:

```csharp
using DrsBasicDI;

public interface IService1
{
    void DoSomething();
}

public class Service1 : IService1
{
    public void DoSomething()
    {
        Console.WriteLine("Service1 doing something...");
    }
}

public interface IService2
{
    void DoSomething();
}

public class Service2 : IService2
{
    private readonly IService1 _service1;

    public Service2(IService1 service1)
    {
        _service1 = service1;
    }

    public void DoSomething()
    {
        Console.WriteLine("Service2 calling Service1.DoSomething()...");
        _service1.DoSomething();
    }
}


IContainer container = ContainerBuilder.Instance
    .AddSingleton<IService1, Service1>()
    .AddTransient<IService2, Service2>()
    .Build();

IService2 service = container.Resolve<IService2>();
service.DoSomething();
```

In this example, we register two services: ***IService1*** and ***IService2***. The ***IService1*** service is registered as a singleton, while the
***IService2*** service is registered as a transient. When we resolve the ***IService2*** service, the container automatically resolves the
***IService1*** dependency and injects it into the constructor of the ***Service2*** class. The ***DoSomething*** method of the ***IService2*** class
is then called, which in turn calls the ***DoSomething*** method of the ***IService1*** class. The output of the program will be:

```plaintext
Service2 calling Service1.DoSomething()...
Service1 doing something...
```

One remaining detail of the ***Resolve*** method concerns what happens when a scoped dependency is resolved from the container. Normally, scoped
dependencies would be resolved from an ***IScope*** object created by the ***CreateScope*** method. However, if a scoped dependency is resolved from
the container it will essentially be treated as though it were a singleton. In this case we say that the scoped dependency is being resolved in the
"global scope." The same scoped dependency may at any time also be resolved from an ***IScope*** object created by the ***CreateScope*** method. In
this case, the scoped dependency will be resolved from the scope and will not be treated as a singleton. This means that the same scoped dependency
may be resolved from both the container and a scope, but the instances and lifetimes will be different. This is an important distinction to keep in
mind when working with scoped dependencies, as it can lead to unexpected behavior if not properly understood. The general rule of thumb is that scoped
dependencies should be resolved from an ***IScope*** object whenever possible, and only resolved from the container in special cases where a singleton
instance is required.

### The ***CreateScope*** Method
The ***CreateScope*** method is used to create a new scope for resolving scoped services. This method returns an instance of the ***IScope***
interface, which can be used to resolve scoped services within the scope. Best practice is to create the scope within a using statement, which will
ensure that the scope is properly disposed of when it is no longer needed. This is important because scoped services are designed to be used within a
scope, and disposing of the scope will also dispose of any scoped services that were created within that scope.

The following example illustrates the use of the ***CreateScope*** method:

```csharp
using DrsBasicDI;

public interface IService
{
    void DoSomething();
}

public class Service : IService
{
    public void DoSomething()
    {
        Console.WriteLine("Service1 doing something...");
    }
}

IContainer container = ContainerBuilder.Instance
    .AddScoped<IService, Service>()
    .Build();

using (IScope scope = container.CreateScope())
{
    IService service = scope.Resolve<IService>();
    service.DoSomething();
}
```

In this example, we register a service called ***IService*** with a scoped lifetime. We then create a new scope using the ***CreateScope*** method
and resolve the ***IService*** dependency within that scope. The ***DoSomething*** method of the ***Service*** class is then called, which prints a
message to the console. When the scope is disposed of at the end of the using statement, any scoped services created within that scope are also
disposed of. This ensures that resources are properly released and that there are no memory leaks or other issues related to the lifetime of the
scoped services.

### The ***Dispose*** Method
The ***Dispose*** method is used to dispose of the container and release any resources held by it. This method should be called when the container is
no longer needed. It is important to call this method to ensure that any resources held by the container are properly released and that there are no
memory leaks or other issues related to the lifetime of the container. The ***Dispose*** method will also dispose of any singleton or scoped
dependencies that were created by the container. This ensures that all resources are properly released and that there are no memory leaks or other
issues related to the lifetime of the dependencies. The ***Dispose*** method is a standard part of the ***IDisposable*** interface, which is
implemented by the ***Container*** class.

Strictly speaking, the ***Dispose*** method only needs to be called if any of the singleton or scoped dependencies that were created by the container
are disposable. (That is, they hold unmanaged resources, or have some other special cleanup requirements, and they implement ***IDisposable***.) If
none of the dependencies are disposable, then the ***Dispose*** method can be safely omitted and resource cleanup will be handled by the normal
garbage collection process. However, it is generally a good practice to call the ***Dispose*** method when the container is no longer needed, just to
be safe.

## The ***Scope*** Class
The ***Scope*** class is an implementation of the ***IScope*** interface. As mentioned earlier, an ***IScope*** object is created by the
***CreateScope*** method of the ***Container*** class. The ***Scope*** class is responsible for managing the lifetime of scoped dependencies and
ensuring that they are properly disposed of when the scope is no longer needed. The class contains two public methods:
- ***Resolve\<TDependency>(string key = "")***: Resolves an instance of the specified dependency type within the scope. The optional *key* parameter
  can be used to specify a resolving key for the service. If no key is specified, the empty string will be used as the default key.
- ***Dispose()***: Disposes of the scope and releases any resources held by it. Normally, this method is called automatically at the end of the
  using statement that creates the scope. However, it can also be called explicitly if needed.

### The ***Resolve*** Method
The ***Resolve*** method is similar to the ***Resolve*** method of the ***Container*** class, but it is specifically designed for resolving scoped
dependencies within the scope. The method takes a generic type parameter that specifies the dependency type to be resolved. The method also takes an
optional *key* parameter that can be used to specify a resolving key for the service. If no key is specified, the empty string will be used as the
default key. The method returns an instance of the specified dependency type.

The ***Resolve*** method of the ***Scope*** class can resolve transient and singleton dependencies in addition to scoped dependencies. This is
necessary because a scoped dependency may have a constructor that contains dependencies that are not scoped. In this case, the ***Scope*** class will
resolve the transient and singleton dependencies as needed. The following steps are taken to resolve a dependency within a scope:

1. The method checks if the requested dependency type is registered in the container with the specified resolving key (or empty string if no key was
   specified). If not, a ***DependencyInjectionException*** is thrown.
1. If the dependency is registered, the method checks if the dependency is scoped or a singleton.
1. If the dependency is scoped, it checks the scoped cache to see if the dependency has already been resolved within the scope. If it has, the cached
   instance is returned.
1. If the dependency is a singleton, it checks the container cache to see if the dependency has already been resolved. If it has, the cached instance
   is returned.
1. If no cached instance was found, or if the dependency was a transient, the method checks if a factory method was specified for the dependency. If a
   factory method was specified, it is invoked to create the instance of the resolving type and the instance is returned. (For singleton dependencies,
   the resolving instance is also saved to the container cache for future use. For scoped dependencies, the resolving instance is saved to the scoped
   cache for future use.)
1. If no factory method was specified, the method uses reflection to create an instance of the resolving type using its constructor. Any dependencies 
   required by the constructor are resolved recursively using the same process. The instance is then returned. (For singleton dependencies, the resolving
   instance is also saved to the container cache for future use. For scoped dependencies, the resolving instance is saved to the scoped cache for future
   use.)

As with the ***Resolve*** method of the ***Container*** class, the resolving instance is always cast to the dependency type before being returned.
Also, any errors detected while resolving a dependency will result in a ***DependencyInjectionException*** being thrown.

> [!NOTE]
> *Each **Scope** class instance has its own scoped cache. Only scoped dependencies resolved by that specific **Scope** instance are saved to that
> cache.*

The following example is similar to the example given for the ***Resolve*** method of the ***Container*** class, but it has been modified slightly to
show how things work when resolving scoped dependencies:

```csharp
using DrsBasicDI;

public interface IService1
{
    void DoSomething();
}

public class Service1 : IService1
{
    public void DoSomething()
    {
        Console.WriteLine("Service1 doing something...");
    }
}

public interface IService2
{
    void DoSomething();
}

public class Service2 : IService2
{
    private readonly IService1 _service1;

    public Service2(IService1 service1)
    {
        _service1 = service1;
    }

    public void DoSomething()
    {
        Console.WriteLine("Service2 calling Service1.DoSomething()...");
        _service1.DoSomething();
    }
}


IContainer container = ContainerBuilder.Instance
    .AddSingleton<IService1, Service1>()
    .AddScoped<IService2, Service2>()
    .Build();

using (IScope scope = container.CreateScope())
{
    IService2 service = scope.Resolve<IService2>();
    service.DoSomething();
}
```

In this example, we register two services: ***IService1*** and ***IService2***. The ***IService1*** service is registered as a singleton, while the
***IService2*** service is registered as a scoped dependency. When we create a new scope using the ***CreateScope*** method and resolve the
***IService2*** dependency within that scope, the container automatically resolves the ***IService1*** dependency and injects it into the constructor
of the ***Service2*** class. The ***DoSomething*** method of the ***IService2*** object is then called, which in turn calls the ***DoSomething***
method of the ***IService1*** class. The output will be identical to the output given in the example of the ***Resolve*** method of the
***Container*** class.

It is important to note in the above example that when the ***IService1*** dependency is resolved within the scope, the resolving instance will get
saved to the container's cache since it is a singleton. The ***IService2*** dependency, on the other hand, will be saved to the scoped cache since it
is a scoped dependency. At the end of the using block the ***Dispose*** method of the ***Scope*** class will be called automatically, which will
dispose of the ***IService2*** dependency and release any resources held by it. The ***IService1*** dependency will not be disposed of, as it is a
singleton and is managed by the container. Future requests for the ***IService1*** dependency will return the value that was saved to the container's
cache whether the request is made through the container instance or some other scope instance.

### The ***Dispose*** Method
The ***Dispose*** method is used to dispose of the scope and release any resources held by it. Everything that was said about the ***Dispose*** method
of the ***Container*** class applies to the ***Dispose*** method of the ***Scope*** class as well. The only difference is that only scoped
dependencies created within the scope will be disposed of when the ***Dispose*** method is called. Normally, this would be done automatically at the
end of a using block, but it can also be called explicitly if needed.

## The ***ResolvingKeyAttribute*** Class
We've already shown how we can specify a resolving key on the ***Resolve*** methods of the ***Container*** class and ***Scope*** class. But what about
a dependency that gets resolved because it is a constructor parameter of another dependency? How do we specify a resolving key in this case? Well,
that's where the ***ResolvingKeyAttribute*** class comes in. This class is an attribute that can be applied to a constructor parameter of a
dependency. The ***ResolvingKeyAttribute*** class is a simple attribute class that contains a single property called ***Key***. This property is used
to specify the resolving key for the dependency.

The following example illustrates the use of the ***ResolvingKey*** attribute:

```csharp
using DrsBasicDI;

public interface IService1
{
    void DoSomething();
}

public class Service1 : IService1
{
    public void DoSomething()
    {
        Console.WriteLine("Service1 doing something...");
    }
}

public interface IService2
{
    void DoSomething();
}

public class Service2 : IService2
{
    private readonly IService1 _service1;

    public Service2([ResolvingKey("Test")] IService1 service1)
    {
        _service1 = service1;
    }

    public void DoSomething()
    {
        Console.WriteLine("Service2 calling Service1.DoSomething()...");
        _service1.DoSomething();
    }
}


IContainer container = ContainerBuilder.Instance
    .AddSingleton<IService1, Service1>()
        .WithResolvingKey("Test")
    .AddTransient<IService2, Service2>()
    .Build();

IService2 service = container.Resolve<IService2>();
service.DoSomething();
```

In this example, we register two services: ***IService1*** and ***IService2***. The ***IService1*** service is registered as a singleton with a
resolving key of "Test", while the ***IService2*** service is registered as a transient. Also, the ***ResolvingKey*** attribute is applied to the
***IService1*** parameter in the constructor of the ***IService2*** class. When we resolve the ***IService2*** service, the container automatically
resolves the ***IService1*** dependency using the specified resolving key and injects it into the constructor of the ***Service2*** class.