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
