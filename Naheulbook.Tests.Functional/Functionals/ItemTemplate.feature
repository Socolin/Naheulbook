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

  Scenario: Can list item slots

  Scenario: Can get item template details

  Scenario: Can edit item template