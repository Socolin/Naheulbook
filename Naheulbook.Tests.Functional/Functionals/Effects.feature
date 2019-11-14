Feature: Effect

  Scenario: Listing effects by category
    Given an effect

    When performing a GET to the url "/api/v2/effectSubCategories/${EffectSubCategory.Id}/effects"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${Effect.Id},
        "name": "${Effect.Name}",
        "description": "${Effect.Description}",
        "durationType": "${Effect.DurationType}",
        "combatCount": ${Effect.CombatCount},
        "dice": ${Effect.Dice},
        "subCategoryId": ${EffectSubCategory.Id},
        "modifiers": [
            {
                "stat": "${Effect.Modifiers.[0].StatName}",
                "value": ${Effect.Modifiers.[0].Value},
                "type": "${Effect.Modifiers.[0].Type}"
            },
            {
                "stat": "${Effect.Modifiers.[1].StatName}",
                "value": ${Effect.Modifiers.[1].Value},
                "type": "${Effect.Modifiers.[1].Type}"
            },
            {
                "stat": "${Effect.Modifiers.[2].StatName}",
                "value": ${Effect.Modifiers.[2].Value},
                "type": "${Effect.Modifiers.[2].Type}"
            }
        ]
    }
    """

  Scenario: Listing effect sub categories
    Given 2 effect sub-categories
    When performing a GET to the url "/api/v2/effectSubCategories/"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${EffectType.Id},
        "name": "${EffectType.Name}",
        "subCategories": [
            {
                "id": ${EffectSubCategory.[0].Id},
                "name": "${EffectSubCategory.[0].Name}",
                "diceCount": ${EffectSubCategory.[0].DiceCount},
                "diceSize": ${EffectSubCategory.[0].DiceSize},
                "note": "${EffectSubCategory.[0].Note}",
                "typeId": ${EffectType.Id}
            },
            {
                "id": ${EffectSubCategory.[1].Id},
                "name": "${EffectSubCategory.[1].Name}",
                "diceCount": ${EffectSubCategory.[1].DiceCount},
                "diceSize": ${EffectSubCategory.[1].DiceSize},
                "note": "${EffectSubCategory.[1].Note}",
                "typeId": ${EffectType.Id}
            }
        ]
    }
    """

  Scenario: Create an effect type
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
        "id": {"__match": {"type": "integer"}},
        "name": "some-effect-type-name",
        "subCategories": []
    }
    """


  Scenario: Create an effect sub-category
    Given a JWT for an admin user
    And an effect type

    When performing a POST to the url "/api/v2/effectSubCategories/" with the following json content and the current jwt
    """
    {
        "name": "some-effect-category-name",
        "diceCount": 2,
        "diceSize": 6,
        "note": "some-note",
        "typeId": ${EffectType.Id}
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
        "id": {"__match": {"type": "integer"}},
        "name": "some-effect-category-name",
        "diceCount": 2,
        "diceSize": 6,
        "note": "some-note",
        "typeId": ${EffectType.Id}
    }
    """

  Scenario: Create an effect
    Given a JWT for an admin user
    And an effect sub-category

    When performing a POST to the url "/api/v2/effectSubCategories/${EffectSubCategory.Id}/effects" with the following json content and the current jwt
    """
    {
      "combatCount": 1,
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
      "id": {"__match": {"type": "integer"}},
      "combatCount": 1,
      "subCategoryId": ${EffectSubCategory.Id},
      "description": "some-description",
      "duration": "some-duration",
      "dice": 4,
      "durationType": "custom",
      "lapCount": 16,
      "name": "some-name",
      "timeDuration": 458,
      "modifiers": [
        {
          "stat": "CHA",
          "value": -3,
          "type": "ADD"
        },
        {
          "stat": "FO",
          "value": 5,
          "type": "ADD"
        }
      ]
    }
    """

  Scenario: Edit an effect
    Given a JWT for an admin user
    Given an effect

    When performing a PUT to the url "/api/v2/effects/${Effect.Id}" with the following json content and the current jwt
    """
    {
      "combatCount": 1,
      "name": "some-name",
      "subCategoryId": ${EffectSubCategory.Id},
      "description": "some-description",
      "duration": "some-duration",
      "dice": 4,
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
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Effect.Id},
      "subCategoryId": ${EffectSubCategory.Id},
      "name": "some-name",
      "combatCount": 1,
      "description": "some-description",
      "duration": "some-duration",
      "dice": 4,
      "durationType": "custom",
      "lapCount": 16,
      "timeDuration": 458,
      "modifiers": [
        {
          "stat": "CHA",
          "value": -3,
          "type": "ADD"
        },
        {
          "stat": "FO",
          "value": 5,
          "type": "ADD"
        }
      ]
    }
    """

  Scenario: Can search an effect
    Given an effect

    When performing a GET to the url "/api/v2/effects/search?filter=${Effect.Name}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": ${Effect.Id},
        "name": "${Effect.Name}",
        "description": "${Effect.Description}",
        "durationType": "${Effect.DurationType}",
        "combatCount": ${Effect.CombatCount},
        "dice": ${Effect.Dice},
        "subCategoryId": ${EffectSubCategory.Id},
        "modifiers": [
            {
                "stat": "${Effect.Modifiers.[0].StatName}",
                "value": ${Effect.Modifiers.[0].Value},
                "type": "${Effect.Modifiers.[0].Type}"
            },
            {
                "stat": "${Effect.Modifiers.[1].StatName}",
                "value": ${Effect.Modifiers.[1].Value},
                "type": "${Effect.Modifiers.[1].Type}"
            },
            {
                "stat": "${Effect.Modifiers.[2].StatName}",
                "value": ${Effect.Modifiers.[2].Value},
                "type": "${Effect.Modifiers.[2].Type}"
            }
        ]
    }
    """