using StructureMap;
using System;
using System.Threading;

namespace IoC
{
    public static class IocContainer
    {
        private static readonly Lazy<Container> ContainerBuilder = new Lazy<Container>(() => new Container(), LazyThreadSafetyMode.ExecutionAndPublication);
        public static IContainer Container = ContainerBuilder.Value;

        public static void Initialize(Registry registry, bool nestedContainer = false)
        {
            Container.Configure(x =>
            {
                x.AddRegistry(registry);
            });
            if (nestedContainer)
            {
                Container = Container.GetNestedContainer();
            }
        }

        public static void Initialize<T>() where T : Registry, new()
        {
            Container.Configure(x =>
            {
                x.AddRegistry<T>();
            });
        }
        public static IDisposable GetContainerPerRequest()
        {
            return Container.GetNestedContainer();
        }
        public static T Get<T>()
        {
            return Container.GetInstance<T>();
        }

        public static T Get<T>(string instanceKey)
        {
            return Container.GetInstance<T>(instanceKey);
        }

        public static object Get(Type t)
        {
            return Container.GetInstance(t);
        }

        public static void Add<TType>(TType obj) where TType : class
        {
            Container.Inject(obj);
        }
    }
}
