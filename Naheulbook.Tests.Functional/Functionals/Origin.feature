Feature: Origin

  Scenario: Listing origins
    Given a origin with all possible data

    When performing a GET to the url "/api/v2/origins"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${Origin.Id},
        "name": "${Origin.Name}",
        "description": "${Origin.Description}",
        "playerDescription": "${Origin.PlayerDescription}",
        "playerSummary": "${Origin.PlayerSummary}",
        "maxLoad": ${Origin.MaxLoad},
        "maxArmorPR": ${Origin.MaxArmorPr},
        "advantage": "${Origin.Advantage}",
        "baseEV": ${Origin.BaseEv},
        "baseEA": ${Origin.BaseEa},
        "bonusAT": ${Origin.BonusAt},
        "bonusPRD": ${Origin.BonusPrd},
        "diceEVLevelUp": ${Origin.DiceEvLevelUp},
        "size": "${Origin.Size}",
        "flags": [
            {
                "type": "value",
                "data": null
            }
        ],
        "speedModifier": ${Origin.SpeedModifier},
        "skillIds": [
            ${Skill.[0].Id}
        ],
        "availableSkillIds": [
            ${Skill.[1].Id}
        ],
        "infos": [
            {
                "title": "${Origin.Information.[0].Title}",
                "description": "${Origin.Information.[0].Description}"
            }
        ],
        "bonuses": [
            {
                "description": "${Origin.Bonuses.[0].Description}",
                "flags": [
                ]
            }
        ],
        "requirements": [
            {
                "stat": "${Stat.Name}",
                "min": ${Origin.Requirements.[0].MinValue},
                "max": ${Origin.Requirements.[0].MaxValue}
            }
        ],
        "restricts": [
            {
                "description": "${Origin.Restrictions.[0].Text}",
                "flags": [
                    {
                        "type": "value",
                        "data": null
                    }
                ]
            }
        ]
    }
    """
