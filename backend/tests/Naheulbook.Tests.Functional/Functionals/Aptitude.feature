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