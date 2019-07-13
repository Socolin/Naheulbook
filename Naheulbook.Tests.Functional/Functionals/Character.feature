Feature: Character

  Scenario: Create a character
    Given a JWT for a user
    And a job
    And a skill
    And a speciality
    And an origin

    When performing a POST to the url "/api/v2/characters" with the following json content and the current jwt
    """
    {
      "name": "some-name",
      "stats": {
        "COU": 8,
        "INT": 9,
        "CHA": 10,
        "AD": 11,
        "FO": 12
      },
      "job": ${Job.Id},
      "origin": ${Origin.Id},
      "skills": [
        ${Skill.Id}
      ],
      "sex": "Homme",
      "money": 150,
      "modifiedStat": {
        "SOME_MODIFIER":{
            "name": "some-name",
            "stats": {
              "FO": 2,
              "INT": -1
            }
         }
      },
      "speciality": ${Speciality.Id},
      "fatePoint": 3
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}}
    }
    """

  Scenario: Load characters list
    Given a JWT for a user
    And a job
    And an origin
    And a character

    When performing a GET to the url "/api/v2/characters" with the current jwt

    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Character.Id},
        "jobs": [
          "${Job.Name}"
        ],
        "level": ${Character.Level},
        "name": "${Character.Name}",
        "origin": "${Origin.Name}",
        "originId": ${Origin.Id}
      }
    ]
    """

  Scenario: Load a character details
    Given a JWT for a user
    And a group

    Given a JWT for a user
    Given a character with all possible data
    And an item template with all optional fields set
    And an item based on that item template in the character inventory
    And an invite from the group to the 1st character

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt

    Then the response status code is 200
    And the response should contains the following json
    """
    {
        "id": ${Character.Id},
        "name": "${Character.Name}",
        "originId": ${Origin.Id},
        "level": ${Character.Level},
        "jobIds": [
            ${Job.[0].Id},
            ${Job.[1].Id}
        ],
        "stats": {
            "COU": ${Character.Cou},
            "AD": ${Character.Ad},
            "FO": ${Character.Fo},
            "INT": ${Character.Int},
            "CHA": ${Character.Cha}
        },
        "experience": ${Character.Experience},
        "sex": "Homme",
        "statBonusAD": "PRD",
        "fatePoint": ${Character.FatePoint},
        "ev": ${Character.Ev},
        "ea": ${Character.Ea},
        "isNpc": false,
        "modifiers": [
            {
                "id": ${Character.Modifiers.[0].Id},
                "name": "${Character.Modifiers.[0].Name}",
                "permanent": ${Character.Modifiers.[0].Permanent},
                "reusable": ${Character.Modifiers.[0].Reusable},
                "combatCount": ${Character.Modifiers.[0].CombatCount},
                "currentCombatCount": ${Character.Modifiers.[0].CurrentCombatCount},
                "description": "${Character.Modifiers.[0].Description}",
                "durationType": "${Character.Modifiers.[0].DurationType}",
                "active": ${Character.Modifiers.[0].IsActive},
                "values": [
                    {
                        "stat": "${Character.Modifiers.[0].Values.[0].StatName}",
                        "value": ${Character.Modifiers.[0].Values.[0].Value},
                        "type": "${Character.Modifiers.[0].Values.[0].Type}"
                    },
                    {
                        "stat": "${Character.Modifiers.[0].Values.[1].StatName}",
                        "value": ${Character.Modifiers.[0].Values.[1].Value},
                        "type": "${Character.Modifiers.[0].Values.[1].Type}"
                    }
                ]
            },
            {
                "id": ${Character.Modifiers.[1].Id},
                "name": "${Character.Modifiers.[1].Name}",
                "description": "${Character.Modifiers.[1].Description}",
                "permanent": ${Character.Modifiers.[1].Permanent},
                "reusable": ${Character.Modifiers.[1].Reusable},
                "active": ${Character.Modifiers.[1].IsActive},
                "values": [
                    {
                        "stat": "${Character.Modifiers.[1].Values.[0].StatName}",
                        "value": ${Character.Modifiers.[1].Values.[0].Value},
                        "type": "${Character.Modifiers.[1].Values.[0].Type}"
                    },
                    {
                        "stat": "${Character.Modifiers.[1].Values.[1].StatName}",
                        "value": ${Character.Modifiers.[1].Values.[1].Value},
                        "type": "${Character.Modifiers.[1].Values.[1].Type}"
                    }
                ]
            },
            {
                "id": ${Character.Modifiers.[2].Id},
                "name": "${Character.Modifiers.[2].Name}",
                "description": "${Character.Modifiers.[2].Description}",
                "permanent": ${Character.Modifiers.[2].Permanent},
                "reusable": ${Character.Modifiers.[2].Reusable},
                "active": ${Character.Modifiers.[2].IsActive},
                "durationType": "${Character.Modifiers.[2].DurationType}",
                "lapCount": ${Character.Modifiers.[2].LapCount},
                "currentLapCount": ${Character.Modifiers.[2].CurrentLapCount},
                "lapCountDecrement": ${Character.Modifiers.[2].LapCountDecrement},
                "values": []
            }
        ],
        "skillIds": [
            ${Skill.[0].Id},
            ${Skill.[1].Id}
        ],
        "specialities": [
            {
                "id": ${Speciality.Id},
                "name": "${Speciality.Name}",
                "description": "${Speciality.Description}",
                "modifiers": [],
                "specials": [],
                "flags": ${Speciality.Flags}
            }
        ],
        "invites": [
          { "__partial": {
              "groupId": ${Group.Id},
              "groupName": "${Group.Name}"
            }
          }
        ],
        "items": [
          {
            "id": ${Item.Id},
            "data": ${Item.Data},
            "modifiers": ${Item.Modifiers},
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
                  "stat": "${ItemTemplate.Modifiers.[0].StatName}",
                  "value": ${ItemTemplate.Modifiers.[0].Value},
                  "job": ${ItemTemplate.Modifiers.[0].RequireJobId},
                  "origin": ${ItemTemplate.Modifiers.[0].RequireOriginId},
                  "special": [],
                  "type": "ADD"
                }
              ],
              "requirements": [
                {
                  "stat": "${ItemTemplate.Requirements.[0].StatName}",
                  "min": ${ItemTemplate.Requirements.[0].MinValue},
                  "max": ${ItemTemplate.Requirements.[0].MaxValue}
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
        ],
        "group": {
            "id": ${Group.Id},
            "name": "${Group.Name}"
        }
    }
    """

  Scenario: Add an item to character inventory
    Given a JWT for a user
    Given an item template with all optional fields set
    Given a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/items" with the following json content and the current jwt
    """
    {
      "itemTemplateId": ${ItemTemplate.Id},
      "itemData": {
        "name": "some-name"
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__match": {"type": "integer"}},
        "data": {
            "name": "some-name"
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

  Scenario: List loot visible by a character
    Given a JWT for a user
    Given a group
    And a loot
    And a character
    And that the character is a member of the group
    And that the loot is visible for players

    When performing a GET to the url "/api/v2/characters/${Character.Id}/loots" with the current jwt

    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Loot.Id},
        "visibleForPlayer": true,
        "name": "${Loot.Name}",
        "items": [],
        "monsters": []
      }
    ]
    """

  Scenario: Load character history
    Given a JWT for a user
    Given a group
    And a loot
    And a character
    And that the character is a member of the group

    Given a group history entry
    And a character history entry

    When performing a GET to the url "/api/v2/characters/${Character.Id}/history?page=0" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${GroupHistoryEntry.Id},
        "date": "2020-10-05T05:07:08",
        "action": "${GroupHistoryEntry.Action}",
        "info": "${GroupHistoryEntry.Info}",
        "gm": ${GroupHistoryEntry.Gm},
        "action": "${GroupHistoryEntry.Action}",
        "isGroup": true,
        "data": ${GroupHistoryEntry.Data}
      },
      {
        "id": ${CharacterHistoryEntry.Id},
        "date": "2019-10-05T05:07:08",
        "action": "${CharacterHistoryEntry.Action}",
        "info": "${CharacterHistoryEntry.Info}",
        "gm": ${CharacterHistoryEntry.Gm},
        "action": "${CharacterHistoryEntry.Action}",
        "isGroup": false,
        "data": ${CharacterHistoryEntry.Data}
      }
    ]
    """

  Scenario: Update character stat
    Given a JWT for a user
    And a character

    When performing a PATCH to the url "/api/v2/characters/${Character.Id}/" with the following json content and the current jwt
    """
    {
      "ev": 8
    }
    """
    Then the response status code is 204

  Scenario:  Can define stat bonus for bad or exceptional AD
    Given a JWT for a user
    And a character

    When performing a PUT to the url "/api/v2/characters/${Character.Id}/statBonusAd" with the following json content and the current jwt
    """
    {
      "stat": "AD"
    }
    """
    Then the response status code is 204

  Scenario: Can add a modifier on a character
    Given a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/modifiers" with the following json content and the current jwt
    """
    {
      "reusable": false,
      "durationType": "combat",
      "values": [],
      "combatCount": 1,
      "name": "some-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    { "__partial": {
      "id": {"__match": {"type": "integer"}},
      "reusable": false,
      "durationType": "combat",
      "values": [],
      "combatCount": 1,
      "name": "some-name"
    }}
    """

  Scenario: Can delete a character modifier
    Given a JWT for a user
    And a character
    And a character modifier

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/modifiers/${CharacterModifier.Id}" with the current jwt
    Then the response status code is 204

  Scenario: Can toggle a character modifier
    Given a JWT for a user
    And a character
    And an inactive reusable character modifier that last 2 combat

    When performing a POST to the url "/api/v2/characters/${Character.Id}/modifiers/${CharacterModifier.Id}/toggle" with the following json content and the current jwt
    """
    {}
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    { "__partial": {
      "currentCombatCount": 2,
      "active": true
    }}
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/modifiers/${CharacterModifier.Id}/toggle" with the following json content and the current jwt
    """
    {}
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    { "__partial": {
      "active": false
    }}
    """

  Scenario: Group master can change some character data
    Given a JWT for a user
    Given a group
    And a character
    And that the character is a member of the group
    And a monster
    And a user

    When performing a PATCH to the url "/api/v2/characters/${Character.Id}/" with the following json content and the current jwt
    """
    {
      "active": true,
      "ownerId": ${User.[-1].Id},
      "target": {
        "isMonster": true,
        "id": ${Monster.Id}
      },
      "color": "054258",
      "mankdebol": 4,
      "debilibeuk": 2
    }
    """
    Then the response status code is 204

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt

    Then the response status code is 200
    And the response should contains the following json
    """
    { "__partial": {
      "id": ${Character.Id},
      "active": true,
      "ownerId": ${User.[-1].Id},
      "target": {
        "isMonster": true,
        "id": ${Monster.Id}
      },
      "color": "054258",
      "gmData": {
        "mankdebol": 4,
        "debilibeuk": 2
      }
    }}
    """