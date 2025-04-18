Feature: MonsterTemplates

  Scenario: Create a monster template
    Given a JWT for an admin user
    Given an item template section
    Given an item template sub-category
    Given an item template
    Given a monster category type
    Given a monster sub-category

    When performing a POST to the url "/api/v2/monsterTemplates/" with the following json content and the current jwt
    """json
    {
      "subCategoryId": "!{MonsterSubCategory.Id}",
      "name": "some-monster-name",
      "data": {
        "at": 1,
        "note": "some-note"
      },
      "inventory": [
        {
          "itemTemplateId": "${ItemTemplate.Id}",
          "chance": 0.5,
          "minCount": 1,
          "maxCount": 2
        }
      ]
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
        "id": {"__match": {"type": "integer"}},
        "name": "some-monster-name",
        "subCategoryId": "!{MonsterSubCategory.Id}",
        "data": {
          "at": 1,
          "note": "some-note"
        },
        "inventory": [
            {
                "id": {"__match": {"type": "integer"}},
                "minCount": 1,
                "maxCount": 2,
                "chance": 0.5,
                "itemTemplate": {"__partial": {
                  "id": "${ItemTemplate.Id}"
                }}
            }
        ]
    }
    """

  Scenario: Listing types
    Given a monster category type
    Given a monster sub-category
    Given a monster sub-category

    When performing a GET to the url "/api/v2/monsterTypes/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
        "id": "!{MonsterType.Id}",
        "name": "${MonsterType.Name}",
        "subCategories": [
            {
                "id": "!{MonsterSubCategory.[0].Id}",
                "name": "${MonsterSubCategory.[0].Name}",
                "typeid": "!{MonsterType.Id}"
            },
            {
                "id": "!{MonsterSubCategory.[1].Id}",
                "name": "${MonsterSubCategory.[1].Name}",
                "typeid": "!{MonsterType.Id}"
            }
        ]
    }
    """

  Scenario: Listing traits
    Given a monster trait

    When performing a GET to the url "/api/v2/monsterTraits/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
        "id": "!{MonsterTrait.Id}",
        "name": "${MonsterTrait.Name}",
        "description": "${MonsterTrait.Description}",
        "levels": ["level1", "level2"]
    }
    """

  Scenario: Listing monster templates
    Given a monster template with inventory

    When performing a GET to the url "/api/v2/monsterTemplates/"

    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
      "id": "!{MonsterTemplate.Id}",
      "name": "${MonsterTemplate.Name}",
      "data": "!{MonsterTemplate.Data}",
      "subCategoryId": "!{MonsterTemplate.SubCategoryId}",
      "inventory": [
        {
          "id": "!{MonsterTemplate.Items.[0].Id}",
          "chance": "!{MonsterTemplate.Items.[0].Chance}",
          "maxCount": "!{MonsterTemplate.Items.[0].MaxCount}",
          "minCount": "!{MonsterTemplate.Items.[0].MinCount}",
          "itemTemplate": { "__partial": {
            "id": "${ItemTemplate.Id}",
            "name": "${ItemTemplate.Name}"
          }}
        }
      ]
    }
    """

  Scenario: Create monster category type
    Given a JWT for an admin user

    When performing a POST to the url "/api/v2/monsterTypes/" with the following json content and the current jwt
    """json
    {
      "name": "some-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-name",
      "subCategories": []
    }
    """

  Scenario: Create monster sub-category
    Given a JWT for an admin user
    Given a monster category type

    When performing a POST to the url "/api/v2/monsterTypes/${MonsterType.Id}/subcategories" with the following json content and the current jwt
    """json
    {
      "name": "some-category-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-category-name",
      "typeid": "!{MonsterType.Id}"
    }
    """

  Scenario: Search monster template
    Given a monster template with inventory

    When performing a GET to the url "/api/v2/monsterTemplates/search?filter=${MonsterTemplate.Name}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
      "id": "!{MonsterTemplate.Id}",
      "name": "${MonsterTemplate.Name}",
      "data": "!{MonsterTemplate.Data}",
      "subCategoryId": "!{MonsterTemplate.SubCategoryId}",
      "inventory": [
        {
          "id": "!{MonsterTemplate.Items.[0].Id}",
          "chance": "!{MonsterTemplate.Items.[0].Chance}",
          "maxCount": "!{MonsterTemplate.Items.[0].MaxCount}",
          "minCount": "!{MonsterTemplate.Items.[0].MinCount}",
          "itemTemplate": { "__partial": {
            "id": "${ItemTemplate.Id}",
            "name": "${ItemTemplate.Name}"
          }}
        }
      ]
    }
    """

  Scenario: Edit a monster template
    Given a JWT for an admin user
    Given a monster template
    Given an item template section
    Given an item template sub-category
    Given an item template
    Given a monster category type
    Given a monster sub-category

    When performing a PUT to the url "/api/v2/monsterTemplates/${MonsterTemplate.Id}" with the following json content and the current jwt
    """json
    {
      "subCategoryId": "!{MonsterSubCategory.[1].Id}",
      "name": "some-new-monster-name",
      "data": {
        "at": 2,
        "prd": 3,
        "note": "some-new-note"
      },
      "inventory": [
        {
          "itemTemplateId": "${ItemTemplate.Id}",
          "chance": 0.5,
          "minCount": 1,
          "maxCount": 2
        }
      ]
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{MonsterTemplate.Id}",
      "name": "some-new-monster-name",
      "subCategoryId": "!{MonsterSubCategory.[1].Id}",
      "data": {
        "at": 2,
        "prd": 3,
        "note": "some-new-note"
      },
      "inventory": [
          {
              "id": {"__match": {"type": "integer"}},
              "minCount": 1,
              "maxCount": 2,
              "chance": 0.5,
              "itemTemplate": {"__partial": {
                "id": "${ItemTemplate.Id}"
              }}
          }
      ]
    }
    """