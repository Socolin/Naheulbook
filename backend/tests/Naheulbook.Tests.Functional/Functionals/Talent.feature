Feature: Talent

  Scenario: Listing talent groups with their talents
    Given a talent group
    And a talent

    When performing a GET to the url "/api/v2/talentGroups"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "talentGroups": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${TalentGroup.Id}",
          "name": "${TalentGroup.Name}",
          "description": "${TalentGroup.Description}",
          "talents": [
            {
              "id": "${Talent.Id}",
              "name": "${Talent.Name}",
              "data": {
                "restriction": "some-restriction",
                "effect": "some-effect",
                "requiredLevel": 5
              }
            }
          ]
        }
      ]}}
    }
    """
