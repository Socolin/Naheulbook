Feature: Loot

  Scenario: Create a loot
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/loots" with the following json content and the current jwt
    """
    {
      "name": "some-loot-name",
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "visibleForPlayer": false,
      "name": "some-loot-name",
      "items": [],
      "monsters": []
    }
    """

  Scenario: List group loots
    Given a JWT for a user
    Given a group
    And a loot

    When performing a GET to the url "/api/v2/groups/${Group.Id}/loots" with the current jwt

    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Loot.Id},
        "visibleForPlayer": false,
        "name": "${Loot.Name}",
        "items": [],
        "monsters": []
      }
    ]
    """

  Scenario: Can change loot visibility
    Given a JWT for a user
    Given a group
    And a loot

    When performing a PUT to the url "/api/v2/loots/${Loot.Id}/visibility" with the following json content and the current jwt
    """
    {
      "visibleForPlayer": true
    }
    """
    Then the response status code is 204

  Scenario: Can delete a loot
    Given a JWT for a user
    Given a group
    And a loot

    When performing a DELETE to the url "/api/v2/loots/${Loot.Id}" with the current jwt
    Then the response status code is 204

  Scenario: Add an item to a loot
    Given a JWT for a user
    Given a group
    Given an item template with all optional fields set
    And a loot

    When performing a POST to the url "/api/v2/loots/${Loot.Id}/items" with the following json content and the current jwt
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