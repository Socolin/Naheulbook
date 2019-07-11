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

  Scenario: Can load group monsters
    Given a JWT for a user
    Given a group
    And an item template
    And a monster with an item in its inventory

    When performing a GET to the url "/api/v2/groups/${Group.Id}/monsters" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Monster.Id},
        "name": "${Monster.Name}",
        "data": ${Monster.Data},
        "modifiers": [],
        "items": [
          { "__partial": {
            "id": ${Item.Id},
            "template": { "__partial": {
                "id": ${ItemTemplate.Id}
            }}
          }}
        ]
      }
    ]
    """

  Scenario: Can load group dead monsters
    Given a JWT for a user
    Given a group
    And a dead monster

    When performing a GET to the url "/api/v2/groups/${Group.Id}/deadMonsters?startIndex=0&count=1" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Monster.Id},
        "dead": "2042-08-06T12:23:24",
        "name": "${Monster.Name}",
        "data": ${Monster.Data},
        "modifiers": [],
        "items": []
      }
    ]
    """

  Scenario: Can add a modifier on a monster
    Given a JWT for a user
    Given a group
    And a monster

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/modifiers" with the following json content and the current jwt
    """
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
      "permanent": false
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": 1,
      "name": "some-modifier-name",
      "reusable": false,
      "durationType": "combat",
      "combatCount": 2,
      "currentCombatCount": 2,
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
    """

  Scenario: Can delete a modifier on a monster
    Given a JWT for a user
    Given a group
    And a monster with a modifier active for 2 laps

    When performing a DELETE to the url "/api/v2/monsters/${Monster.Id}/modifiers/8" with the current jwt
    Then the response status code is 204

  Scenario: Can delete a monster
    Given a JWT for a user
    Given a group
    And a monster

    When performing a DELETE to the url "/api/v2/monsters/${Monster.Id}/" with the current jwt
    Then the response status code is 204

  Scenario: Can kill a monster
    Given a JWT for a user
    Given a group
    And a monster

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/kill" with the following json content and the current jwt
    """
    {}
    """
    Then the response status code is 204

  Scenario: Add an item to a monster inventory
    Given a JWT for a user
    Given a group
    Given an item template with all optional fields set
    And a monster

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/items" with the following json content and the current jwt
    """
    {
      "itemTemplateId": ${ItemTemplate.Id},
      "itemData": {
        "some-key": "some-value"
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__match": {"type": "integer"}},
        "data": {
            "some-key": "some-value"
        },
        "modifiers": [],
        "template": {
            "id": ${ItemTemplate.Id},
            "name": "${ItemTemplate.Name}",
            "techName": "${ItemTemplate.TechName}",
            "source": "official",
            "categoryId": ${ItemTemplateCategory.Id},
            "data": {
                "key": "value"
            },
            "slots": [
                {
                    "id": ${Slot.[-1].Id},
                    "name": "${Slot.[-1].Name}",
                    "techName": "${Slot.[-1].TechName}"
                }
            ],
            "modifiers": [
                {
                    "stat": "${Stat.Name}",
                    "value": -2,
                    "job": ${Job.Id},
                    "origin": ${Origin.Id},
                    "special": [],
                    "type": "ADD"
                }
            ],
            "requirements": [
                {
                    "stat": "${Stat.Name}",
                    "min": 2,
                    "max": 12
                }
            ],
            "skillModifiers": [
                {
                    "skill": ${Skill.[-3].Id},
                    "value": 2
                }
            ],
            "skills": [
                {
                    "id":  ${Skill.[-1].Id}
                }
            ],
            "unskills": [
                {
                    "id":  ${Skill.[-2].Id}
                }
            ]
        }
    }
    """