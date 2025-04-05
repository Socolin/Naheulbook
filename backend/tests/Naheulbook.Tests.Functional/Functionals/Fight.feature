Feature: Fight Preparation

  Scenario: Can load group fights
    Given a JWT for a user
    Given a group
    And a prepared fight
    And a monster for the fight

    When performing a GET to the url "/api/v2/groups/${Group.Id}/fights" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    [
      {
        "id": "!{Fight.Id}",
        "name": "${Fight.Name}",
        "monsters": [
          {
            "id": "!{Monster.Id}",
            "name": "${Monster.Name}",
            "fightId": "!{Fight.Id}",
            "data": "!{Monster.Data}",
            "modifiers": [],
            "items": []
          }
        ]
      }
    ]
    """

  Scenario: A GM can prepare a fight with monters
    Given a JWT for a user
    Given an item template
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/fights" with the following json content and the current jwt
    """json
    {
        "name": "some-fight"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__capture": {"type": "integer", "name": "FightId"}},
      "name": "some-fight",
      "monsters": []
    }
    """

    When performing a POST to the url "/api/v2/groups/${Group.Id}/monsters" with the following json content and the current jwt
    """json
    {
      "name": "some-monster-name",
      "fightId": "!{FightId}",
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
      "fightId": "!{FightId}",
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

  Scenario: Can delete a group fight
    Given a JWT for a user
    Given a group
    And a prepared fight

    When performing a DELETE to the url "/api/v2/groups/${Group.Id}/fights/${Fight.Id}" with the current jwt
    Then the response status code is 204