using StructureMap;
using System;

namespace IoC.Init
{
    public static class IocInitializer
    {
        public static void Init(Registry registry = null, bool nestedContainer = false)
        {
            registry = registry ?? new BaseRegistry();
            IocContainer.Initialize(registry, nestedContainer);
        }
    }
}
