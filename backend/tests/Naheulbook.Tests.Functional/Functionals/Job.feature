Feature: Job

  Scenario: Listing jobs
    Given a job with all possible data

    When performing a GET to the url "/api/v2/jobs"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
        "id": "${Job.Id}",
        "name": "${Job.Name}",
        "information": "${Job.Information}",
        "playerDescription": "${Job.PlayerDescription}",
        "playerSummary": "${Job.PlayerSummary}",
        "data": "!{Job.Data}",
        "isMagic": "!{Job.IsMagic}",
        "flags": [
            {
                "type": "value"
            }
        ],
        "skillIds": [
          "${Skill.[-2].Id}"
        ],
        "availableSkillIds": [
          "${Skill.[-1].Id}"
        ],
        "bonuses": [
            {
                "description": "${Job.Bonuses.[0].Description}",
                "flags": [
                    {
                       "data": "some-data",
                       "type": "some-type"
                    }
                ]
            }
        ],
        "requirements": [
            {
                "stat": "${Stat.Name}",
                "min": "!{Job.Requirements.[0].MinValue}",
                "max": "!{Job.Requirements.[0].MaxValue}"
            }
        ],
        "restrictions": [
            {
                "description": "${Job.Restrictions.[0].Text}",
                "flags": [
                    {
                       "data": "some-data",
                       "type": "some-type"
                    }
                ]
            }
        ],
        "specialities": [
            {
                "id": "${Speciality.Id}",
                "name": "${Speciality.Name}",
                "description": "${Speciality.Description}",
                "modifiers": [
                    {
                        "type": "ADD",
                        "value": "!{SpecialityModifierEntity.Value}",
                        "stat": "${SpecialityModifierEntity.Stat}"
                    }
                ],
                "specials": [
                    {
                        "id": "!{Speciality.Specials.[0].Id}",
                        "isBonus": "!{Speciality.Specials.[0].IsBonus}",
                        "description": "${Speciality.Specials.[0].Description}",
                        "flags": "!{Speciality.Specials.[0].Flags}"
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