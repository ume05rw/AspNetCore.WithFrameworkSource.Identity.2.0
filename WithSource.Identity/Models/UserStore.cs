using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Xb.Db;

namespace WithSource.Identity.Models
{
    public class UserStore : AppDbModel, IUserStore<User>, IUserPasswordStore<User>
    {
        #region "AppDbModel Implements"
        public override string TableName => "Users";

        protected override Dictionary<string, string> DbColumnDefinitions => new Dictionary<string, string>()
        {
            { "Id" , "TEXT NOT NULL" },
            { "LoginName" , "TEXT NOT NULL" },
            { "NormalizedLoginName" , "TEXT NOT NULL" },
            { "ScreenName" , "TEXT" },
            { "Email" , "TEXT" },
            { "PasswordHash" , "TEXT" },
        };

        protected override List<string> DbAdditionalDefinitions => new List<string>()
        {
            "PRIMARY KEY (Id)"
        };

        protected override void FormatDbInitData(Sqlite db)
        {
            //No init data.
        }
        #endregion "AppDbModel Implements"

        private static IdentityErrorDescriber IdentityErrorDescriber
            = new IdentityErrorDescriber();

        /// <summary>
        /// ユーザーからDBレコードを生成する。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private Xb.Db.ResultRow GetRow(User user = null)
        {
            var row = this.DbModel.NewRow();
            if (user == null)
                return row;

            row["Id"] = user.UserId;
            row["LoginName"] = user.LoginName;
            row["NormalizedLoginName"] = user.NormalizedLoginName;
            row["ScreenName"] = user.ScreenName;
            row["Email"] = user.Email;
            row["PasswordHash"] = user.PasswordHash;

            return row;
        }

        /// <summary>
        /// DBレコードからユーザーを生成する。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private User GetUser(Xb.Db.ResultRow row)
        {
            var user = new User();
            user.Id = Guid.Parse(row.Get<string>("Id"));
            user.LoginName = row.Get<string>("LoginName");
            user.NormalizedLoginName = row.Get<string>("NormalizedLoginName");
            user.ScreenName = row.Get<string>("ScreenName");
            user.Email = row.Get<string>("Email");
            user.PasswordHash = row.Get<string>("PasswordHash");

            return user;
        }

        /// <summary>
        /// ユーザー追加
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var row = this.GetRow(user);
            var result = this.DbModel.Write(row);

            if (result.Length > 0)
            {
                return Task.FromResult(
                    IdentityResult.Failed(IdentityErrorDescriber.ConcurrencyFailure())
                );
            }

            return Task.FromResult(IdentityResult.Success);
        }

        /// <summary>
        /// ユーザー更新
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var row = this.GetRow(user);
            var result = this.DbModel.Write(row);

            if (result.Length > 0)
            {
                return Task.FromResult(
                    IdentityResult.Failed(IdentityErrorDescriber.ConcurrencyFailure())
                );
            }

            return Task.FromResult(IdentityResult.Success);
        }

        /// <summary>
        /// ユーザー削除
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var row = this.GetRow(user);
            var result = this.DbModel.Delete(row);

            if (result.Length > 0)
            {
                return Task.FromResult(
                    IdentityResult.Failed(IdentityErrorDescriber.ConcurrencyFailure())
                );
            }

            return Task.FromResult(IdentityResult.Success);
        }

        /// <summary>
        /// ユーザーをIDで検索する。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var row = this.DbModel.Find(userId);
            if (row == null)
                return Task.FromResult<User>(null);

            var user = this.GetUser(row);

            return Task.FromResult(user);
        }

        /// <summary>
        /// ユーザーを名前で検索する。
        /// </summary>
        /// <param name="normalizedUserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            var where = $"NormalizedLoginName = { this.Db.Quote(normalizedUserName) }";
            var table = this.DbModel.FindAll(where);
            if (table.RowCount <= 0)
                return Task.FromResult<User>(null);

            var user = this.GetUser(table.Rows[0]);

            return Task.FromResult(user);
        }

        /// <summary>
        /// ユーザーの平坦化名称を取得する。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedLoginName);
        }

        /// <summary>
        /// ユーザーの平坦化名称をセットする。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedLoginName = normalizedName;

            //DO NOT Write DB. Update user-object field only.

            return Task.CompletedTask;
        }

        /// <summary>
        /// ユーザIDを取得する。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserId);
        }

        /// <summary>
        /// ユーザー名を取得する。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LoginName);
        }

        /// <summary>
        /// ユーザー名をセットする。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LoginName = userName;

            //DO NOT Write DB. Update user-object field only.

            return Task.CompletedTask;
        }

        /// <summary>
        /// パスワードを保持しているか否か
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        /// パスワードハッシュを取得する。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// パスワードハッシュをセットする。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            //DO NOT Write DB. Update user-object field only.

            return Task.CompletedTask;
        }
    }
}
