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
