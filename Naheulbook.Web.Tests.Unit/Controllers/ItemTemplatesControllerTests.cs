using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class ItemTemplatesControllerTests
    {
        private IItemTemplateService _itemTemplateService;
        private IMapper _mapper;
        private ItemTemplatesController _itemTemplatesController;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _itemTemplateService = Substitute.For<IItemTemplateService>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplatesController = new ItemTemplatesController(_itemTemplateService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateItemTemplate_CallItemService()
        {
            var createItemTemplateRequest = new CreateItemTemplateRequest();
            var itemTemplate = new ItemTemplate();
            var itemTemplateResponse = new ItemTemplateResponse();

            _itemTemplateService.CreateItemTemplateAsync(_executionContext, createItemTemplateRequest)
                .Returns(itemTemplate);
            _mapper.Map<ItemTemplateResponse>(itemTemplate)
                .Returns(itemTemplateResponse);

            var result = await _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, createItemTemplateRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(itemTemplateResponse);
        }

        [Test]
        public void PostCreateItemTemplate_WhenCatchForbiddenAccessException_Return403()
        {
            _itemTemplateService.CreateItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateItemTemplateRequest>())
                .Returns(Task.FromException<ItemTemplate>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, new CreateItemTemplateRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}