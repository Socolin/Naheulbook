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

  Scenario: A character can get a talent, only once, and lose it
    Given a talent group
    And a talent
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/talents" with the following json content and the current jwt
    """json
    {
      "talentId": "${Talent.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "${Talent.Id}",
      "name": "${Talent.Name}",
      "data": {
        "restriction": "some-restriction",
        "effect": "some-effect",
        "requiredLevel": 5
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/talents" with the following json content and the current jwt
    """json
    {
      "talentId": "${Talent.Id}"
    }
    """
    Then the response status code is 400

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/talents/${Talent.Id}" with the current jwt
    Then the response status code is 204

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/talents/${Talent.Id}" with the current jwt
    Then the response status code is 404
