using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Naheulbook.Web.ActionResults;

public class CreatedActionResult<TValue> : IConvertToActionResult
    where TValue : class
{
    public int StatusCode { get; }
    public TValue? Value { get; }
    private readonly ActionResult? _result;

    private CreatedActionResult(int statusCode, TValue value)
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
        return new CreatedActionResult<TValue>(StatusCodes.Status201Created, value);
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
            StatusCode = StatusCode,
        };
    }
}