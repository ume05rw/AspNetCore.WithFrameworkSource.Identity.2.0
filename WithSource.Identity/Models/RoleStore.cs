using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Xb.Db;

namespace WithSource.Identity.Models
{
    public class RoleStore : AppDbModel, IRoleStore<Role>
    {
        #region "AppDbModel Implements"
        public override string TableName => "Roles";

        protected override Dictionary<string, string> DbColumnDefinitions => new Dictionary<string, string>()
        {
            { "Id" , "TEXT NOT NULL" },
            { "Name" , "TEXT NOT NULL" },
            { "NormalizedRoleName" , "TEXT NOT NULL" },
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
        /// ロールからDBレコードを生成する。
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private Xb.Db.ResultRow GetRow(Role role = null)
        {
            var row = this.DbModel.NewRow();
            if (role == null)
                return row;

            row["Id"] = role.RoleId;
            row["Name"] = role.Name;
            row["NormalizedRoleName"] = role.NormalizedRoleName;

            return row;
        }

        /// <summary>
        /// DBレコードからロールを生成する。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Role GetRole(Xb.Db.ResultRow row)
        {
            var role = new Role();
            role.Id = Guid.Parse(row.Get<string>("Id"));
            role.Name = row.Get<string>("Name");
            role.NormalizedRoleName = row.Get<string>("NormalizedRoleName");

            return role;
        }

        /// <summary>
        /// ロール追加
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var row = this.GetRow(role);
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
        /// ロール更新
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var row = this.GetRow(role);
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
        /// ロール削除
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var row = this.GetRow(role);
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
        /// ロールをIDで検索する。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException(nameof(roleId));

            var row = this.DbModel.Find(roleId);
            if (row == null)
                return Task.FromResult<Role>(null);

            var user = this.GetRole(row);

            return Task.FromResult(user);
        }

        /// <summary>
        /// ロールを名前で検索する。
        /// </summary>
        /// <param name="normalizedRoleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var where = $"NormalizedLoginName = { this.Db.Quote(normalizedRoleName) }";
            var table = this.DbModel.FindAll(where);
            if (table.RowCount <= 0)
                return Task.FromResult<Role>(null);

            var user = this.GetRole(table.Rows[0]);

            return Task.FromResult(user);
        }

        /// <summary>
        /// ロールの平坦化名称を取得する。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedRoleName);
        }

        /// <summary>
        /// ロールの平坦化名称をセットする。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedRoleName = normalizedName;

            //DO NOT Write DB. Update role-object field only.

            return Task.CompletedTask;
        }

        /// <summary>
        /// ロールIDを取得する。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.RoleId);
        }

        /// <summary>
        /// ロール名を取得する。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }


        /// <summary>
        /// ロール名をセットする。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            //DO NOT Write DB. Update role-object field only.

            return Task.CompletedTask;
        }
    }
}
