Feature: Monster

  Scenario: Create a monster
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/monsters" with the following json content and the current jwt
    """
    {
        "name": "some-monster-name",
        "data": {
            "key": "value"
        },
        "items": [
            {
                "data": {
                    "name": "some-item-name",
                    "notIdentified": true
                },
                "itemTemplateId": 1
            }
        ],
        "modifiers": [
            {
                "name": "some-modifier-name",
                "reusable": false,
                "durationType": "combat",
                "combatCount": 2,
                "description": "some-modifier-description",
                "type": "some-modifier-type",
                "values": [
                    {
                        "type": "ADD",
                        "stat": "FO",
                        "value": 12,
                        "special": [
                            "something"
                        ]
                    }
                ],
                "active": true,
                "permanent": false
            }
        ]
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-monster-name",
      "data": {
        "key": "value"
      },
      items: [],
      modifiers: [
        {
          "id": 1,
          "name": "some-modifier-name",
          "reusable": false,
          "durationType": "combat",
          "combatCount": 2,
          "description": "some-modifier-description",
          "type": "some-modifier-type",
          "values": [
            {
              "type": "ADD",
              "stat": "FO",
              "value": 12,
              "special": [
                "something"
              ]
            }
          ],
          "active": true,
          "permanent": false
        }
      ]
    }
    """
