Feature: Skill

  Scenario: Listing skills
    Given a skill

    When performing a GET to the url "/api/v2/skills"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": "${Skill.Id}",
        "name": "${Skill.Name}",
        "description": "${Skill.Description}",
        "playerDescription": "${Skill.PlayerDescription}",
        "require": "${Skill.Require}",
        "resist": "${Skill.Resist}",
        "using": "${Skill.Using}",
        "roleplay": "${Skill.Roleplay}",
        "test": ${Skill.Test},
        "stat": [
          "${Skill.Stat}"
        ],
        "flags": [
          {
            "type": "value"
          }
        ],
        "effects": [
            {
                "stat": "${Skill.SkillEffects.[0].StatName}",
                "value": ${Skill.SkillEffects.[0].Value},
                "type": "ADD"
            }
        ]
    }
    """
