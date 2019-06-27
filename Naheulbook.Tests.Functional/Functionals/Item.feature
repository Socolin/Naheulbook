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

  Scenario: Can update item modifiers

    Given a JWT for a user
    And a character
    And an item template
    And an item based on that item template in the character inventory

    When performing a PUT to the url "/api/v2/items/${Item.Id}/modifiers" with the following json content and the current jwt
    """
    [
      {
        "id": 1,
        "reusable": false,
        "durationType": "combat",
        "values": [],
        "combatCount": 1,
        "name": "a",
        "active": true,
        "permanent": false,
        "currentCombatCount": 1
      }
    ]
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Item.Id},
      "data": ${Item.Data},
      "modifiers":  [
        {
          "id": 1,
          "reusable": false,
          "durationType": "combat",
          "values": [],
          "combatCount": 1,
          "name": "a",
          "active": true,
          "permanent": false,
          "currentCombatCount": 1
        }
      ]
    }
    """

  Scenario: Can equip an item
    Given a JWT for a user
    And a character
    And an item template
    And an item based on that item template in the character inventory

    When performing a POST to the url "/api/v2/items/${Item.Id}/equip" with the following json content and the current jwt
    """
    {
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Item.Id},
      "modifiers": ${Item.Modifiers},
      "data": {"__partial": {
        "equiped": 1
      }},
    }
    """
