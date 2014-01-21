using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Excimer
{
    public static class Env
    {
        public static bool RunningOnMono = (Type.GetType("Mono.Runtime") != null);

        public static bool IsDebugConfig = false;

        public static object Get(Type type, bool returnDefaultIfNotRegistered = false)
        {
            if (instances.ContainsKey(type)) return instances[type];
            
            if (returnDefaultIfNotRegistered)  return null;

            if (type.IsInterface)
            {
                var defaultClassName = type.Namespace + "." + type.Name.Substring(1) + ", " + type.Assembly.FullName;
                var defaultClass = Type.GetType(defaultClassName);

                if (defaultClass != null)
                {
                    Env.Log(type.Name + " automatically bound to " + defaultClassName);
                    var instance = Get(defaultClass);
                    Bind(type, instance);
                    return instance;
                }

                throw new Exception(type.Name + " has no registered bindings and no corresponding class ('" + defaultClassName + "' was found)");
            }

            var constructors = type.GetConstructors();

            if (!constructors.Any()) throw new Exception(type.Name + " has no public constructors");

            if (constructors.Count() > 1) throw new Exception(type.Name + " has more than one public constructor");

            var parameters = constructors.Single().GetParameters();
            var o = Activator.CreateInstance(type, parameters.Select(parameter => Get(parameter.ParameterType)).ToArray(), null);
            return o;
        }

        public static T Get<T>(bool returnDefaultIfNotRegistered = false)
        {
            return (T)Get(typeof(T), returnDefaultIfNotRegistered);
        }

        public static void Bind(Type interfaceType, object instance)
        {
            instances[interfaceType] = instance;
        }

        public static void Bind<T>(T instance)
        {
            Bind(typeof(T), instance);
        }

        public static void Bind<T1, T2>(bool singleton = true) where T2 : T1
        {
            if (!singleton) throw new Exception("NOT SINGLETON");

            Bind<T1>(Get<T2>());
        }

        private static Dictionary<Type, object> instances = new Dictionary<Type, object>();

        public static SynchronizationContext MainSynchronizationContext { get; set; }

        public static void ExecuteSynchronized(Action action)
        {
            MainSynchronizationContext.Send((o) => action(), null);
        }

        private static ILogger logger;

        public static void Log(string message)
        {
            if (logger == null)
                logger = Env.Get<ILogger>(true);

            if (logger != null)
                logger.Log(DateTime.Now.ToShortTimeString() + " [" + Thread.CurrentThread.ManagedThreadId + "] " + message);
        }
    }
}
