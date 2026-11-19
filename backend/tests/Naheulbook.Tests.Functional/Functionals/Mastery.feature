Feature: Mastery

  Scenario: Listing mastery groups with their masteries
    Given a mastery group
    And a mastery

    When performing a GET to the url "/api/v2/masteryGroups"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "masteryGroups": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${MasteryGroup.Id}",
          "name": "${MasteryGroup.Name}",
          "description": "${MasteryGroup.Description}",
          "masteries": [
            {
              "id": "${Mastery.Id}",
              "name": "${Mastery.Name}",
              "data": {
                "restriction": "some-restriction",
                "effect": "some-effect",
                "requiredLevel": 10
              }
            }
          ]
        }
      ]}}
    }
    """

  Scenario: A character can get a mastery, only once, and lose it
    Given a mastery group
    And a mastery
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/masteries" with the following json content and the current jwt
    """json
    {
      "masteryId": "${Mastery.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "${Mastery.Id}",
      "name": "${Mastery.Name}",
      "data": {
        "restriction": "some-restriction",
        "effect": "some-effect",
        "requiredLevel": 10
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/masteries" with the following json content and the current jwt
    """json
    {
      "masteryId": "${Mastery.Id}"
    }
    """
    Then the response status code is 400

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/masteries/${Mastery.Id}" with the current jwt
    Then the response status code is 204

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/masteries/${Mastery.Id}" with the current jwt
    Then the response status code is 404
