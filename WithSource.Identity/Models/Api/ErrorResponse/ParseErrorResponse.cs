using System;
using System.Collections.Generic;
using System.Text;

namespace WithSource.Identity.Models.Api.ErrorResponse
{
    public class ParseErrorResponse : Response
    {
        public ParseErrorResponse(string jsonString)
        {
            this.Succeeded = false;
            this.Errors.Add(new Error()
            {
                Item = "",
                Code = ErrorCode.JsonParseFailure,
                Message = $"Parse error: {jsonString}"
            });
        }
    }
}
