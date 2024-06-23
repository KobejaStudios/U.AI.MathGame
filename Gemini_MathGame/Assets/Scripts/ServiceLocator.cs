using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static ConcurrentDictionary<Type, object> _services = new();
    
    public static T Get<T>() where T : class
    {
        return (T)_services[typeof(T)];
    }
    
    public static void CreateAndRegisterService<TInterface, TService>() where TService: class, TInterface, new() where TInterface : class
    {
        AddService<TInterface>(new TService());
    }

    private static void AddService<TInterface>(TInterface service) where TInterface: class
    {
        _services[typeof(TInterface)] = service;
    }
}
