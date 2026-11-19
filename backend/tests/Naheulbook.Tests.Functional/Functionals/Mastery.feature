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
