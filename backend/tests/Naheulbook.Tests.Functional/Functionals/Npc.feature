Feature: Npc

  Scenario: Can create a Npc
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/npcs" with the following json content and the current jwt
    """json
    {
      "name": "some-npc-name",
      "data": {
        "location": "some-location",
        "note": "some-note"
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-npc-name",
      "data": {
        "location": "some-location",
        "note": "some-note"
      }
    }
    """

  Scenario: Can load npcs
    Given a JWT for a user
    Given a group
    And a npc

    When performing a GET to the url "/api/v2/groups/${Group.Id}/npcs" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Npc.Id},
        "name": "${Npc.Name}",
        "data": ${Npc.Data}
      }
    ]
    """

  Scenario: Can edit npc
    Given a JWT for a user
    Given a group
    And a npc

    When performing a PUT to the url "/api/v2/npcs/${Npc.Id}" with the following json content and the current jwt
    """json
    {
      "name": "new-name",
      "data": {
        "note": "new-note",
        "sex": "new-sex",
        "location": "new-location",
        "originName": "new-originName"
      }
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Npc.Id}",
      "name": "new-name",
      "data": {
        "note": "new-note",
        "sex": "new-sex",
        "location": "new-location",
        "originName": "new-originName"
      }
    }
    """