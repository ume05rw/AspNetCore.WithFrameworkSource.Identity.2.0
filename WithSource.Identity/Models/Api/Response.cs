using System;
using System.Collections.Generic;
using System.Text;

namespace WithSource.Identity.Models.Api
{
    public class Response
    {
        /// <summary>
        /// 処理の成否
        /// </summary>
        public bool Succeeded { get; set; } = false;

        /// <summary>
        /// エラーデータ
        /// </summary>
        public List<Error> Errors { get; set; } = new List<Error>();

        /// <summary>
        /// 応答データ(オプション)
        /// </summary>
        public object Result { get; set; } = null;
    }
}
