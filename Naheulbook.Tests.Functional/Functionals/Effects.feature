Feature: Effect

  Scenario: Listing effects by category
    When performing a GET to the url "/api/v2/effectCategories/1/effects"
    Then the response status code be 200
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
