Feature: Group

  Scenario: Create a group
    Given a JWT for a user

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
      "invites": [],
      "characterIds": [],
      "config": {
        "allowPlayersToAddObject": true,
        "allowPlayersToSeeGemPriceWhenIdentified": false,
        "allowPlayersToSeeSkillGmDetails": false,
        "autoIncrementMonsterColor": true,
        "autoIncrementMonsterNumber": true
      }
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
    And 3 characters
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
      "characterIds": [
        ${Character.[2].Id}
      ],
      "config": {
        "allowPlayersToAddObject": true,
        "allowPlayersToSeeGemPriceWhenIdentified": false,
        "allowPlayersToSeeSkillGmDetails": false,
        "autoIncrementMonsterColor": true,
        "autoIncrementMonsterNumber": true
      },
      "invites": [
        {
          "id": ${Character.[0].Id},
          "name": "${Character.[0].Name}",
          "origin": "${Character.[0].Origin.Name}",
          "jobs": [],
          "fromGroup": true
        },
        {
          "id": ${Character.[1].Id},
          "name": "${Character.[1].Name}",
          "origin": "${Character.[1].Origin.Name}",
          "jobs": [],
          "fromGroup": false
        }
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
    Given that the owner of the character allow to appear in searches

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
      "group": {
        "id": ${Group.Id},
        "name": "${Group.Name}",
        "config": {
          "allowPlayersToAddObject": true,
          "allowPlayersToSeeGemPriceWhenIdentified": false,
          "allowPlayersToSeeSkillGmDetails": false,
          "autoIncrementMonsterColor": true,
          "autoIncrementMonsterNumber": true
        }
      },
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

  Scenario: Can accept an invite
    Given a JWT for a user
    Given a group
    Given a character
    And an invite from the group to the 1st character

    When performing a POST to the url "/api/v2/groups/${Group.Id}/invites/${Character.Id}/accept" with the following json content and the current jwt
    """
    {}
    """
    Then the response status code is 204

  Scenario: Can edit group properties
    Given a JWT for a user
    Given a group

    When performing a PATCH to the url "/api/v2/groups/${Group.Id}/" with the following json content and the current jwt
    """
    {
      "name": "new-name",
      "mankdebol": 4,
      "debilibeuk": 2,
      "date": {
        "year": 45,
        "day": 84,
        "hour": 4,
        "minutes": 8
      }
    }
    """
    Then the response status code is 204

  Scenario: Can start/stop combat
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/startCombat" with the following json content and the current jwt
    """
    {
    }
    """
    Then the response status code is 204

    When performing a POST to the url "/api/v2/groups/${Group.Id}/endCombat" with the following json content and the current jwt
    """
    {
    }
    """
    Then the response status code is 204


  Scenario: Can update item/character/monster modifiers durations and  item lifetime durations
    Given a JWT for a user
    Given a group
    And a character
    And an active reusable character modifier active for 2 lap
    And an active non-reusable character modifier active for 1 lap
    And that the character is a member of the group
    And a monster with a modifier active for 2 lap

    When performing a POST to the url "/api/v2/groups/${Group.Id}/updateDurations" with the following json content and the current jwt
    """
    [
      {
        "monsterId": ${Monster.Id},
        "changes": [
          {
            "type": "modifier",
            "modifier": {
              "id": 8,
              "active": true,
              "currentLapCount": 1
            }
          }
        ]
      },
      {
        "characterId": ${Character.Id},
        "changes": [
          {
            "type": "modifier",
            "modifier": {
              "id": ${Character.Modifiers.[0].Id},
              "active": true,
              "currentLapCount": 1
            }
          }
        ]
      }
    ]
    """
    Then the response status code is 204

  Scenario: A character can get the list of active characters in the group
    Given a JWT for a user
    Given a group
    And a character
    And that the character is a member of the group

    When performing a GET to the url "/api/v2/groups/${Group.Id}/activeCharacters" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Character.Id},
        "name": "${Character.Name}",
        "isNpc": ${Character.IsNpc}
      }
    ]
    """

  Scenario: Can advance group date
    Given a JWT for a user
    Given a group
    And that the group have a date set to the 5th day of the year 1459 at 8:42

    When performing a POST to the url "/api/v2/groups/${Group.Id}/addTime" with the following json content and the current jwt
    """
    {
      "minute":32,
      "hour":18,
      "day":5,
      "week":2,
      "year":1
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "minute":14,
      "hour":3,
      "day":25,
      "year":1460
    }
    """

  Scenario: Can add group log
    Given a JWT for a user
    Given a group
    When performing a POST to the url "/api/v2/groups/${Group.Id}/history" with the following json content and the current jwt
    """
    {
        isGm: true,
        info: "some-log-info"
    }
    """
    Then the response status code is 204

  Scenario: Can change group configuration
    Given a JWT for a user
    Given a group
    When performing a PATCH to the url "/api/v2/groups/${Group.Id}/config" with the following json content and the current jwt
    """
    {
      "allowPlayersToSeeSkillGmDetails": true,
      "allowPlayersToAddObject": true,
      "allowPlayersToSeeGemPriceWhenIdentified": true
    }
    """
    Then the response status code is 204
