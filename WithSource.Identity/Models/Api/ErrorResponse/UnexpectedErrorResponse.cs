using System;
using System.Collections.Generic;
using System.Text;

namespace WithSource.Identity.Models.Api.ErrorResponse
{
    public class UnexpectedErrorResponse : Response
    {
        public UnexpectedErrorResponse(Exception ex)
        {
            var errs = Xb.Util.GetErrorString(ex);
            var errString = Xb.Util.GetHighlighted(errs);
            this.Succeeded = false;
            this.Errors.Add(new Error()
            {
                Item = "",
                Code = ErrorCode.UnexpectedError,
                Message = $"Unexpected error: \r\n{errString}"
            });
        }
    }
}
