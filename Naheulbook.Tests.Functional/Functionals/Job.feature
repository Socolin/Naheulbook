Feature: Job

  Scenario: Listing jobs
    Given a job with all possible data

    When performing a GET to the url "/api/v2/jobs"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${Job.Id},
        "name": "${Job.Name}",
        "informations": "${Job.Information}",
        "playerDescription": "${Job.PlayerDescription}",
        "playerSummary": "${Job.PlayerSummary}",
        "maxLoad": ${Job.MaxLoad},
        "maxArmorPR": ${Job.MaxArmorPr},
        "isMagic": ${Job.IsMagic},
        "baseEv": ${Job.BaseEv},
        "factorEv": ${Job.FactorEv},
        "bonusEv": ${Job.BonusEv},
        "baseEa": ${Job.BaseEa},
        "diceEaLevelUp": ${Job.DiceEaLevelUp},
        "baseAT": ${Job.BaseAt},
        "basePRD": ${Job.BasePrd},
        "flags": [
            {
                "type": "value"
            }
        ],
        "skillIds": [
          ${Skill.[-2].Id}
        ],
        "availableSkillIds": [
          ${Skill.[-1].Id}
        ],
        "originsWhitelist": [
            {
                "id": ${Origin.[-1].Id},
                "name": "${Origin.[-1].Name}"
            }
          ],
        "originsBlacklist": [
            {
                "id": ${Origin.[-2].Id},
                "name": "${Origin.[-2].Name}"
            }
        ],
        "bonuses": [
            {
                "description": "${Job.Bonuses.[0].Description}",
                "flags": []
            }
        ],
        "requirements": [
            {
                "stat": "${Stat.Name}",
                "min": ${Job.Requirements.[0].MinValue},
                "max": ${Job.Requirements.[0].MaxValue}
            }
        ],
        "restricts": [
            {
                "description": "${Job.Restrictions.[0].Text}",
                "flags": []
            }
        ],
        "specialities": [
            {
                "id": ${Speciality.Id},
                "name": "${Speciality.Name}",
                "description": "${Speciality.Description}",
                "modifiers": [
                    {
                        "type": "ADD",
                        "value": ${Speciality.Modifiers.[0].Value},
                        "stat": "${Stat.Name}",
                    }
                ],
                "specials": [
                    {
                        "id": ${Speciality.Specials.[0].Id},
                        "isBonus": ${Speciality.Specials.[0].IsBonus},
                        "description": "${Speciality.Specials.[0].Description}",
                        "flags": ${Speciality.Specials.[0].Flags}
                    }
                ],
                "flags": [
                    {
                        "type": "value"
                    }
                ]
            }
        ]
    }
    """
