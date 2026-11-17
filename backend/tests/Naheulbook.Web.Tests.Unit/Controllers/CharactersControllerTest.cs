using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class CharactersControllerTest
{
    private ICharacterService _characterService;
    private IAptitudeMapper _aptitudeMapper;
    private ICharacterMapper _characterMapper;
    private ICharacterModifierMapper _characterModifierMapper;
    private IItemMapper _itemMapper;
    private ILootMapper _lootMapper;
    private NaheulbookExecutionContext _executionContext;

    private CharactersController _controller;
    private ICharacterBackupService _characterBackupService;
    private ICharacterAptitudeService _characterAptitudeService;

    [SetUp]
    public void SetUp()
    {
        _characterService = Substitute.For<ICharacterService>();
        _aptitudeMapper = Substitute.For<IAptitudeMapper>();
        _characterMapper = Substitute.For<ICharacterMapper>();
        _characterModifierMapper = Substitute.For<ICharacterModifierMapper>();
        _itemMapper = Substitute.For<IItemMapper>();
        _lootMapper = Substitute.For<ILootMapper>();
        _characterBackupService = Substitute.For<ICharacterBackupService>();
        _characterAptitudeService = Substitute.For<ICharacterAptitudeService>();

        _controller = new CharactersController(
            _characterService,
            _aptitudeMapper,
            _characterMapper,
            _characterModifierMapper,
            _itemMapper,
            _lootMapper,
            _characterBackupService,
            _characterAptitudeService
        );

        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task GetCharactersListAsync_ShouldLoadListOfCharacterFromService_ThenMapItIntoSummary()
    {
        var entity1 = new CharacterEntity();
        var entity2 = new CharacterEntity();
        var characters = new List<CharacterEntity> {entity1, entity2};
        var response1 = new CharacterSummaryResponse();
        var response2 = new CharacterSummaryResponse();

        _characterService.GetCharacterListAsync(_executionContext)
            .Returns(characters);
        _characterMapper.ToSummaryResponse(entity1)
            .Returns(response1);
        _characterMapper.ToSummaryResponse(entity2)
            .Returns(response2);

        var result = await _controller.GetCharactersListAsync(_executionContext, CancellationToken.None);

        result.Value.Should().Equal(response1, response2);
    }

    [Test]
    public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
    {
        const int characterId = 2;
        const int groupMasterId = 3;
        var character = new CharacterEntity {Group = new GroupEntity {MasterId = groupMasterId}};
        var characterResponse = new CharacterResponse();

        _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
            .Returns(character);
        _characterMapper.ToResponse(character)
            .Returns(characterResponse);

        var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId, CancellationToken.None);

        result.Value.Should().BeSameAs(characterResponse);
    }

    [Test]
    public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoCharacterResponse_WhenNoGroup()
    {
        const int characterId = 2;
        var character = new CharacterEntity();
        var characterResponse = new CharacterResponse();

        _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
            .Returns(character);
        _characterMapper.ToResponse(character)
            .Returns(characterResponse);

        var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId, CancellationToken.None);

        result.Value.Should().BeSameAs(characterResponse);
    }

    [Test]
    public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoCharacterForGmResponse()
    {
        const int characterId = 2;
        const int groupMasterId = 3;
        _executionContext.UserId = groupMasterId;
        var character = new CharacterEntity {Group = new GroupEntity {MasterId = groupMasterId}};
        var characterResponse = new CharacterFoGmResponse();

        _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
            .Returns(character);
        _characterMapper.ToGmResponse(character)
            .Returns(characterResponse);

        var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId, CancellationToken.None);

        result.Value.Should().BeSameAs(characterResponse);
    }

    [Test]
    public async Task GetCharacterDetailsAsync_WhenCatchForbiddenAccessException_Return403()
    {
        const int characterId = 2;
        _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
            .Returns(Task.FromException<CharacterEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Test]
    public async Task GetCharacterDetailsAsync_WhenCatchCharacterNotFoundException_Return404()
    {
        const int characterId = 2;
        _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
            .Returns(Task.FromException<CharacterEntity>(new CharacterNotFoundException(characterId)));

        Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task PostCreateCharacterAsync_ShouldCreateCharacterWithCharacterService_ThenReturnCharacterResponse()
    {
        var createCharacterRequest = new CreateCharacterRequest();
        var createdCharacter = new CharacterEntity();
        var characterResponse = new CreateCharacterResponse();

        _characterService.CreateCharacterAsync(_executionContext, createCharacterRequest)
            .Returns(createdCharacter);
        _characterMapper.ToCreateResponse(createdCharacter)
            .Returns(characterResponse);

        var result = await _controller.PostCreateCharacterAsync(_executionContext, createCharacterRequest, CancellationToken.None);

        result.Value.Should().BeSameAs(characterResponse);
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Test]
    public async Task PostAddItemToCharacterInventory_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
    {
        const int characterId = 2;
        var item = new ItemEntity();
        var itemResponse = new ItemResponse();
        var request = new CreateItemRequest();

        _characterService.AddItemToCharacterAsync(_executionContext, characterId, request)
            .Returns(item);
        _itemMapper.ToResponse(item)
            .Returns(itemResponse);

        var result = await _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request, CancellationToken.None);

        result.Value.Should().BeSameAs(itemResponse);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task PostAddItemToCharacterInventory_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int characterId = 2;
        var request = new CreateItemRequest();

        _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
            .Returns(Task.FromException<ItemEntity>(exception));

        Func<Task> act = () => _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task PostAddItemToCharacterInventory_WhenCatchItemTemplateNotFound_Return400()
    {
        const int characterId = 2;
        var itemTemplateId = Guid.NewGuid();
        var request = new CreateItemRequest();

        _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
            .Returns(Task.FromException<ItemEntity>(new ItemTemplateNotFoundException(itemTemplateId)));

        Func<Task> act = () => _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task GetCharacterLoots_ShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
    {
        const int characterId = 2;
        var entity1 = new LootEntity();
        var entity2 = new LootEntity();
        var loots = new List<LootEntity> {entity1, entity2};
        var response1 = new LootResponse();
        var response2 = new LootResponse();

        _characterService.GetCharacterLootsAsync(_executionContext, characterId)
            .Returns(loots);
        _lootMapper.ToResponse(entity1)
            .Returns(response1);
        _lootMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _controller.GetCharacterLootsAsync(_executionContext, characterId, CancellationToken.None);

        result.Value.Should().Equal(response1, response2);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task GetCharacterLoots_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.GetCharacterLootsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
            .Returns(Task.FromException<List<LootEntity>>(exception));

        Func<Task> act = () => _controller.GetCharacterLootsAsync(_executionContext, 2, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task PostAddModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        var request = new AddCharacterModifierRequest {Name = string.Empty, DurationType = string.Empty};

        _characterService.AddModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<AddCharacterModifierRequest>())
            .Returns(Task.FromException<CharacterModifierEntity>(exception));

        Func<Task> act = () => _controller.PostAddModifiersAsync(_executionContext, 2, request, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterModifierExceptionsAndExpectedStatusCode))]
    public async Task DeleteModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.DeleteModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<CharacterModifierEntity>(exception));

        Func<Task> act = () => _controller.DeleteModifiersAsync(_executionContext, 2, 4, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetToggleCharacterModifierExceptionsAndExpectedStatusCode))]
    public async Task PostToggleModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.ToggleModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<CharacterModifierEntity>(exception));

        Func<Task> act = () => _controller.PostToggleModifiersAsync(_executionContext, 2, 4, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task GetCharacterHistoryEntryAsyncShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
    {
        const int characterId = 2;
        IHistoryEntry entity1 = new CharacterHistoryEntryEntity();
        IHistoryEntry entity2 = new CharacterHistoryEntryEntity();
        var historyEntries = new List<IHistoryEntry> {entity1, entity2};
        IHistoryEntryResponse response1 = new CharacterHistoryEntryResponse();
        IHistoryEntryResponse response2 = new CharacterHistoryEntryResponse();

        _characterService.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8)
            .Returns(historyEntries);
        _characterMapper.ToResponse(entity1)
            .Returns(response1);
        _characterMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _controller.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8, CancellationToken.None);

        result.Value.Should().Equal(response1, response2);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task GetCharacterHistoryEntryAsyncShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.GetCharacterHistoryEntryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<List<IHistoryEntry>>(exception));

        Func<Task> act = () => _controller.GetCharacterHistoryEntryAsync(_executionContext, 42, 0, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }


    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task PatchCharacterAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.UpdateCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PatchCharacterRequest>())
            .Returns(Task.FromException<List<LootEntity>>(exception));

        Func<Task> act = () => _controller.PatchCharacterAsync(_executionContext, 2, new PatchCharacterRequest(), CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
    public async Task PutStatBonusAdAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _characterService.SetCharacterAdBonusStatAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PutStatBonusAdRequest>())
            .Returns(Task.FromException<List<LootEntity>>(exception));

        Func<Task> act = () => _controller.PutStatBonusAdAsync(_executionContext, 2, new PutStatBonusAdRequest {Stat = string.Empty}, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    private static IEnumerable<TestCaseData> GetCommonCharacterExceptionsAndExpectedStatusCode()
    {
        yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
        yield return new TestCaseData(new CharacterNotFoundException(42), StatusCodes.Status404NotFound);
    }

    private static IEnumerable<TestCaseData> GetCommonCharacterModifierExceptionsAndExpectedStatusCode()
    {
        foreach (var testCaseData in GetCommonCharacterExceptionsAndExpectedStatusCode())
            yield return testCaseData;
        yield return new TestCaseData(new CharacterModifierNotFoundException(42), StatusCodes.Status404NotFound);
    }

    private static IEnumerable<TestCaseData> GetToggleCharacterModifierExceptionsAndExpectedStatusCode()
    {
        foreach (var testCaseData in GetCommonCharacterModifierExceptionsAndExpectedStatusCode())
            yield return testCaseData;
        yield return new TestCaseData(new CharacterModifierNotReusableException(42), StatusCodes.Status400BadRequest);
    }
}