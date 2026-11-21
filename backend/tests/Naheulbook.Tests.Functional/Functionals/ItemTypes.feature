Feature: ItemType

  Scenario: List item types
    Given a item type

    When performing a GET to the url "/api/v2/itemTypes"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {"__partialArray": {"key": "id", "array": [
        {
          "id": "!{ItemType.Id}",
          "displayName": "${ItemType.DisplayName}",
          "techName": "${ItemType.TechName}"
        }
    ]}}
    """