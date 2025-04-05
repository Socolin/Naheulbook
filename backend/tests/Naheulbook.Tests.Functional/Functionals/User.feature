Feature: User

  Scenario: Can update user profile
    Given a JWT for a user

    When performing a PATCH to the url "/api/v2/users/${UserId}" with the following json content and the current jwt
    """json
    {
      "displayName": "some-new-display-name",
      "showInSearchFor": 600
    }
    """
    Then the response status code is 204

  Scenario: Can manage access token
    Given a user identified by a password
    And the user is authenticated with a session

    When performing a POST to the url "/api/v2/users/me/accessTokens" with the following json content and the current session
    """json
    {
      "name": "some-access-token-name"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """json
    {
      "__partial": {
        "id": {"__capture": {"name": "AccessTokenId", "type": "string"}},
        "name": "some-access-token-name",
        "key": {"__capture": {"name": "UserAccessKey", "type": "string"}},
        "dateCreated": {"__match": {"type": "string"}}
      }
    }
    """

    When performing a GET to the url "/api/v2/users/me/accessTokens" with the current session
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": "${AccessTokenId}",
        "name": "some-access-token-name",
        "dateCreated": {"__match": {"type": "string"}}
      }
    ]
    """

    Given a group

    When performing a GET to the url "/api/v2/groups/${Group.Id}" with "${UserAccessKey}" as access token
    Then the response status code is 200

    When performing a DELETE to the url "/api/v2/users/me/accessTokens/${AccessTokenId}" with the current session
    Then the response status code is 204

    When performing a GET to the url "/api/v2/groups/${Group.Id}" with "${UserAccessKey}" as access token
    Then the response status code is 401