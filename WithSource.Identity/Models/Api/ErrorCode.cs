using System;
using System.Collections.Generic;
using System.Text;

namespace WithSource.Identity.Models.Api
{
    public enum ErrorCode
    {
        /// <summary>
        /// 想定外のエラー
        /// </summary>
        UnexpectedError = 0,

        /// <summary>
        /// JSON文字列解釈不能エラー
        /// </summary>
        JsonParseFailure = 101,

        /// <summary>
        /// 渡し値に項目が存在しない
        /// </summary>
        ItemNotFound = 201,

        /// <summary>
        /// 渡し値項目の値が空
        /// </summary>
        EmptyValue = 202,

        /// <summary>
        /// 値が等価でない
        /// </summary>
        ValueNotSame = 203,

        /// <summary>
        /// ロックされている
        /// </summary>
        Lockouted = 301,

        /// <summary>
        /// 許可されていない
        /// </summary>
        NotAllowed = 302,

        /// <summary>
        /// 2ファクタ認証が必要
        /// </summary>
        RequiresTwoFactor = 303,
    }
}
