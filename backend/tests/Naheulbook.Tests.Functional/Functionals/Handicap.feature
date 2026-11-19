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

  Scenario: A character can get a handicap, only once, and lose it
    Given a handicap
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/handicaps" with the following json content and the current jwt
    """json
    {
      "handicapId": "${Handicap.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "${Handicap.Id}",
      "diceRoll": ${Handicap.DiceRoll},
      "name": "${Handicap.Name}",
      "data": {
        "condition": "some-condition",
        "effect": "some-effect"
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/handicaps" with the following json content and the current jwt
    """json
    {
      "handicapId": "${Handicap.Id}"
    }
    """
    Then the response status code is 400

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/handicaps/${Handicap.Id}" with the current jwt
    Then the response status code is 204

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/handicaps/${Handicap.Id}" with the current jwt
    Then the response status code is 404
