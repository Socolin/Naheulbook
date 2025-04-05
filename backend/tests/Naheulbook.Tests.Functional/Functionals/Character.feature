Feature: Character

  Scenario: Create a character
    Given a JWT for a user
    And a job
    And a skill
    And a speciality
    And an origin
    And item templates required for initial inventory

    When performing a POST to the url "/api/v2/characters" with the following json content and the current jwt
    """json
    {
      "name": "some-name",
      "stats": {
        "COU": 8,
        "INT": 9,
        "CHA": 10,
        "AD": 11,
        "FO": 12
      },
      "jobId": "${Job.Id}",
      "originId": "${Origin.Id}",
      "skillIds": [
        "${Skill.Id}"
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
      "specialityId": "${Speciality.Id}",
      "fatePoint": 3
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}}
    }
    """

  Scenario: Create a character using the custom method
    Given a JWT for a user
    And an origin
    And a skill
    And 2 jobs
    And 2 specialities

    When performing a POST to the url "/api/v2/characters/custom" with the following json content and the current jwt
    """json
    {
      "name": "some-name",
      "sex": "Homme",
      "fatePoint": 3,
      "level": 5,
      "experience": 1100,
      "stats": {
        "cou": 8,
        "int": 9,
        "cha": 10,
        "ad": 11,
        "fo": 12
      },
      "basicStatsOverrides": {
        "at": 9,
        "prd": 6,
        "ev": 25,
        "ea": 30
      },
      "originId": "${Origin.Id}",
      "jobIds": [
        "${Job.[1].Id}",
        "${Job.[0].Id}"
      ],
      "skillIds": [
        "${Skill.Id}"
      ],
      "specialityIds": {
        "${Job.[0].Id}": [
          "${Speciality.[0].Id}"
        ]
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
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
        "id": "!{Character.Id}",
        "jobs": [
          "${Job.Name}"
        ],
        "level": "!{Character.Level}",
        "name": "${Character.Name}",
        "origin": "${Origin.Name}"
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
    """json
    {
        "id": "!{Character.Id}",
        "name": "${Character.Name}",
        "originId": "${Origin.Id}",
        "level": "!{Character.Level}",
        "jobIds": {"__partialArray": {"array": [
            "${Job.[0].Id}",
            "${Job.[1].Id}"
        ]}},
        "stats": {
            "COU": "!{Character.Cou}",
            "AD": "!{Character.Ad}",
            "FO": "!{Character.Fo}",
            "INT": "!{Character.Int}",
            "CHA": "!{Character.Cha}"
        },
        "experience": "!{Character.Experience}",
        "sex": "Homme",
        "statBonusAD": "PRD",
        "notes": "some-notes",
        "fatePoint": "!{Character.FatePoint}",
        "ev": "!{Character.Ev}",
        "ea": "!{Character.Ea}",
        "isNpc": false,
        "modifiers": [
            {
                "id": "!{Character.Modifiers.[0].Id}",
                "name": "${Character.Modifiers.[0].Name}",
                "permanent": "!{Character.Modifiers.[0].Permanent}",
                "reusable": "!{Character.Modifiers.[0].Reusable}",
                "combatCount": "!{Character.Modifiers.[0].CombatCount}",
                "currentCombatCount": "!{Character.Modifiers.[0].CurrentCombatCount}",
                "description": "${Character.Modifiers.[0].Description}",
                "durationType": "${Character.Modifiers.[0].DurationType}",
                "active": "!{Character.Modifiers.[0].IsActive}",
                "values": [
                    {
                        "stat": "${Character.Modifiers.[0].Values.[0].StatName}",
                        "value": "!{Character.Modifiers.[0].Values.[0].Value}",
                        "type": "${Character.Modifiers.[0].Values.[0].Type}"
                    },
                    {
                        "stat": "${Character.Modifiers.[0].Values.[1].StatName}",
                        "value": "!{Character.Modifiers.[0].Values.[1].Value}",
                        "type": "${Character.Modifiers.[0].Values.[1].Type}"
                    }
                ]
            },
            {
                "id": "!{Character.Modifiers.[1].Id}",
                "name": "${Character.Modifiers.[1].Name}",
                "description": "${Character.Modifiers.[1].Description}",
                "permanent": "!{Character.Modifiers.[1].Permanent}",
                "reusable": "!{Character.Modifiers.[1].Reusable}",
                "durationType": "${Character.Modifiers.[1].DurationType}",
                "active": "!{Character.Modifiers.[1].IsActive}",
                "values": [
                    {
                        "stat": "${Character.Modifiers.[1].Values.[0].StatName}",
                        "value": "!{Character.Modifiers.[1].Values.[0].Value}",
                        "type": "${Character.Modifiers.[1].Values.[0].Type}"
                    },
                    {
                        "stat": "${Character.Modifiers.[1].Values.[1].StatName}",
                        "value": "!{Character.Modifiers.[1].Values.[1].Value}",
                        "type": "${Character.Modifiers.[1].Values.[1].Type}"
                    }
                ]
            },
            {
                "id": "!{Character.Modifiers.[2].Id}",
                "name": "${Character.Modifiers.[2].Name}",
                "description": "${Character.Modifiers.[2].Description}",
                "permanent": "!{Character.Modifiers.[2].Permanent}",
                "reusable": "!{Character.Modifiers.[2].Reusable}",
                "active": "!{Character.Modifiers.[2].IsActive}",
                "durationType": "${Character.Modifiers.[2].DurationType}",
                "lapCount": "!{Character.Modifiers.[2].LapCount}",
                "currentLapCount": "!{Character.Modifiers.[2].CurrentLapCount}",
                "lapCountDecrement": "!{Character.Modifiers.[2].LapCountDecrement}",
                "values": []
            }
        ],
        "skillIds": {"__partialArray": {"array": [
            "${Skill.[1].Id}",
            "${Skill.[0].Id}"
        ]}},
        "specialities": [
            {
                "id": "${Speciality.Id}",
                "name": "${Speciality.Name}",
                "description": "${Speciality.Description}",
                "modifiers": [],
                "specials": [],
                "flags": "!{Speciality.Flags}"
            }
        ],
        "invites": [
          { "__partial": {
            "groupId": "!{Group.Id}",
            "groupName": "${Group.Name}",
            "config": {
              "allowPlayersToAddObject": true,
              "allowPlayersToSeeGemPriceWhenIdentified": false,
              "allowPlayersToSeeSkillGmDetails": false,
              "autoIncrementMonsterColor": true,
              "autoIncrementMonsterNumber": true
            }}
          }
        ],
        "items": [
          {
            "id": "!{Item.Id}",
            "data": "!{Item.Data}",
            "modifiers": "!{Item.Modifiers}",
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
                  "stat": "${ItemTemplate.Modifiers.[0].StatName}",
                  "value": "!{ItemTemplate.Modifiers.[0].Value}",
                  "jobId": "${ItemTemplate.Modifiers.[0].RequiredJobId}",
                  "originId": "${ItemTemplate.Modifiers.[0].RequiredOriginId}",
                  "special": [],
                  "type": "ADD"
                }
              ],
              "requirements": [
                {
                  "stat": "${ItemTemplate.Requirements.[0].StatName}",
                  "min": "!{ItemTemplate.Requirements.[0].MinValue}",
                  "max": "!{ItemTemplate.Requirements.[0].MaxValue}"
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
        ],
        "group": {
          "id": "!{Group.Id}",
          "name": "${Group.Name}",
          "config": {
            "allowPlayersToAddObject": true,
            "allowPlayersToSeeGemPriceWhenIdentified": false,
            "allowPlayersToSeeSkillGmDetails": false,
            "autoIncrementMonsterColor": true,
            "autoIncrementMonsterNumber": true
          }
        }
    }
    """

  Scenario: Add an item to character inventory
    Given a JWT for a user
    Given an item template with all optional fields set
    Given a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/items" with the following json content and the current jwt
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
    """json
    [
      {
        "id": "!{Loot.Id}",
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
    """json
    [
      {
        "id": "!{GroupHistoryEntry.Id}",
        "date": "2020-10-05T05:07:08",
        "action": "${GroupHistoryEntry.Action}",
        "info": "${GroupHistoryEntry.Info}",
        "gm": "!{GroupHistoryEntry.Gm}",
        "isGroup": true,
        "data": "!{GroupHistoryEntry.Data}"
      },
      {
        "id": "!{CharacterHistoryEntry.Id}",
        "date": "2019-10-05T05:07:08",
        "action": "${CharacterHistoryEntry.Action}",
        "info": "${CharacterHistoryEntry.Info}",
        "gm": "!{CharacterHistoryEntry.Gm}",
        "isGroup": false,
        "data": "!{CharacterHistoryEntry.Data}"
      }
    ]
    """

  Scenario: Update character stat
    Given a JWT for a user
    And a character

    When performing a PATCH to the url "/api/v2/characters/${Character.Id}/" with the following json content and the current jwt
    """json
    {
      "ev": 8
    }
    """
    Then the response status code is 204

  Scenario: Can define stat bonus for bad or exceptional AD
    Given a JWT for a user
    And a character

    When performing a PUT to the url "/api/v2/characters/${Character.Id}/statBonusAd" with the following json content and the current jwt
    """json
    {
      "stat": "AD"
    }
    """
    Then the response status code is 204

  Scenario: Can add a modifier on a character
    Given a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/modifiers" with the following json content and the current jwt
    """json
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
    """json
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
    """json
    {}
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    { "__partial": {
      "currentCombatCount": 2,
      "active": true
    }}
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/modifiers/${CharacterModifier.Id}/toggle" with the following json content and the current jwt
    """json
    {}
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
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
    """json
    {
      "active": true,
      "ownerId": "!{User.[-1].Id}",
      "target": {
        "isMonster": true,
        "id": "!{Monster.Id}"
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
    """json
    { "__partial": {
      "id": "!{Character.Id}",
      "active": true,
      "ownerId": "!{User.[-1].Id}",
      "target": {
        "isMonster": true,
        "id": "!{Monster.Id}"
      },
      "color": "054258",
      "gmData": {
        "mankdebol": 4,
        "debilibeuk": 2
      }
    }}
    """

  Scenario: A character can level up
    Given a JWT for a user
    And a character
    And a skill

    When performing a POST to the url "/api/v2/characters/${Character.Id}/levelUp" with the following json content and the current jwt
    """json
    {
      "evOrEa": "EV",
      "evOrEaValue": 4,
      "targetLevelUp": 2,
      "statToUp": "AD",
      "skillId": "${Skill.Id}",
      "specialityIds": []
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "newModifiers": [
        {
          "id": {"__match": {"type": "integer"}},
          "active": true,
          "name": "LevelUp: 2",
          "reusable": false,
          "permanent": true,
          "durationType": "forever",
          "values": [
            {
              "stat": "EV",
              "value": 4,
              "type": "ADD"
            },
            {
              "stat": "AD",
              "value": 1,
              "type": "ADD"
            }
          ]
        }
      ],
      "newSkillIds": [
        "${Skill.Id}"
      ],
      "newLevel": 2,
      "newSpecialities": []
    }
    """

  Scenario: A character can learn new job
    Given a JWT for a user
    And a character
    And a job

    When performing a POST to the url "/api/v2/characters/${Character.Id}/addJob" with the following json content and the current jwt
    """json
    {
      "jobId": "${Job.[-1].Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "jobId": "${Job.[-1].Id}"
    }
    """

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
        "__partial": {
            "jobIds": [
                "${Job.[-1].Id}"
            ]
        }
    }
    """

  Scenario: A character can forget a job
    Given a JWT for a user
    And a job
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/removeJob" with the following json content and the current jwt
    """json
    {
      "jobId": "${Job.[0].Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "jobId": "${Job.[0].Id}"
    }
    """

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
        "__partial": {
            "jobIds": []
        }
    }
    """

  Scenario: A character can quit group
    Given a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/quitGroup" with the following json content and the current jwt
    """json
    {}
    """
    Then the response status code is 204