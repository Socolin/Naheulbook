Feature: MonsterTemplates

  Scenario: Create a monster template
    Given a JWT for an admin user
    Given an item template
    Given a location
    Given a monster category type
    Given a monster category

    When performing a POST to the url "/api/v2/monsterTemplates/" with the following json content and the current jwt
    """
    {
      "categoryId": ${MonsterCategory.Id},
      "monster": {
        "name": "some-monster-name",
        "data": {
          "some-key-1": 1,
          "some-key-2": "some-data-2",
        },
        "locations": [
          ${Location.Id}
        ],
        "simpleInventory": [
          {
            "itemTemplate": {
              "id": ${ItemTemplate.Id}
            },
            "chance": 0.5,
            "minCount": 1,
            "maxCount": 2
          }
        ]
      },
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__match": {"type": "integer"}},
        "name": "some-monster-name",
        "categoryId": ${MonsterCategory.Id},
        "data": {
          "some-key-1": 1,
          "some-key-2": "some-data-2",
        },
        "locations": [
            ${Location.Id}
        ],
        "simpleInventory": [
            {
                "id": {"__match": {"type": "integer"}},
                "minCount": 1,
                "maxCount": 2,
                "chance": 0.5,
                "itemTemplate": {"__partial": {
                  "id": ${ItemTemplate.Id}
                }}
            }
        ]
    }
    """

