using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Joywire
{
    public class ThirdParties
    {
        private static Dictionary<System.Type, object> elements;

        static ThirdParties()
        {
            elements = new Dictionary<System.Type, object>();
        }

        public static void Register<T>(object o)
        {
            var type = typeof(T);
            elements[type] = o;
        }

        public static void Unregister<T>()
        {
            elements.Remove(typeof(T));
        }

        public static T Get<T>()
        {
            var type = typeof(T);

            if(elements.TryGetValue(type, out var o))
            {
                return (T)o;
            }
            return default(T);
        }

        public static bool Find<T>(out T val)
        {
            if(elements.TryGetValue(typeof(T),out var o))
            {
                val = (T)o;
                return true;
            }
            val = default;
            return false;
        }
    }
}
