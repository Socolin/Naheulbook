using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
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
        public void PutEditItemDataAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _itemService.UpdateItemDataAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<ItemData>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PutEditItemDataAsync(_executionContext, 2, new ItemData());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonItemExceptionsAndExpectedStatusCode))]
        public void PutEditItemModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _itemService.UpdateItemModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<List<ActiveStatsModifier>>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PutEditItemModifiersAsync(_executionContext, 2, new List<ActiveStatsModifier>());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonItemExceptionsAndExpectedStatusCode))]
        public void PostEquipItemAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _itemService.EquipItemAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<EquipItemRequest>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PostEquipItemAsync(_executionContext, 2, new EquipItemRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonItemExceptionsAndExpectedStatusCode))]
        public void PutChangeItemContainerAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _itemService.ChangeItemContainerAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<ChangeItemContainerRequest>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PutChangeItemContainerAsync(_executionContext, 2, new ChangeItemContainerRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> GetCommonItemExceptionsAndExpectedStatusCode()
        {
            yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
            yield return new TestCaseData(new ItemNotFoundException(42), StatusCodes.Status404NotFound);
        }
    }
}