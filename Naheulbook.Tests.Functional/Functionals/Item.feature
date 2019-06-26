Feature: Item

  Scenario: Can edit item data

    Given a JWT for a user
    And a character
    And an item template
    And an item based on that item template in the character inventory

    When performing a PUT to the url "/api/v2/items/${Item.Id}/data" with the following json content and the current jwt
    """
    {
      "name": "some-new-name",
      "description": "some-new-description"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Item.Id},
      "modifiers": ${Item.Modifiers},
      "data": {
        "name": "some-new-name",
        "description": "some-new-description"
      }
    }
    """
