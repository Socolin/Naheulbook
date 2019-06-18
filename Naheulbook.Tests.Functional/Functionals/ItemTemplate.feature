Feature: ItemTemplate

  Scenario: Can create an item template section
    Given a JWT for an admin user

    When performing a POST to the url "/api/v2/itemTemplateSections/" with the following json content and the current jwt
    """
    {
      "name": "some-name",
      "note": "some-note",
      "specials": [
        "some-special"
      ]
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__capture": {"name": "ItemTemplateSectionId", "type": "integer"}},
      "name": "some-name",
      "note": "some-note",
      "categories": [],
      "specials": [
        "some-special"
      ]
    }
    """

  Scenario: Can create an item category
    Given a JWT for an admin user
    And an item template section

    When performing a POST to the url "/api/v2/itemTemplateCategories/" with the following json content and the current jwt
    """
    {
      "sectionId": ${ItemTemplateSection.Id},
      "name": "some-name",
      "techName": "some-tech-name",
      "description": "some-description",
      "note": "some-note"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "sectionId": ${ItemTemplateSection.Id},
      "name": "some-name",
      "techName": "some-tech-name",
      "description": "some-description",
      "note": "some-note"
    }
    """

  Scenario: List item template categories
    Given an item template section
    And an item template category
    And an item template category

    When performing a GET to the url "/api/v2/itemTemplateSections/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${ItemTemplateSection.Id},
        "name": "${ItemTemplateSection.Name}",
        "note": "${ItemTemplateSection.Note}",
        "specials": [
            "${ItemTemplateSection.Special}"
        ],
        "categories": [
            {
                "id": ${ItemTemplateCategory.[0].Id},
                "name": "${ItemTemplateCategory.[0].Name}",
                "techName": "${ItemTemplateCategory.[0].TechName}",
                "description": "${ItemTemplateCategory.[0].Description}",
                "note": "${ItemTemplateCategory.[0].Note}",
                "sectionId": ${ItemTemplateCategory.[0].SectionId}
            },
            {
                "id": ${ItemTemplateCategory.[1].Id},
                "name": "${ItemTemplateCategory.[1].Name}",
                "techName": "${ItemTemplateCategory.[1].TechName}",
                "description": "${ItemTemplateCategory.[1].Description}",
                "note": "${ItemTemplateCategory.[1].Note}",
                "sectionId": ${ItemTemplateCategory.[1].SectionId}
            }

        ]
    }
    """

  Scenario: Can create an item template
    Given a JWT for an admin user
    And an item template section
    And an item template category
    And an item slot
    And 3 skills

    When performing a POST to the url "/api/v2/itemTemplates/" with the following json content and the current jwt
    """
    {
      "source": "official",
      "categoryId": ${ItemTemplateCategory.Id},
      "name": "some-name",
      "techName": "some-tech-name",
      "source": "official",
      "modifiers": [
        {
          "stat": "FO",
          "value": 1,
          "type": "ADD",
          "special": ["SOME-SPECIAL"],
          "job": null,
          "origin": null
        }
      ],
      "skills": [
        {
          "id": ${Skill.[0].Id}
        }
      ],
      "unskills": [
        {
          "id": ${Skill.[1].Id}
        }
      ],
      "skillModifiers": [
        {
          "skill": ${Skill.[2].Id},
          "value": 1
        }
      ],
      "requirements": [
        {
          "stat": "FO",
          "min": 3,
          "max": 8
        }
      ],
      "slots": [
        {
          "id": ${Slot.Id}
        }
      ],
      "data": {
        "some-data": 42
      }
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "source": "official",
      "categoryId": ${ItemTemplateCategory.Id},
      "name": "some-name",
      "techName": "some-tech-name",
      "source": "official",
      "modifiers": [
        {
          "stat": "FO",
          "value": 1,
          "type": "ADD",
          "special": ["SOME-SPECIAL"]
        }
      ],
      "skills": [
        {
          "id": ${Skill.[0].Id}
        }
      ],
      "unskills": [
        {
          "id": ${Skill.[1].Id}
        }
      ],
      "skillModifiers": [
        {
          "skill": ${Skill.[2].Id},
          "value": 1
        }
      ],
      "requirements": [
        {
          "stat": "FO",
          "min": 3,
          "max": 8
        }
      ],
      "slots": [
        {
          "id": ${Slot.Id},
          "name": "${Slot.Name}",
          "techName": "${Slot.TechName}"
        }
      ],
      "data": {
        "some-data": 42
      }
    }
    """

  Scenario: Can list item slots
    Given an item slot

    When performing a GET to the url "/api/v2/itemSlots"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${Slot.Id},
        "name": "${Slot.Name}",
        "techName": "${Slot.TechName}"
    }
    """

  Scenario: Can get item template details
    Given an item template with all optional fields set

    When performing a GET to the url "/api/v2/itemTemplates/${ItemTemplate.Id}"
    Then the response status code is 200
    And the response should contains the following json
    """
    {
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
    """

  Scenario: Can list item template of a section
    Given an item template with all optional fields set

    When performing a GET to the url "/api/v2/itemTemplateSections/${ItemTemplateSection.Id}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
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
    """

  Scenario: Can edit item template
    Given a JWT for an admin user
    And an item template with all optional fields set
    And an item template category
    And an item slot
    And a stat
    And 3 skills

    When performing a PUT to the url "/api/v2/itemTemplates/${ItemTemplate.Id}" with the following json content and the current jwt
    """
    {
        "id": ${ItemTemplate.Id},
        "name": "some-new-name",
        "techName": "some-new-tech-name",
        "source": "official",
        "categoryId": ${ItemTemplateCategory.[-1].Id},
        "data": {
            "new-key": "new-value"
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
                "stat": "${Stat.[-1].Name}",
                "value": 4,
                "job": null,
                "origin": null,
                "special": [],
                "type": "ADD"
            }
        ],
        "requirements": [
            {
                "stat": "${Stat.[-1].Name}",
                "min": 4,
                "max": 15
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
                "id":  ${Skill.[-1].Id},
            }
        ],
        "unskills": [
            {
                "id":  ${Skill.[-2].Id},
            }
        ]
    }
    """

    Then the response status code is 200
    And the response should contains the following json
    """
    {
         "id": ${ItemTemplate.Id},
         "name": "some-new-name",
         "techName": "some-new-tech-name",
         "source": "official",
         "categoryId": ${ItemTemplateCategory.[-1].Id},
         "data": {
             "new-key": "new-value"
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
                 "stat": "${Stat.[-1].Name}",
                 "value": 4,
                 "special": [],
                 "type": "ADD"
             }
         ],
         "requirements": [
             {
                 "stat": "${Stat.[-1].Name}",
                 "min": 4,
                 "max": 15
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
                 "id":  ${Skill.[-1].Id},
             }
         ],
         "unskills": [
             {
                 "id":  ${Skill.[-2].Id},
             }
         ]
     }
     """
