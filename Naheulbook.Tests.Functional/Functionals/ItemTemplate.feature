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
      "sectionId": ${ItemTemplateSectionId},
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
      "id": {"__capture": {"name": "ItemTemplateCategoryId", "type": "integer"}},
      "sectionId": ${ItemTemplateSectionId},
      "name": "some-name",
      "techName": "some-tech-name",
      "description": "some-description",
      "note": "some-note",
      "itemTemplates": []
    }
    """

  Scenario: Can create an item template
    Given a JWT for an admin user
    And an item template category

    When performing a POST to the url "/api/v2/itemTemplates/" with the following json content and the current jwt
    """
    {
      "source": "official",
      "categoryId": ${ItemTemplateCategoryId},
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
          "id": 2
        }
      ],
      "unskills": [
        {
          "id": 1
        }
      ],
      "skillModifiers": [
        {
          "skill": 4,
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
          "id": 1
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
      "id": {"__capture": {"name": "ItemTemplateId", "type": "integer"}},
      "source": "official",
      "categoryId": ${ItemTemplateCategoryId},
      "name": "some-name",
      "techName": "some-tech-name",
      "source": "official",
      "sourceUser": null,
      "sourceUserId": null,
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
          "id": 2
        }
      ],
      "unskills": [
        {
          "id": 1
        }
      ],
      "skillModifiers": [
        {
          "skill": 4,
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
          "id": 1
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
        "id": ${ItemSlot.Id},
        "name": "${ItemSlot.Name}",
        "techName": "${ItemSlot.TechName}"
    }
    """

  Scenario: Can get item template details

  Scenario: Can edit item template