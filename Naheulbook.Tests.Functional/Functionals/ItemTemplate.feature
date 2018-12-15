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
      "id": {"__capture": {"name": "itemSectionId", "type": "integer"}},
      "name": "some-name",
      "note": "some-note",
      "categories": [],
      "specials": [
        "some-special"
      ]
    }
    """

  Scenario: Can create an item category

  Scenario: Can create an item template

  Scenario: Can list item slots

  Scenario: Can get item template details

  Scenario: Can edit item template