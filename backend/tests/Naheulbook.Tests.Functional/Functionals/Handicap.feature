Feature: Handicap

  Scenario: Listing handicaps
    Given a handicap

    When performing a GET to the url "/api/v2/handicaps"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "handicaps": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${Handicap.Id}",
          "diceRoll": ${Handicap.DiceRoll},
          "name": "${Handicap.Name}",
          "data": {
            "condition": "some-condition",
            "effect": "some-effect"
          }
        }
      ]}}
    }
    """
