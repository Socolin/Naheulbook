Feature: Entropy

  Scenario: Listing mankdebol calamities and debilibeuk blessings
    Given a mankdebol calamity
    And a debilibeuk blessing

    When performing a GET to the url "/api/v2/entropyTables"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "mankdebolCalamities": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${MankdebolCalamity.Id}",
          "diceRoll": ${MankdebolCalamity.DiceRoll},
          "name": "${MankdebolCalamity.Name}",
          "effect": "${MankdebolCalamity.Effect}"
        }
      ]}},
      "debilibeukBlessings": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${DebilibeukBlessing.Id}",
          "diceRoll": ${DebilibeukBlessing.DiceRoll},
          "name": "${DebilibeukBlessing.Name}",
          "effect": "${DebilibeukBlessing.Effect}"
        }
      ]}}
    }
    """

  Scenario: The GM can apply a mankdebol calamity to the group
    Given a JWT for a user
    And a group
    And that the group has 7 mankdebol points
    And a character
    And that the character is a member of the group
    And a mankdebol calamity

    When performing a POST to the url "/api/v2/groups/${Group.Id}/mankdebolCalamities" with the following json content and the current jwt
    """json
    {
      "eventId": "${MankdebolCalamity.Id}",
      "characterId": ${Character.Id}
    }
    """
    Then the response status code is 204

    When performing a GET to the url "/api/v2/groups/${Group.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {"__partial": {
      "data": {"mankdebol": 2}
    }}
    """

    When performing a GET to the url "/api/v2/groups/${Group.Id}/history?page=0" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    [
      {
        "id": {"__match": {"type": "integer"}},
        "date": {"__match": {"type": "string"}},
        "action": "MANKDEBOL_CALAMITY",
        "gm": true,
        "isGroup": true,
        "data": {
          "event": {
            "id": "${MankdebolCalamity.Id}",
            "diceRoll": ${MankdebolCalamity.DiceRoll},
            "name": "${MankdebolCalamity.Name}",
            "effect": "${MankdebolCalamity.Effect}"
          },
          "character": {
            "id": ${Character.Id},
            "name": "${Character.Name}"
          },
          "oldValue": 7,
          "newValue": 2
        }
      }
    ]
    """

  Scenario: Applying a blessing to a character outside of the group fails
    Given a JWT for a user
    And a group
    And a character
    And a debilibeuk blessing

    When performing a POST to the url "/api/v2/groups/${Group.Id}/debilibeukBlessings" with the following json content and the current jwt
    """json
    {
      "eventId": "${DebilibeukBlessing.Id}",
      "characterId": ${Character.Id}
    }
    """
    Then the response status code is 400
