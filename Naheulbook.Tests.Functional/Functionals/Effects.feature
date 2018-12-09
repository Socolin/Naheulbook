Feature: Effect

  Scenario: Listing effects by category
    When performing a GET to the url "/api/v2/effectCategories/1/effects"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": 5,
        "name": "Allongement des bras",
        "description": "Signe distinctif difficile à cacher, handicapant.",
        "durationType": "forever",
        "duration": null,
        "combatCount": null,
        "lapCount": null,
        "timeDuration": null,
        "dice": 5,
        "categoryId": 1,
        "modifiers": [
            {
                "stat": "AD",
                "value": -1,
                "type": "ADD"
            },
            {
                "stat": "CHA",
                "value": -1,
                "type": "ADD"
            }
        ]
    }
    """

  Scenario: Listing effect categories
    When performing a GET to the url "/api/v2/effectCategories/"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": 1,
        "name": "Mutation",
        "categories": [
            {
                "id": 1,
                "name": "Mutation mineure",
                "diceCount": 2,
                "diceSize": 6,
                "note": "La mutation magique survient en cinq minutes. Pour se débarrasser d'une mutation, lui ou un allié, un mage peut essayer un dispel magic, une seule fois.\nEn cas d'échec, il convient de se rendre dans une clinique de mages, à Glargh ou bien à Waldorg. Le résultat n'est pas garanti.\nDans le cas d'une mutation non visible immédiatement, le MJ doit se débrouiller pour permettre au héros de découvrir sa mutation, cela peut prendre du temps.",
                "typeId": 1
            },
            {
                "id": 2,
                "name": "Mutation majeure",
                "diceCount": 1,
                "diceSize": 20,
                "note": "La mutation magique survient en cinq minutes. Pour se débarrasser d'une mutation, lui ou un allié, un mage peut essayer un dispel magic, une seule fois.\nEn cas d'échec, il convient de se rendre dans une clinique de mages, à Glargh ou bien à Waldorg. Le résultat n'est pas garanti.\nDans le cas d'une mutation non visible immédiatement, le MJ doit se débrouiller pour permettre au héros de découvrir sa mutation, cela peut prendre du temps.",
                "typeId": 1
            }
        ]
    }
    """

  Scenario: Create an effect category and an effect
    Given a JWT for an admin user
    When performing a POST to the url "/api/v2/effectTypes/" with the following json content and the current jwt
    """
    {
        "name": "some-effect-type-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__capture": {"name": "effectTypeId", "type": "integer"}},
        "name": "some-effect-type-name",
        "categories": []
    }
    """

    When performing a POST to the url "/api/v2/effectCategories/" with the following json content and the current jwt
    """
    {
        "name": "some-effect-category-name",
        "diceCount": 2,
        "diceSize": 6,
        "note": "some-note",
        "typeId": ${effectTypeId}
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__capture": {"name": "effectCategoryId", "type": "integer"}},
        "name": "some-effect-category-name",
        "diceCount": 2,
        "diceSize": 6,
        "note": "some-note",
        "typeId": ${effectTypeId}
    }
    """

    When performing a POST to the url "/api/v2/effects/" with the following json content and the current jwt
    """
    {
      "combatCount": 1,
      "categoryId": ${effectCategoryId},
      "description": "some-description",
      "duration": "some-duration",
      "dice": 4,
      "name": "some-name",
      "durationType": "custom",
      "lapCount": 16,
      "timeDuration": 458,
      "modifiers": [
        {
          "stat": "FO",
          "value": 5,
          "type": "ADD"
        },
        {
          "stat": "CHA",
          "value": -3,
          "type": "ADD"
        }
      ]
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__capture": {"name": "effectId", "type": "integer"}},
      "combatCount": 1,
      "categoryId": ${effectCategoryId},
      "description": "some-description",
      "duration": "some-duration",
      "dice": 4,
      "durationType": "custom",
      "lapCount": 16,
      "name": "some-name",
      "timeDuration": 458,
      "modifiers": [
        {
          "stat": "FO",
          "value": 5,
          "type": "ADD"
        },
        {
          "stat": "CHA",
          "value": -3,
          "type": "ADD"
        },
      ]
    }
    """

