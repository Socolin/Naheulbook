Feature: ItemTemplate

  Scenario: Can create an item template section
    Given a JWT for an admin user

    When performing a POST to the url "/api/v2/itemTemplateSections/" with the following json content and the current jwt
    """
    {
      "name": "some-name",
      "note": "some-note",
      "icon": "some-icon",
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
      "icon": "some-icon",
      "subCategories": [],
      "specials": [
        "some-special"
      ]
    }
    """

  Scenario: Can create an item sub-category
    Given a JWT for an admin user
    And an item template section

    When performing a POST to the url "/api/v2/itemTemplateSubCategories/" with the following json content and the current jwt
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

  Scenario: List item template sub categories
    Given an item template section
    And an item template sub-category
    And an item template sub-category

    When performing a GET to the url "/api/v2/itemTemplateSections/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${ItemTemplateSection.Id},
        "name": "${ItemTemplateSection.Name}",
        "note": "${ItemTemplateSection.Note}",
        "icon": "${ItemTemplateSection.Icon}",
        "specials": [
            "${ItemTemplateSection.Special}"
        ],
        "subCategories": [
            {
                "id": ${ItemTemplateSubCategory.[0].Id},
                "name": "${ItemTemplateSubCategory.[0].Name}",
                "techName": "${ItemTemplateSubCategory.[0].TechName}",
                "description": "${ItemTemplateSubCategory.[0].Description}",
                "note": "${ItemTemplateSubCategory.[0].Note}",
                "sectionId": ${ItemTemplateSubCategory.[0].SectionId}
            },
            {
                "id": ${ItemTemplateSubCategory.[1].Id},
                "name": "${ItemTemplateSubCategory.[1].Name}",
                "techName": "${ItemTemplateSubCategory.[1].TechName}",
                "description": "${ItemTemplateSubCategory.[1].Description}",
                "note": "${ItemTemplateSubCategory.[1].Note}",
                "sectionId": ${ItemTemplateSubCategory.[1].SectionId}
            }

        ]
    }
    """

  Scenario: Can create an item template
    Given a JWT for an admin user
    And an item template section
    And an item template sub-category
    And an item slot
    And 3 skills

    When performing a POST to the url "/api/v2/itemTemplates/" with the following json content and the current jwt
    """
    {
      "source": "official",
      "subCategoryId": ${ItemTemplateSubCategory.Id},
      "name": "some-name",
      "techName": "some-tech-name",
      "source": "official",
      "modifiers": [
        {
          "stat": "FO",
          "value": 1,
          "type": "ADD",
          "special": ["SOME-SPECIAL"],
          "jobId": null,
          "originId": null
        }
      ],
      "skillIds": [
        "${Skill.[0].Id}"
      ],
      "unSkillIds": [
        "${Skill.[1].Id}"
      ],
      "skillModifiers": [
        {
          "skillId": "${Skill.[2].Id}",
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
      "id": {"__match": {"type": "string"}},
      "source": "official",
      "subCategoryId": ${ItemTemplateSubCategory.Id},
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
      "skillIds": [
        "${Skill.[0].Id}"
      ],
      "unSkillIds": [
        "${Skill.[1].Id}"
      ],
      "skillModifiers": [
        {
          "skillId": "${Skill.[2].Id}",
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
        "id": "${ItemTemplate.Id}",
        "name": "${ItemTemplate.Name}",
        "techName": "${ItemTemplate.TechName}",
        "source": "official",
        "subCategoryId": ${ItemTemplateSubCategory.Id},
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
    """

  Scenario: Can list item template of a section
    Given an item template with all optional fields set

    When performing a GET to the url "/api/v2/itemTemplateSections/${ItemTemplateSection.Id}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": "${ItemTemplate.Id}",
        "name": "${ItemTemplate.Name}",
        "techName": "${ItemTemplate.TechName}",
        "source": "official",
        "subCategoryId": ${ItemTemplateSubCategory.Id},
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
    """

  Scenario: Can edit item template
    Given a JWT for an admin user
    And an item template with all optional fields set
    And an item template sub-category
    And an item slot
    And a stat
    And 3 skills

    When performing a PUT to the url "/api/v2/itemTemplates/${ItemTemplate.Id}" with the following json content and the current jwt
    """
    {
        "id": "${ItemTemplate.Id}",
        "name": "some-new-name",
        "techName": "some-new-tech-name",
        "source": "official",
        "subCategoryId": ${ItemTemplateSubCategory.[-1].Id},
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
                "jobId": null,
                "originId": null,
                "special": [
                    "SOME_SPECIAL"
                ],
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
    """

    Then the response status code is 200
    And the response should contains the following json
    """
    {
         "id": "${ItemTemplate.Id}",
         "name": "some-new-name",
         "techName": "some-new-tech-name",
         "source": "official",
         "subCategoryId": ${ItemTemplateSubCategory.[-1].Id},
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
                 "special": [
                     "SOME_SPECIAL"
                 ],
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
    """

  Scenario: Can search an item template
    Given an item template

    When performing a GET to the url "/api/v2/itemTemplates/search?filter=${ItemTemplate.Name}"
    Then the response status code is 200
    And the response should contains the following json
      """
      [
        {
           "id": "${ItemTemplate.Id}",
           "name": "${ItemTemplate.Name}",
           "techName": "${ItemTemplate.TechName}",
           "source": "official",
           "subCategoryId": ${ItemTemplateSubCategory.Id},
           "data": ${ItemTemplate.Data},
           "slots": [],
           "modifiers": [],
           "requirements": [],
           "skillModifiers": [],
           "skillIds": [],
           "unSkillIds": []
         }
      ]
      """

  Scenario: Can get items by sub-category techName
    Given an item template

    When performing a GET to the url "/api/v2/itemTemplateSubCategories/${ItemTemplateSubCategory.TechName}/itemTemplates"
    Then the response status code is 200
    And the response should contains the following json
      """
      [
        {
           "id": "${ItemTemplate.Id}",
           "name": "${ItemTemplate.Name}",
           "techName": "${ItemTemplate.TechName}",
           "source": "official",
           "subCategoryId": ${ItemTemplateSubCategory.Id},
           "data": ${ItemTemplate.Data},
           "slots": [],
           "modifiers": [],
           "requirements": [],
           "skillModifiers": [],
           "skillIds": [],
           "unSkillIds": []
         }
      ]
      """
