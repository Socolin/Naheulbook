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
    Given a character with all possible data

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
        "invites": [],
        "items": [],
        "group": {
            "id": ${Group.Id},
            "name": "${Group.Name}"
        }
    }
    """
