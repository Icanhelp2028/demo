using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IocDemo
{
    public interface IMyServiceCollection
    {
    }

    public interface IMyServiceProvider : IMyServiceCollection
    {
        object GetService(Type type);
    }

    public class MyServiceProvider : IMyServiceProvider, IMyServiceCollection
    {
        public object GetService(Type serviceType)
        {
            return MyServiceExtension.GetService(serviceType);
        }
    }

    internal enum MyServiceScope
    {
        Transient,
        Scoped,
        Singleton
    }

    internal class MyServiceDescriptor
    {
        public Type Type;
        public object Instance;
        public MyServiceScope Scope;
        internal ReaderWriterLockSlim slim;

        private MyServiceDescriptor() { }

        public static MyServiceDescriptor Transient(object _instance)
        {
            return new MyServiceDescriptor
            {
                Instance = _instance,
                Scope = MyServiceScope.Transient
            };
        }

        public static MyServiceDescriptor Transient(Type _serviceType)
        {
            return new MyServiceDescriptor
            {
                Type = _serviceType,
                Scope = MyServiceScope.Transient
            };
        }

        public static MyServiceDescriptor Scoped(Type _serviceType)
        {
            return new MyServiceDescriptor
            {
                Type = _serviceType,
                Scope = MyServiceScope.Scoped
            };
        }

        public static MyServiceDescriptor Scoped(object _instance)
        {
            return new MyServiceDescriptor
            {
                Instance = _instance,
                Scope = MyServiceScope.Scoped
            };
        }

        public static MyServiceDescriptor Singleton(object _instance)
        {
            return new MyServiceDescriptor
            {
                Instance = _instance,
                Scope = MyServiceScope.Singleton,
                slim = new ReaderWriterLockSlim()
            };
        }

        public static MyServiceDescriptor Singleton(Type _serviceType)
        {
            return new MyServiceDescriptor
            {
                Type = _serviceType,
                Scope = MyServiceScope.Singleton,
                slim = new ReaderWriterLockSlim()
            };
        }
    }

    public static class MyServiceExtension
    {
        private static Dictionary<Type, MyServiceDescriptor> serviceTypes = new Dictionary<Type, MyServiceDescriptor>();
        [ThreadStatic] internal static Dictionary<Type, MyServiceDescriptor> scopedServiceTypes = null;

        static MyServiceExtension()
        {
            serviceTypes.Add(typeof(IMyServiceProvider), MyServiceDescriptor.Singleton(new MyServiceProvider()));
        }

        public static void AddTransient<T1>(this IMyServiceCollection services)
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Transient(typeof(T1)));
        }

        public static void AddTransient<T1, T2>(this IMyServiceCollection services) where T2 : T1
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Transient(typeof(T2)));
        }

        public static void AddScoped<T1>(this IMyServiceCollection services)
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Scoped(typeof(T1)));
        }

        public static void AddScoped<T1, T2>(this IMyServiceCollection services) where T2 : T1
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Scoped(typeof(T2)));
        }

        public static void AddSingleton<T1>(this IMyServiceCollection services, object obj)
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Singleton(obj));
        }

        public static void AddSingleton<T1>(this IMyServiceCollection services)
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Singleton(typeof(T1)));
        }

        public static void AddSingleton<T1, T2>(this IMyServiceCollection services) where T2 : T1
        {
            serviceTypes.Add(typeof(T1), MyServiceDescriptor.Singleton(typeof(T2)));
        }

        public static T GetService<T>(this IMyServiceProvider serviceProvider)
        {
            return (T)GetService(typeof(T));
        }

        private static MyServiceDescriptor GetServiceDescriptor(Type serviceType)
        {
            MyServiceDescriptor serviceDescriptor = null;

            if (scopedServiceTypes == null)
            {
                scopedServiceTypes = new Dictionary<Type, MyServiceDescriptor>();
            }
            else if (scopedServiceTypes.TryGetValue(serviceType, out serviceDescriptor))
            {
                return serviceDescriptor;
            }

            if (!serviceTypes.TryGetValue(serviceType, out serviceDescriptor))
            {
                serviceDescriptor = MyServiceDescriptor.Transient(serviceType);
            }
            else if (serviceDescriptor.Scope == MyServiceScope.Singleton)
            {
                var hasInstance = serviceDescriptor.Instance != null;
                if (hasInstance)
                    return serviceDescriptor;

                serviceDescriptor.slim.EnterReadLock();
                hasInstance = serviceDescriptor.Instance != null;
                serviceDescriptor.slim.ExitReadLock();

                if (hasInstance)
                    return serviceDescriptor;
            }

            if (serviceDescriptor.Type.IsInterface)
                throw new NotImplementedException();

            var constructor = serviceDescriptor.Type
                 .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                 .FirstOrDefault();

            var parameters = new ArrayList();

            foreach (var parameter in constructor.GetParameters())
            {
                var descriptor = GetServiceDescriptor(parameter.ParameterType);
                if (serviceDescriptor.Scope == MyServiceScope.Singleton)
                {
                    if (descriptor.Scope == MyServiceScope.Scoped)
                    {
                        throw new Exception("Scoped");
                    }
                }
                parameters.Add(parameter.HasDefaultValue ? parameter.DefaultValue : descriptor.Instance);
            }

            var instance = Activator.CreateInstance(serviceDescriptor.Type, parameters.ToArray());

            if (serviceDescriptor.Scope == MyServiceScope.Scoped)
            {
                scopedServiceTypes.Add(serviceType, serviceDescriptor = MyServiceDescriptor.Scoped(instance));
            }
            else if (serviceDescriptor.Scope == MyServiceScope.Singleton)
            {
                serviceDescriptor.slim.EnterWriteLock();
                serviceDescriptor.Instance = instance;
                serviceDescriptor.slim.ExitWriteLock();
            }
            else
            {
                serviceDescriptor = MyServiceDescriptor.Transient(instance);
            }

            return serviceDescriptor;
        }

        internal static object GetService(Type serviceType)
        {
            return GetServiceDescriptor(serviceType).Instance;
        }
    }
}