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
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class CharactersControllerTest
    {
        private ICharacterService _characterService;
        private IMapper _mapper;
        private NaheulbookExecutionContext _executionContext;

        private CharactersController _controller;

        [SetUp]
        public void SetUp()
        {
            _characterService = Substitute.For<ICharacterService>();
            _mapper = Substitute.For<IMapper>();

            _controller = new CharactersController(
                _characterService,
                _mapper
            );

            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task GetCharactersListAsync_ShouldLoadListOfCharacterFromService_ThenMapItIntoSummary()
        {
            var characters = new List<Character>();
            var charactersResponse = new List<CharacterSummaryResponse>();

            _characterService.GetCharacterListAsync(_executionContext)
                .Returns(characters);
            _mapper.Map<List<CharacterSummaryResponse>>(characters)
                .Returns(charactersResponse);

            var result = await _controller.GetCharactersListAsync(_executionContext);

            result.Value.Should().BeSameAs(charactersResponse);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var character = new Character();
            var characterResponse = new CharacterResponse();

            _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
                .Returns(character);
            _mapper.Map<CharacterResponse>(character)
                .Returns(characterResponse);

            var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            result.Value.Should().BeSameAs(characterResponse);
        }

        [Test]
        public void GetCharacterDetailsAsync_WhenCatchForbiddenAccessException_Return403()
        {
            const int characterId = 2;
            _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<Character>(new ForbiddenAccessException()));

            Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public void GetCharacterDetailsAsync_WhenCatchCharacterNotFoundException_Return404()
        {
            const int characterId = 2;
            _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<Character>(new CharacterNotFoundException(characterId)));

            Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task PostCreateCharacterAsync_ShouldCreateCharacterWithCharacterService_ThenReturnCharacterResponse()
        {
            var createCharacterRequest = new CreateCharacterRequest();
            var createdCharacter = new Character();
            var characterResponse = new CreateCharacterResponse();

            _characterService.CreateCharacterAsync(_executionContext, createCharacterRequest)
                .Returns(createdCharacter);
            _mapper.Map<CreateCharacterResponse>(createdCharacter)
                .Returns(characterResponse);

            var result = await _controller.PostCreateCharacterAsync(_executionContext, createCharacterRequest);

            result.Value.Should().BeSameAs(characterResponse);
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task PostAddItemToCharacterInventory_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var item = new Item();
            var itemResponse = new ItemResponse();
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(_executionContext, characterId, request)
                .Returns(item);
            _mapper.Map<ItemResponse>(item)
                .Returns(itemResponse);

            var result = await _controller.PostAddItemToCharacterInventory(_executionContext, characterId, request);

            result.Value.Should().BeSameAs(itemResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void PostAddItemToCharacterInventory_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int characterId = 2;
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(Task.FromException<Item>(exception));

            Func<Task> act = () => _controller.PostAddItemToCharacterInventory(_executionContext, characterId, request);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public void PostAddItemToCharacterInventory_WhenCatchItemTemplateNotFound_Return400()
        {
            const int characterId = 2;
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(Task.FromException<Item>(new ItemTemplateNotFoundException(3)));

            Func<Task> act = () => _controller.PostAddItemToCharacterInventory(_executionContext, characterId, request);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetCharacterLoots_ShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var loots = new List<Loot>();
            var lootsResponse = new List<LootResponse>();

            _characterService.GetCharacterLootsAsync(_executionContext, characterId)
                .Returns(loots);
            _mapper.Map<List<LootResponse>>(loots)
                .Returns(lootsResponse);

            var result = await _controller.GetCharacterLoots(_executionContext, characterId);

            result.Value.Should().BeSameAs(lootsResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void GetCharacterLoots_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.GetCharacterLootsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<List<Loot>>(exception));

            Func<Task> act = () => _controller.GetCharacterLoots(_executionContext, 2);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void PostAddModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.AddModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<AddCharacterModifierRequest>())
                .Returns(Task.FromException<CharacterModifier>(exception));

            Func<Task> act = () => _controller.PostAddModifiersAsync(_executionContext, 2, new AddCharacterModifierRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterModifierExceptionsAndExpectedStatusCode))]
        public void DeleteModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.DeleteModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterModifier>(exception));

            Func<Task> act = () => _controller.DeleteModifiersAsync(_executionContext, 2, 4);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetToggleCharacterModifierExceptionsAndExpectedStatusCode))]
        public void PostToggleModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.ToggleModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterModifier>(exception));

            Func<Task> act = () => _controller.PostToggleModifiersAsync(_executionContext, 2, 4);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetCharacterHistoryEntryAsyncShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var historyEntries = new List<IHistoryEntry>();
            var historyEntriesResponse = new List<HistoryEntryResponse>();

            _characterService.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8)
                .Returns(historyEntries);
            _mapper.Map<List<HistoryEntryResponse>>(historyEntries)
                .Returns(historyEntriesResponse);

            var result = await _controller.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8);

            result.Value.Should().BeSameAs(historyEntriesResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void GetCharacterHistoryEntryAsyncShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.GetCharacterHistoryEntryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<List<IHistoryEntry>>(exception));

            Func<Task> act = () => _controller.GetCharacterHistoryEntryAsync(_executionContext, 42, 0);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }


        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void PatchCharacterStatsAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.UpdateCharacterStatAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PatchCharacterStatsRequest>())
                .Returns(Task.FromException<List<Loot>>(exception));

            Func<Task> act = () => _controller.PatchCharacterStatsAsync(_executionContext, 2, new PatchCharacterStatsRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public void PutStatBonusAdAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            _characterService.SetCharacterAdBonusStatAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PutStatBonusAdRequest>())
                .Returns(Task.FromException<List<Loot>>(exception));

            Func<Task> act = () => _controller.PutStatBonusAdAsync(_executionContext, 2, new PutStatBonusAdRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> GetCommonCharacterExceptionsAndExpectedStatusCode()
        {
            yield return new TestCaseData(new ForbiddenAccessException(), HttpStatusCode.Forbidden);
            yield return new TestCaseData(new CharacterNotFoundException(42), HttpStatusCode.NotFound);
        }

        private static IEnumerable<TestCaseData> GetCommonCharacterModifierExceptionsAndExpectedStatusCode()
        {
            foreach (var testCaseData in GetCommonCharacterExceptionsAndExpectedStatusCode())
                yield return testCaseData;
            yield return new TestCaseData(new CharacterModifierNotFoundException(42), HttpStatusCode.NotFound);
        }

        private static IEnumerable<TestCaseData> GetToggleCharacterModifierExceptionsAndExpectedStatusCode()
        {
            foreach (var testCaseData in GetCommonCharacterModifierExceptionsAndExpectedStatusCode())
                yield return testCaseData;
            yield return new TestCaseData(new CharacterModifierNotReusableException(42), HttpStatusCode.BadRequest);
        }
    }
}