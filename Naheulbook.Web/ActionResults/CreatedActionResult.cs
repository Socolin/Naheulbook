using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Naheulbook.Web.ActionResults
{
    public class CreatedActionResult<TValue> : IConvertToActionResult
    {
        public HttpStatusCode StatusCode { get; }
        public TValue Value { get; }
        private readonly ActionResult _result;

        private CreatedActionResult(HttpStatusCode statusCode, TValue value)
        {
            StatusCode = statusCode;
            Value = value;
        }

        private CreatedActionResult(ActionResult actionResult)
        {
            _result = actionResult;
        }

        public static implicit operator CreatedActionResult<TValue>(TValue value)
        {
            return new CreatedActionResult<TValue>(HttpStatusCode.Created, value);
        }

        public static implicit operator CreatedActionResult<TValue>(ActionResult result)
        {
            return new CreatedActionResult<TValue>(result);
        }

        IActionResult IConvertToActionResult.Convert()
        {
            var result = _result;
            if (result != null)
                return result;

            return new JsonResult(Value)
            {
                StatusCode = (int) StatusCode
            };
        }
    }
}