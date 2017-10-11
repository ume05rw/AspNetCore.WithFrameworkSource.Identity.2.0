using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithSource.Identity.Models
{
    public abstract class AppDbModel : IDisposable
    {
        /// <summary>
        /// SQLiteコネクション管理オブジェクト
        /// </summary>
        public Xb.Db.Sqlite Db { get; protected set; }

        /// <summary>
        /// モデル対象テーブルのXb.Db.Modelオブジェクト
        /// </summary>
        public Xb.Db.Model DbModel { get; protected set; }

        /// <summary>
        /// テーブル名
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// DBカラム名/カラム定義文字列配列
        /// </summary>
        /// <remarks>
        ///   Key: ColumnName   ex) "ServerId"
        /// Value: Property     ex) "INTEGER NOT NULL DEFAULT 1"
        /// </remarks>
        protected abstract Dictionary<string, string> DbColumnDefinitions { get; }

        /// <summary>
        /// DBテーブルの付加的定義文字列
        /// </summary>
        /// <remarks>
        /// ex) "PRIMARY KEY (Id)"
        /// </remarks>
        protected abstract List<string> DbAdditionalDefinitions { get; }


        protected AppDbModel()
        {
            if (DbProvider.IsDbReady)
            {
                this.Db = DbProvider.Db;
                this.DbModel = this.Db.Models[this.TableName];
            }
            else
            {
                DbProvider.Init();
                DbProvider.DbReady += (sender, e) => 
                {
                    this.Db = DbProvider.Db;
                    this.DbModel = this.Db.Models[this.TableName];
                };
            }
        }


        /// <summary>
        /// 渡し値DBカラムの追加SQL文字列を取得する。
        /// </summary>
        /// <returns></returns>
        protected string GetCreateSql()
        {
            var defs = this.DbColumnDefinitions.Select(p => $"{p.Key} {p.Value}").ToList();
            defs.AddRange(this.DbAdditionalDefinitions);

            var sql = new StringBuilder();
            sql.AppendLine($" CREATE TABLE {this.TableName} ( ");
            sql.Append(string.Join("\r\n , ", defs));
            sql.Append($" ); ");

            return sql.ToString();
        }

        /// <summary>
        /// 渡し値DBカラムの追加SQL文字列を取得する。
        /// </summary>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        protected string GetAddColumnSql(string columnKey)
        {
            if (!this.DbColumnDefinitions.ContainsKey(columnKey))
                throw new ArgumentOutOfRangeException(columnKey, $"Not found on DbColumnDefinitions");

            var columnDef = this.DbColumnDefinitions[columnKey];

            var sql = new StringBuilder();
            sql.AppendLine($" ALTER TABLE {this.TableName} ");
            sql.AppendLine($"   ADD {columnKey} {columnDef}; ");

            return sql.ToString();
        }
        

        /// <summary>
        /// DB構造・データの検証と補正を行う。
        /// </summary>
        /// <returns></returns>
        public void FormatDb(Xb.Db.Sqlite db)
        {
            try
            {
                this.FormatDbTable(db);
                this.FormatDbInitData(db);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DBテーブル・カラムの構造チェックと補正を行う。
        /// </summary>
        /// <param name="db"></param>
        private void FormatDbTable(Xb.Db.Sqlite db)
        {
            try
            {
                var sql = new System.Text.StringBuilder();

                //1)テーブル存在チェック
                sql.Clear();
                sql.Append($" SELECT name as TABLE_NAME ");
                sql.Append($" FROM sqlite_master ");
                sql.Append($" WHERE type = 'table' ");
                sql.Append($"     AND name = {db.Quote(this.TableName)} ");
                var rtDb = db.Query(sql.ToString());

                if (rtDb.RowCount <= 0)
                {
                    //テーブルが存在しないとき
                    db.Execute(this.GetCreateSql());
                }
                else
                {
                    //テーブルが存在するとき
                    var rtSt = db.Query($"PRAGMA table_info({db.Quote(this.TableName)}); ");
                    var rowNames = rtSt.Rows.Select(r => r.Get<string>("name"));

                    foreach (var pair in this.DbColumnDefinitions)
                        if (rowNames.All(n => n != pair.Key))
                            db.Execute(this.GetAddColumnSql(pair.Key));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DBの初期データ存在チェックと補正を行う。
        /// </summary>
        /// <param name="db"></param>
        protected abstract void FormatDbInitData(Xb.Db.Sqlite db);

        /// <summary>
        /// DBテーブルを破棄する。
        /// </summary>
        /// <param name="db"></param>
        public void DropDb(Xb.Db.Sqlite db)
        {
            try
            {
                db.Execute($"DROP TABLE {this.TableName}; ");
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this._isDisposed)
                throw new NullReferenceException("object already disposed");
        }

        #region IDisposable Support
        private bool _isDisposed = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                }
                _isDisposed = true;
            }
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion
    }
}
