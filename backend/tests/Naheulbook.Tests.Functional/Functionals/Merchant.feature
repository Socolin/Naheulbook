Feature: Merchant

  Scenario: A group owner can create a merchant
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/merchants" with the following json content and the current jwt
    """json
    {
        "name": "some-merchant-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
        "id": {"__match": {"type": "integer"}},
        "name": "some-merchant-name"
    }
    """