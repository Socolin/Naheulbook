Feature: Character backup
    Scenario: Can backup a character
        Given a user
        And a JWT for a user
        And a character with all possible data

        Given an item template with all optional fields set
        And an item in the character inventory

        When performing a GET to the url "/api/v2/characters/${Character.Id}/backup" with the current jwt
        Then the response status code is 200
        And the response should contains the following json
        """
        {
          "version": 1,
          "name": "${Character.Name}",
          "notes": "${Character.Notes}",
          "originId": "${Character.OriginId}",
          "sex": "${Character.Sex}",
          "skillIds": {"__partialArray": {"array": [
            "${Character.Skills.[0].SkillId}",
            "${Character.Skills.[1].SkillId}"
          ]}},
          "specialitiesIds": [
            "${Character.Specialities.[0].SpecialityId}"
          ],
          "statBonusAd": "${Character.StatBonusAd}",
          "stats": {
            "ad": ${Character.Ad},
            "cha": ${Character.Cha},
            "cou": ${Character.Cou},
            "fo": ${Character.Fo},
            "int": ${Character.Int}
          },
          "ea": ${Character.Ea},
          "ev": ${Character.Ev},
          "experience": ${Character.Experience},
          "fatePoint": ${Character.FatePoint},
          "level": ${Character.Level},
          "items": [
            {
              "data": "{\"key\":\"value\"}",
              "modifiers": "[{\"id\":0,\"permanent\":false,\"active\":true,\"reusable\":false,\"description\":\"some-description\"}]",
              "itemTemplate": {
                "id": "${ItemTemplate.Id}",
                "name": "${ItemTemplate.Name}",
                "techName": "${ItemTemplate.TechName}",
                "subCategoryId": ${ItemTemplateSubCategory.Id},
                "data": "{\"key\": \"value\"}",
                "subCategoryId": ${ItemTemplate.SubCategoryId},
                "modifiers": [
                  {
                    "requiredJobId": "${ItemTemplate.Modifiers.[0].RequiredJobId}",
                    "requiredOriginId": "${ItemTemplate.Modifiers.[0].RequiredOriginId}",
                    "statName": "${ItemTemplate.Modifiers.[0].StatName}",
                    "type": "${ItemTemplate.Modifiers.[0].Type}",
                    "value": ${ItemTemplate.Modifiers.[0].Value}
                  }
                ],
                "requirements": [
                  {
                    "minValue": ${ItemTemplate.Requirements.[0].MinValue},
                    "maxValue": ${ItemTemplate.Requirements.[0].MaxValue},
                    "statName": "${ItemTemplate.Requirements.[0].StatName}"
                  }
                ],
                "skillModifiers": [
                  {
                    "skillId": "${ItemTemplate.SkillModifiers.[0].SkillId}",
                    "value": ${ItemTemplate.SkillModifiers.[0].Value}
                  }
                ],
                "skills": [
                  "${ItemTemplate.Skills.[0].SkillId}"
                ],
                "slots": [
                  "${ItemTemplate.Slots.[0].Slot.TechName}"
                ],
                "unSkills": [
                  "${ItemTemplate.UnSkills.[0].SkillId}"
                ]
              }
            }
          ],
          "jobIds": {"__partialArray": {"array": [
            "${Character.Jobs.[0].JobId}",
            "${Character.Jobs.[1].JobId}"
          ]}},
          "modifiers": [
            {
              "combatCount": ${Character.Modifiers.[0].CombatCount},
              "currentCombatCount": ${Character.Modifiers.[0].CurrentCombatCount},
              "description": "${Character.Modifiers.[0].Description}",
              "durationType": "${Character.Modifiers.[0].DurationType}",
              "isActive": true,
              "name": "${Character.Modifiers.[0].Name}",
              "permanent": ${Character.Modifiers.[0].Permanent},
              "reusable": ${Character.Modifiers.[0].Reusable},
              "values": [
                {
                  "statName": "${Character.Modifiers.[0].Values.[0].StatName}",
                  "type": "${Character.Modifiers.[0].Values.[0].Type}",
                  "value": ${Character.Modifiers.[0].Values.[0].Value}
                },
                {
                  "statName": "${Character.Modifiers.[0].Values.[1].StatName}",
                  "type": "${Character.Modifiers.[0].Values.[1].Type}",
                  "value": ${Character.Modifiers.[0].Values.[1].Value}
                }
              ]
            },
            {
              "description": "${Character.Modifiers.[1].Description}",
              "durationType": "${Character.Modifiers.[1].DurationType}",
              "isActive": true,
              "name": "${Character.Modifiers.[1].Name}",
              "permanent": ${Character.Modifiers.[1].Permanent},
              "reusable": ${Character.Modifiers.[1].Reusable},
              "values": [
                {
                  "statName": "${Character.Modifiers.[1].Values.[0].StatName}",
                  "type": "${Character.Modifiers.[1].Values.[0].Type}",
                  "value": ${Character.Modifiers.[1].Values.[0].Value}
                },
                {
                  "statName": "${Character.Modifiers.[1].Values.[1].StatName}",
                  "type": "${Character.Modifiers.[1].Values.[1].Type}",
                  "value": ${Character.Modifiers.[1].Values.[1].Value}
                }
              ]
            },
            {
              "lapCount": ${Character.Modifiers.[2].LapCount},
              "lapCountDecrement": {"__match": {"type": "string"}},
              "currentLapCount": ${Character.Modifiers.[2].CurrentLapCount},
              "description": "${Character.Modifiers.[2].Description}",
              "durationType": "${Character.Modifiers.[2].DurationType}",
              "isActive": true,
              "name": "${Character.Modifiers.[2].Name}",
              "permanent": ${Character.Modifiers.[2].Permanent},
              "reusable": ${Character.Modifiers.[2].Reusable},
              "values": []
            }
          ]
        }
        """