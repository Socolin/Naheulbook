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

  Scenario: Can change item container
    Given a JWT for a user
    And a character
    And an item template
    And an item based on that item template in the character inventory
    And an item based on that item template in the character inventory

    When performing a PUT to the url "/api/v2/items/${Item.[1].Id}/container" with the following json content and the current jwt
    """
    {
      "containerId": ${Item.[0].Id}
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Item.[1].Id},
      "modifiers": ${Item.[1].Modifiers},
      "data": ${Item.[1].Data},
      "containerId": ${Item.[0].Id}
    }
    """

  Scenario: Can delete an item
    Given a JWT for a user
    And a character
    And an item template
    And an item based on that item template in the character inventory

    When performing a DELETE to the url "/api/v2/items/${Item.Id}/" with the current jwt
    Then the response status code is 204


  Scenario: A character can take an item from a loot
    Given a JWT for a user
    And a character
    And a group
    And that the character is a member of the group
    And a loot
    And an item in the loot


    When performing a POST to the url "/api/v2/items/${Item.Id}/take" with the following json content and the current jwt
    """
    {
      "characterId": ${Character.Id}
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "remainingQuantity": 0,
      "takenItem": {"__partial": {
        "id": ${Item.Id}
      }}
    }
    """

  Scenario: A character can give an item to another character
    Given a JWT for a user
    And a group
    And a character
    And that the character is a member of the group
    And a character
    And that the character is a member of the group
    And an item in the character inventory

    When performing a POST to the url "/api/v2/items/${Item.Id}/give" with the following json content and the current jwt
    """
    {
      "characterId": ${Character.[0].Id}
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "remainingQuantity": 0
    }
    """