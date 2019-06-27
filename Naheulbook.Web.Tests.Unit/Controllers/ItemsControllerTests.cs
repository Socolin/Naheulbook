using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class ItemsControllerTests
    {
        private IItemService _itemService;
        private IMapper _mapper;
        private ItemsController _controller;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _itemService = Substitute.For<IItemService>();
            _mapper = Substitute.For<IMapper>();
            _controller = new ItemsController(_itemService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        [TestCaseSource(nameof(GetCommonItemExceptionsAndExpectedStatusCode))]
        public void PutEditItemDataAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _itemService.UpdateItemDataAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<JObject>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PutEditItemDataAsync(_executionContext, 2, new JObject());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonItemExceptionsAndExpectedStatusCode))]
        public void PutEditItemModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _itemService.UpdateItemModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<List<ActiveStatsModifier>>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PutEditItemModifiersAsync(_executionContext, 2, new List<ActiveStatsModifier>());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> GetCommonItemExceptionsAndExpectedStatusCode()
        {
            yield return new TestCaseData(new ForbiddenAccessException(), HttpStatusCode.Forbidden);
            yield return new TestCaseData(new ItemNotFoundException(42), HttpStatusCode.NotFound);
        }
    }
}