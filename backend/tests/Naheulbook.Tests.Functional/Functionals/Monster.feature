Feature: Monster

  Scenario: Create a monster
    Given a JWT for a user
    Given a group
    Given an item template

    When performing a POST to the url "/api/v2/groups/${Group.Id}/monsters" with the following json content and the current jwt
    """json
    {
      "name": "some-monster-name",
      "data": {
        "at": 8,
        "chercheNoise": false,
        "color": null,
        "cou": 0,
        "dmg": null,
        "ea": 0,
        "esq": null,
        "ev": 0,
        "int": null,
        "maxEa": 0,
        "maxEv": 0,
        "note": null,
        "number": 0,
        "page": 0,
        "pr": 0,
        "pr_magic": 4,
        "prd": 10,
        "resm": 0,
        "sex": null,
        "xp": 0
      },
      "items": [
        {
          "itemData": {
            "name": "some-item-name",
            "notIdentified": true
          },
          "itemTemplateId": "${ItemTemplate.Id}"
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
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-monster-name",
      "data": {
        "at": 8,
        "chercheNoise": false,
        "cou": 0,
        "ea": 0,
        "ev": 0,
        "maxEa": 0,
        "maxEv": 0,
        "number": 0,
        "page": 0,
        "pr": 0,
        "pr_magic": 4,
        "prd": 10,
        "resm": 0,
        "xp": 0
      },
      "items": [
        {
          "data": {
            "name": "some-item-name",
            "notIdentified": true
          },
          "id": {"__match": {"type": "integer"}},
          "modifiers": [],
            "template": {"__partial": {
              "id": "${ItemTemplate.Id}"
            }}
          }
      ],
      "modifiers": [
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
        "id": "!{Monster.Id}",
        "name": "${Monster.Name}",
        "data": "!{Monster.Data}",
        "modifiers": [],
        "items": [
          { "__partial": {
            "id": "!{Item.Id}",
            "template": { "__partial": {
                "id": "${ItemTemplate.Id}"
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
        "id": "!{Monster.Id}",
        "dead": "2042-08-06T12:23:24",
        "name": "${Monster.Name}",
        "data": "!{Monster.Data}"
      }
    ]
    """

  Scenario: Can add a modifier on a monster
    Given a JWT for a user
    Given a group
    And a monster

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/modifiers" with the following json content and the current jwt
    """json
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
    """json
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
    """json
    {}
    """
    Then the response status code is 204

  Scenario: Add an item to a monster inventory
    Given a JWT for a user
    Given a group
    Given an item template with all optional fields set
    And a monster

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/items" with the following json content and the current jwt
    """json
    {
      "itemTemplateId": "${ItemTemplate.Id}",
      "itemData": {
          "name": "some-name"
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
        "id": {"__match": {"type": "integer"}},
        "data": {
            "name": "some-name"
        },
        "modifiers": [],
        "template": {
            "id": "${ItemTemplate.Id}",
            "name": "${ItemTemplate.Name}",
            "techName": "${ItemTemplate.TechName}",
            "source": "official",
            "subCategoryId": "!{ItemTemplateSubCategory.Id}",
            "data": {
                "key": "value"
            },
            "slots": [
                {
                    "id": "!{Slot.[-1].Id}",
                    "name": "${Slot.[-1].Name}",
                    "techName": "${Slot.[-1].TechName}"
                }
            ],
            "modifiers": [
                {
                    "stat": "${Stat.Name}",
                    "value": -2,
                    "jobId": "${Job.Id}",
                    "originId": "${Origin.Id}",
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
                    "skillId": "${Skill.[-3].Id}",
                    "value": 2
                }
            ],
            "skillIds": [
                "${Skill.[-1].Id}"
            ],
            "unSkillIds": [
                "${Skill.[-2].Id}"
            ]
        }
    }
    """

  Scenario: Can add a random item from a category to a monster
    Given a JWT for a user
    Given a group
    And a monster

    Given an item template

    When performing a POST to the url "/api/v2/monsters/${Monster.Id}/addRandomItem" with the following json content and the current jwt
    """json
    {
      "subCategoryTechName": "${ItemTemplateSubCategory.TechName}"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "data": {
        "name": "${ItemTemplate.Name}"
      },
      "modifiers": [],
      "template": {
        "id": "${ItemTemplate.Id}",
        "name": "${ItemTemplate.Name}",
        "techName": "${ItemTemplate.TechName}",
        "source": "official",
        "subCategoryId": "!{ItemTemplateSubCategory.Id}",
        "data": {
          "key": "value"
        },
        "slots": [
        ],
        "modifiers": [
        ],
        "requirements": [
        ],
        "skillModifiers": [
        ],
        "skillIds": [
        ],
        "unSkillIds": [
        ]
      }
    }
    """

  Scenario: Can update monster
    Given a JWT for a user
    Given a group
    And a monster
    And a character
    And that the character is a member of the group

    When performing a PUT to the url "/api/v2/monsters/${Monster.Id}/data" with the following json content and the current jwt
    """json
    {
      "at": 5,
      "prd": 8
    }
    """
    Then the response status code is 204


    When performing a PUT to the url "/api/v2/monsters/${Monster.Id}/target" with the following json content and the current jwt
    """json
    {
      "isMonster": false,
      "id": "!{Character.Id}"
    }
    """
    Then the response status code is 204


    When performing a PATCH to the url "/api/v2/monsters/${Monster.Id}/" with the following json content and the current jwt
    """json
    {
      "name": "some-new-name"
    }
    """
    Then the response status code is 204

  Scenario: Load a monster info
    Given a JWT for a user
    Given a group
    Given a stat

    And an item template
    And a monster with an item in its inventory and a modifier

    When performing a GET to the url "/api/v2/monsters/${Monster.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Monster.Id}",
      "name": "${Monster.Name}",
      "data": {
        "key": "value"
      },
      "modifiers": [
        {
          "active": true,
          "combatCount": 2,
          "currentCombatCount": 2,
          "durationType": "combat",
          "id": 8,
          "name": "some-name",
          "permanent": false,
          "reusable": false
        }
      ],
      "items": [
        {
          "id": "!{Item.Id}",
          "data": {
            "key": "value"
          },
          "modifiers": [
            {
              "active": true,
              "description": "some-description",
              "id": 0,
              "permanent": false,
              "reusable": false
            }
          ],
          "template": {
            "id": "${ItemTemplate.Id}",
            "name": "${ItemTemplate.Name}",
            "techName": "${ItemTemplate.TechName}",
            "source": "official",
            "subCategoryId": "!{ItemTemplateSubCategory.Id}",
            "data": {
              "key": "value"
            },
            "slots": [
            ],
            "modifiers": [
            ],
            "requirements": [
            ],
            "skillModifiers": [
            ],
            "skillIds": [
            ],
            "unSkillIds": [
            ]
          }
        }
      ]
    }
    """