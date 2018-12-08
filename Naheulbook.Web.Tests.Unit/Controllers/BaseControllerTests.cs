using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class BaseControllerTests
    {
        protected NaheulbookExecutionContext ExecutionContext;
        protected HttpContext HttpContext;

        protected void MockHttpContext(Controller controller)
        {
            HttpContext = Substitute.For<HttpContext>();
            ExecutionContext = new NaheulbookExecutionContext();
            HttpContext.Items["NaheulbookExecutionContext"].Returns(ExecutionContext);
            controller.ControllerContext.HttpContext = HttpContext;
        }
    }
}