Feature: Spell

  Scenario: Listing spell books with their spells
    Given a spell book
    And a spell

    When performing a GET to the url "/api/v2/spellBooks"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "spellBooks": {"__partialArray": {"key":"id", "array": [
        {
          "id": "${SpellBook.Id}",
          "techName": "${SpellBook.TechName}",
          "name": "${SpellBook.Name}",
          "kind": "mage",
          "data": {
            "description": "some-description",
            "categories": ["some-category"]
          },
          "spells": [
            {
              "id": "${Spell.Id}",
              "name": "${Spell.Name}",
              "level": 3,
              "levelLabel": "Niveau 3",
              "kind": "spell",
              "category": "some-category",
              "description": "${Spell.Description}",
              "data": {
                "usages": ["some-usage"],
                "cost": {"text": "4 PA", "kind": "fixed", "astralPoints": 4}
              }
            }
          ]
        }
      ]}}
    }
    """

  Scenario: A character can get a spell book with a max level, change it, and lose it
    Given a spell book
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/spellBooks" with the following json content and the current jwt
    """json
    {
      "spellBookId": "${SpellBook.Id}",
      "maxLevel": 2
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "spellBookId": "${SpellBook.Id}",
      "maxLevel": 2
    }
    """

    When performing a POST to the url "/api/v2/characters/${Character.Id}/spellBooks" with the following json content and the current jwt
    """json
    {
      "spellBookId": "${SpellBook.Id}"
    }
    """
    Then the response status code is 400

    When performing a PUT to the url "/api/v2/characters/${Character.Id}/spellBooks/${SpellBook.Id}" with the following json content and the current jwt
    """json
    {
      "maxLevel": null
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "spellBookId": "${SpellBook.Id}",
      "maxLevel": null
    }
    """

    When performing a GET to the url "/api/v2/characters/${Character.Id}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    {"__partial": {
      "spellBooks": [
        {
          "spellBookId": "${SpellBook.Id}",
          "maxLevel": null
        }
      ]
    }}
    """

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/spellBooks/${SpellBook.Id}" with the current jwt
    Then the response status code is 204

    When performing a DELETE to the url "/api/v2/characters/${Character.Id}/spellBooks/${SpellBook.Id}" with the current jwt
    Then the response status code is 404

  Scenario: A character cannot get a spell book with an invalid max level
    Given a spell book
    And a JWT for a user
    And a character

    When performing a POST to the url "/api/v2/characters/${Character.Id}/spellBooks" with the following json content and the current jwt
    """json
    {
      "spellBookId": "${SpellBook.Id}",
      "maxLevel": 0
    }
    """
    Then the response status code is 400
