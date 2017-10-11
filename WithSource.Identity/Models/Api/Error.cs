using System;
using System.Collections.Generic;
using System.Text;

namespace WithSource.Identity.Models.Api
{
    public class Error
    {
        /// <summary>
        /// 項目名(=input要素のID)
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        public ErrorCode Code { get; set; }

        /// <summary>
        /// エラーコードに対応する定型文字列
        /// </summary>
        public string CodeString => this.Code.ToString();

        /// <summary>
        /// UI表示用の文字列
        /// </summary>
        public string Message { get; set; }
    }
}
