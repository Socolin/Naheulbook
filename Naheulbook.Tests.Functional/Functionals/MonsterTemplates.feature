Feature: MonsterTemplates

  Scenario: Create a monster template
    Given a JWT for an admin user
    Given an item template section
    Given an item template category
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
        "locationIds": [
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
        "locationIds": [
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


  Scenario: Listing types
    Given a monster category type
    Given a monster category
    Given a monster category

    When performing a GET to the url "/api/v2/monsterTypes/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${MonsterType.Id},
        "name": "${MonsterType.Name}",
        "categories": [
            {
                "id": ${MonsterCategory.[0].Id},
                "name": "${MonsterCategory.[0].Name}",
                "typeid": ${MonsterType.Id}
            },
            {
                "id": ${MonsterCategory.[1].Id},
                "name": "${MonsterCategory.[1].Name}",
                "typeid": ${MonsterType.Id}
            }
        ]
    }
    """

  Scenario: Listing traits
    Given a monster trait

    When performing a GET to the url "/api/v2/monsterTraits/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${MonsterTrait.Id},
        "name": "${MonsterTrait.Name}",
        "description": "${MonsterTrait.Description}",
        "levels": ["level1", "level2"],
    }
    """


  Scenario: Listing monster templates
    Given a monster template with locations and inventory

    When performing a GET to the url "/api/v2/monsterTemplates/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
      "id": ${MonsterTemplate.Id},
      "name": "${MonsterTemplate.Name}",
      "data": ${MonsterTemplate.Data},
      "categoryId": ${MonsterTemplate.CategoryId},
      "simpleInventory": [
        {
          "id": ${MonsterTemplate.Items.[0].Id},
          "chance": ${MonsterTemplate.Items.[0].Chance},
          "maxCount": ${MonsterTemplate.Items.[0].MaxCount},
          "minCount": ${MonsterTemplate.Items.[0].MinCount},
          "itemTemplate": { "__partial": {
            "id": ${ItemTemplate.Id},
            "name": "${ItemTemplate.Name}"
          }}
        },
      ],
      "locationIds": [
        ${Location.Id}
      ]
    }
    """

  Scenario: Create monster category type

  Scenario: Create monster category

  Scenario: Search monster template
    Given a monster template with locations and inventory

    When performing a GET to the url "/api/v2/monsterTemplates/search?filter=${MonsterTemplate.Name}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
      "id": ${MonsterTemplate.Id},
      "name": "${MonsterTemplate.Name}",
      "data": ${MonsterTemplate.Data},
      "categoryId": ${MonsterTemplate.CategoryId},
      "simpleInventory": [
        {
          "id": ${MonsterTemplate.Items.[0].Id},
          "chance": ${MonsterTemplate.Items.[0].Chance},
          "maxCount": ${MonsterTemplate.Items.[0].MaxCount},
          "minCount": ${MonsterTemplate.Items.[0].MinCount},
          "itemTemplate": { "__partial": {
            "id": ${ItemTemplate.Id},
            "name": "${ItemTemplate.Name}"
          }}
        },
      ],
      "locationIds": [
        ${Location.Id}
      ]
    }
    """

  Scenario: Edit a monster template
