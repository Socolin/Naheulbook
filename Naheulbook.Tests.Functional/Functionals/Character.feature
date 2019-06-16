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