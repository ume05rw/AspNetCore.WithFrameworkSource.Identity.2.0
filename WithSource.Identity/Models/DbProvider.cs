using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WithSource.Identity.Models
{
    public static class DbProvider
    {
        private const string SqliteFileName = "authNoneEf.sqlite3";

        private static readonly string DbPath = System.IO.Path.Combine(
            System.Environment.CurrentDirectory, 
            DbProvider.SqliteFileName
        );

        public static event EventHandler DbReady;

        private static Xb.Db.Sqlite _db;

        public static Xb.Db.Sqlite Db
        {
            get
            {
                if (DbProvider._db != null)
                    return DbProvider._db;

                DbProvider.Init();

                return DbProvider._db;
            }
        }

        public static bool IsDbReady => (DbProvider._db != null);

        private static bool _initOnce = false;
        public static void Init()
        {
            if (DbProvider._initOnce)
                return;
            DbProvider._initOnce = true;

            if (!System.IO.File.Exists(DbProvider.DbPath))
                DbProvider.CreateDb();

            DbProvider._db = new Xb.Db.Sqlite(DbProvider.DbPath);

            DbProvider.DbReady?.Invoke(null, new EventArgs());
        }

        private static void CreateDb()
        {
            var tableModels = new AppDbModel[]
            {
                (new UserStore()),
                (new RoleStore()),
            };
            var db = new Xb.Db.Sqlite(DbProvider.DbPath);

            foreach (var model in tableModels)
                model.FormatDb(db);

            db.Dispose();
        }
    }
}
