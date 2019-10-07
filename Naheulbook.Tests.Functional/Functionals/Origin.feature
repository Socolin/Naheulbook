Feature: Origin

  Scenario: Listing origins
    Given an origin with all possible data

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
        "data": ${Origin.Data},
        "advantage": "${Origin.Advantage}",
        "size": "${Origin.Size}",
        "flags": [
            {
                "type": "value"
            }
        ],
        "skillIds": [
            ${Skill.[0].Id}
        ],
        "availableSkillIds": [
            ${Skill.[1].Id}
        ],
        "information": [
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
        "restrictions": [
            {
                "description": "${Origin.Restrictions.[0].Text}",
                "flags": [
                    {
                        "type": "value"
                    }
                ]
            }
        ]
    }
    """

  Scenario: Can get random name for an origin
    Given an origin with random name api configured

    When performing a GET to the url "/api/v2/origins/${Origin.Id}/randomCharacterName?sex=some-sex"
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "name": {"__match": {"type": "string"}}
    }
    """