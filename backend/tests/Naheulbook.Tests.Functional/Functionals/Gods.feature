Feature: God

  Scenario: List gods
    Given a god

    When performing a GET to the url "/api/v2/gods"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {"__partialArray": {"key": "id", "array": [
        {
          "id": "!{God.Id}",
          "displayName": "${God.DisplayName}",
          "description": "${God.Description}",
          "techName": "${God.TechName}"
        }
    ]}}
    """