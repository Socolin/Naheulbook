Feature: User

  Scenario: Can update user profile
    Given a JWT for a user

    When performing a PATCH to the url "/api/v2/users/${UserId}" with the following json content and the current jwt
    """
    {
      "displayName": "some-new-display-name",
      "showInSearchFor": 600
    }
    """
    Then the response status code is 204
