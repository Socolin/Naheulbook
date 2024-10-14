Feature: Fight Preparation

    Scenario: Can load group fights

    Scenario: A GM can prepare a fight with monters
        Given a JWT for a user
        Given a group

        When performing a POST to the url "/api/v2/groups/${Group.Id}/fights" with the following json content and the current jwt
        """json
        {
            "name": "some-fight"
        }
        """
        Then the response status code is 201
        And the response should contains the following json
        """
        [
          {
            "id": {"__capture": {"name": "FightId"}}
            "name": "some-fight"
          }
        ]
        """

        When performing a POST to the url "/api/v2/groups/${Group.Id}/fights/${FightId}/monsters" with the following json content and the current jwt
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

    Scenario: Can delete a group fight
        Given a JWT for a user
        Given a group
        And a prepared fight

        When performing a DELETE to the url "/api/v2/groups/${Group.Id}/events/${Event.Id}" with the current jwt
        Then the response status code is 204