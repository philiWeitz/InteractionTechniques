using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Datamodel.Util
{
    internal class DatabaseUtil
    {
        private static DatabaseUtil instance;
        private ISession session;

        public ISession Session
        {
            get { return this.session; }
        }

        private DatabaseUtil()
        {
            initialize();
        }

        private void initialize()
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            cfg.Configure();

            cfg.AddAssembly(typeof(Model.AbstractEntity).Assembly);

            // Get ourselves an NHibernate Session
            var sessions = cfg.BuildSessionFactory();
            session = sessions.OpenSession();

            // Create the database schema
            new SchemaUpdate(cfg).Execute(false, true);
        }

        ~DatabaseUtil()
        {
            session.Dispose();
            session.Close();
        }

        public static DatabaseUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseUtil();
                }
                return instance;
            }
        }
    }
}