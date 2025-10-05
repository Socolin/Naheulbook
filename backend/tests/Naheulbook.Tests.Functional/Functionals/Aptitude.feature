Feature: Aptitude

  Scenario: Listing aptitude groups
    Given an aptitude group
    And an aptitude

    When performing a GET to the url "/api/v2/aptitudeGroups"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "aptitudeGroups": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${AptitudeGroup.Id}",
          "name": "${AptitudeGroup.Name}"
        }
      ]}}
    }
    """

  Scenario: Can get an aptitude group with all the aptitudes
    Given an aptitude group
    And an aptitude

    When performing a GET to the url "/api/v2/aptitudeGroups/${AptitudeGroup.Id}"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "${AptitudeGroup.Id}",
      "name": "${AptitudeGroup.Name}",
      "aptitudes": [
        {
          "id": "${Aptitude.Id}",
          "name": "${Aptitude.Name}",
          "roll": "!{Aptitude.Roll}",
          "type": "${Aptitude.Type}",
          "description": "${Aptitude.Description}",
          "effect": "${Aptitude.Effect}"
        }
      ]
    }
    """

  Scenario: A character can obtain new aptitude
    Given an aptitude group
    And an aptitude

    Given a JWT for a user
    And a character
    And a job

    When performing a POST to the url "/api/v2/characters/${Character.Id}/aptitudes" with the following json content and the current jwt
    """json
    {
      "aptitudeId": "${Aptitude.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "active": false,
      "count": 1,
      "aptitude": {
        "id": "${Aptitude.Id}",
        "name": "${Aptitude.Name}",
        "roll": "!{Aptitude.Roll}",
        "type": "${Aptitude.Type}",
        "description": "${Aptitude.Description}",
        "effect": "${Aptitude.Effect}"
      }
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/aptitudes" with the following json content and the current jwt
    """json
    {
      "aptitudeId": "${Aptitude.Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "active": false,
      "count": 2,
      "aptitude": {
        "id": "${Aptitude.Id}",
        "name": "${Aptitude.Name}",
        "roll": "!{Aptitude.Roll}",
        "type": "${Aptitude.Type}",
        "description": "${Aptitude.Description}",
        "effect": "${Aptitude.Effect}"
      }
    }
    """

    When performing a PUT to the url "/api/v2/characters/${Character.Id}/aptitudes/${Aptitude.Id}" with the following json content and the current jwt
    """json
    {
      "active": true
    }
    """
    Then the response status code is 204

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/aptitudes/${Aptitude.Id}" with the current jwt
    Then the response status code is 204