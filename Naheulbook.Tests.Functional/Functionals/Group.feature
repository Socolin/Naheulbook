Feature: Group

  Scenario: Create a group
    Given a JWT for a user
    Given a location

    When performing a POST to the url "/api/v2/groups/" with the following json content and the current jwt
    """
    {
      "name": "some-group-name",
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-group-name",
      "data": {},
      "invitedCharacterIds": [],
      "invitesCharacterIds": [],
      "characterIds": [],
      "location": { "__partial": {
        "id": 1
      }}
    }
    """

  Scenario: List groups owne by a user
    Given a JWT for a user
    Given a group

    When performing a GET to the url "/api/v2/groups/" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Group.Id},
        "name": "${Group.Name}",
        "characterCount": 0
      }
    ]
    """

  Scenario: Can get a group details
    Given a JWT for a user
    Given a group
    And 3 character
    And an invite from the group to the 1st character
    And a request from 2nd character to join the group
    And that the 3rd character is a member of the group
    And a loot
    And that the loot is the current group combat loot

    When performing a GET to the url "/api/v2/groups/${Group.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Group.Id},
      "name": "${Group.Name}",
      "data": {},
      "location": {
        "data": ${Location.Data},
        "id": ${Location.Id},
        "name": "${Location.Name}",
        "parent": 0
      },
      "characterIds": [
        ${Character.[2].Id}
      ],
      "invitedCharacterIds": [
        ${Character.[0].Id}
      ],
      "invitesCharacterIds": [
        ${Character.[1].Id}
      ]
    }
    """

  Scenario: Load group history
    Given a JWT for a user
    Given a group
    And a group history entry

    When performing a GET to the url "/api/v2/groups/${Group.Id}/history?page=0" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${GroupHistoryEntry.Id},
        "date": "2020-10-05T05:07:08",
        "action": "${GroupHistoryEntry.Action}",
        "info": "${GroupHistoryEntry.Info}",
        "gm": ${GroupHistoryEntry.Gm},
        "action": "${GroupHistoryEntry.Action}",
        "isGroup": true,
        "data": ${GroupHistoryEntry.Data}
      }
    ]
    """

  Scenario: Can search for character to invite
    Given a JWT for a user
    Given a group
    Given a character

    When performing a GET to the url "/api/v2/characters/search?filter=${Character.Name}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
      """
      [
        {
          "id": ${Character.Id},
          "name": "${Character.Name}",
          "origin": "${Character.Origin.Name}",
          "owner": {"__match": {"type": "string"}}
         }
      ]
      """

  Scenario: Can invite a character to a group
    Given a JWT for a user
    Given a group
    Given a job
    Given a character

    When performing a POST to the url "/api/v2/groups/${Group.Id}/invites/" with the following json content and the current jwt
    """
    {
      "characterId": ${Character.Id},
      "fromGroup": true
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": ${Character.Id},
      "name": "${Character.Name}",
      "origin": "${Character.Origin.Name}",
      "jobs": [
        "${Job.Name}"
      ],
      "groupId": ${Group.Id},
      "groupName": "${Group.Name}",
      "fromGroup": true
    }
    """

  Scenario: Can cancel or reject an invite to a group
    Given a JWT for a user
    Given a group
    Given a character
    And an invite from the group to the 1st character

    When performing a DELETE to the url "/api/v2/groups/${Group.Id}/invites/${Character.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "groupId": ${Group.Id},
      "characterId": ${Character.Id},
      "fromGroup": true
    }
    """

