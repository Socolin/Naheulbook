Feature: Group

  Scenario: Create a group
    Given a JWT for a user
    Given a location

    When performing a POST to the url "/api/v2/groups/" with the following json content and the current jwt
    """
    {
      "name": "some-group-name",
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-group-name",
      "data": {},
      "invited": [],
      "invites": [],
      "characters": [],
      "location": { "__partial": {
        "id": 1
      }}
    }
    """