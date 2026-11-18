Feature: SpecialAttack

  Scenario: Listing special attack groups with their special attacks
    Given a special attack group
    And a special attack

    When performing a GET to the url "/api/v2/specialAttackGroups"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "specialAttackGroups": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${SpecialAttackGroup.Id}",
          "name": "${SpecialAttackGroup.Name}",
          "description": "${SpecialAttackGroup.Description}",
          "specialAttacks": [
            {
              "id": "${SpecialAttack.Id}",
              "name": "${SpecialAttack.Name}",
              "data": {
                "requiredLevel": 3,
                "usableBy": "some-usable-by",
                "effects": "some-effects",
                "xp": 2
              }
            }
          ]
        }
      ]}}
    }
    """

  Scenario: A character can learn a special attack, only once
    Given a special attack group
    And a special attack
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/specialAttacks" with the following json content and the current jwt
    """json
    {
      "specialAttackId": "${SpecialAttack.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "specialAttack": {
        "id": "${SpecialAttack.Id}",
        "name": "${SpecialAttack.Name}",
        "data": {
          "requiredLevel": 3,
          "usableBy": "some-usable-by",
          "effects": "some-effects",
          "xp": 2
        }
      },
      "data": {
        "successCount": 0,
        "cooldowns": []
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/specialAttacks" with the following json content and the current jwt
    """json
    {
      "specialAttackId": "${SpecialAttack.Id}"
    }
    """
    Then the response status code is 400

  Scenario: Using a special attack starts its cooldown, gives its XP on success, and it cannot be used again until the cooldown is over
    Given a special attack group
    And a special attack usable once per hour, with a 30 minutes cooldown on failure, giving 2 XP
    And a JWT for a user
    And a character
    And that the character knows the special attack

    When performing a POST to the url "/api/v2/characters/${Character.Id}/specialAttacks/${SpecialAttack.Id}/use" with the following json content and the current jwt
    """json
    {
      "success": true
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "specialAttack": {"__partial": {"id": "${SpecialAttack.Id}"}},
      "data": {
        "successCount": 1,
        "cooldowns": [
          {"durationType": "time", "timeDuration": 3600, "currentTimeDuration": 3600, "failure": false}
        ]
      },
      "experience": 44
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/specialAttacks/${SpecialAttack.Id}/use" with the following json content and the current jwt
    """json
    {
      "success": true
    }
    """
    Then the response status code is 400

    When performing a PUT to the url "/api/v2/characters/${Character.Id}/specialAttacks/${SpecialAttack.Id}" with the following json content and the current jwt
    """json
    {
      "resetCooldowns": true
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "specialAttack": {"__partial": {"id": "${SpecialAttack.Id}"}},
      "data": {
        "successCount": 1,
        "cooldowns": []
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/specialAttacks/${SpecialAttack.Id}/use" with the following json content and the current jwt
    """json
    {
      "success": false
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "specialAttack": {"__partial": {"id": "${SpecialAttack.Id}"}},
      "data": {
        "successCount": 1,
        "cooldowns": [
          {"durationType": "time", "timeDuration": 1800, "currentTimeDuration": 1800, "failure": true}
        ]
      },
      "experience": 44
    }
    """

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/specialAttacks/${SpecialAttack.Id}" with the current jwt
    Then the response status code is 204

  Scenario: The group durations update the special attacks cooldowns, and remove the expired ones
    Given a JWT for a user
    And a group
    And a character
    And that the character is a member of the group
    And a special attack group
    And a special attack usable once per hour, with a 30 minutes cooldown on failure, giving 2 XP
    And that the character knows the special attack, with a running cooldown

    When performing a POST to the url "/api/v2/groups/${Group.Id}/updateDurations" with the following json content and the current jwt
    """json
    [
      {
        "characterId": "!{Character.Id}",
        "changes": [
          {
            "type": "specialAttack",
            "specialAttackId": "${SpecialAttack.Id}",
            "cooldowns": [
              {"durationType": "time", "timeDuration": 3600, "currentTimeDuration": 0, "failure": false}
            ]
          }
        ]
      }
    ]
    """
    Then the response status code is 204

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "__partial": {
        "specialAttacks": [
          {
            "specialAttack": {"__partial": {"id": "${SpecialAttack.Id}"}},
            "data": {
              "successCount": 1,
              "cooldowns": []
            }
          }
        ]
      }
    }
    """
