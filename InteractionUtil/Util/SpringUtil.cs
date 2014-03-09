using Spring.Context;

namespace InteractionUtil.Util
{
    public class SpringUtil
    {
        private static SpringUtil instance;
        private IApplicationContext ctx;

        public static T getService<T>()
        {
            return Instance.getSpringService<T>();
        }

        private T getSpringService<T>()
        {
            T service = (T)ctx.GetObject(typeof(T).Name);
            return service;
        }

        private SpringUtil()
        {
            ctx = Spring.Context.Support.ContextRegistry.GetContext();
        }

        private static SpringUtil Instance
        { 
            get {
                if (instance == null) {
                    instance = new SpringUtil();
                }    
                return instance;
            }
        }
    }
}
