Feature: Skill

  Scenario: Listing skills
    When performing a GET to the url "/api/v2/skills"
    Then the response status code be 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "stat": [],
        "id": 49,
        "name": "Truc de mauviette",
        "description": "Le héros n'a pas mal, même quand il a mal. Il serre les dents et continue de faire son\nmalin, parce que la douleur est une simple information. Le héros qui rit des trucs de mauviette dispose d'une protection\nnaturelle de 1 point supplémentaire (PR+1).",
        "playerDescription": "résistance à la douleur",
        "require": null,
        "resist": null,
        "using": "+1 au score de PR totale.",
        "roleplay": null,
        "test": null,
        "flags": [],
        "effects": [
            {
                "stat": "PR",
                "value": 1,
                "type": "Add"
            }
        ]
    }
    """
