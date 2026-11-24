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

  Scenario: A super admin can search users by their identifiers
    Given a user
    And a JWT for a super admin user

    When performing a GET to the url "/api/v2/users?facebookId=${User.[0].FbId}&googleId=${User.[0].GoogleId}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    [
      {
        "id": ${User.[0].Id},
        "username": "${User.[0].Username}",
        "displayName": "${User.[0].DisplayName}",
        "adminLevel": 0,
        "facebookId": "${User.[0].FbId}",
        "googleId": "${User.[0].GoogleId}",
        "microsoftId": "${User.[0].MicrosoftId}"
      }
    ]
    """

  Scenario: An admin cannot search users by their identifiers
    Given a user
    And a JWT for an admin user

    When performing a GET to the url "/api/v2/users?facebookId=${User.[0].FbId}" with the current jwt
    Then the response status code is 403

  Scenario: A super admin can delete a user with its groups, characters and item templates
    # First user: the one deleted, GM of the first group, with 2 characters (one in the group of another GM)
    Given a user
    And 2 characters
    And a group
    And a private item template created by the first user
    And a community item template created by the first user
    # Second user: GM of the second group, with a character in the group of the first user
    And a user
    And a group
    And a character
    And that the second character is a member of the second group
    And that the third character is a member of the first group
    And a JWT for a super admin user

    When performing a DELETE to the url "/api/v2/users/${User.[0].Id}" with the current jwt
    Then the response status code is 204
    And the first user does not exist anymore
    And the first group does not exist anymore
    And the first character does not exist anymore
    And the second character does not exist anymore
    And the second user still exists
    And the second group still exists
    And the third character still exists
    And the third character is not in a group anymore
    And the first item template still exists
    And the first item template has no owner anymore
    And the second item template still exists
    And the second item template has no owner anymore

    When performing a GET to the url "/api/v2/users?facebookId=${User.[0].FbId}" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """json
    []
    """

  Scenario: A super admin cannot delete itself
    Given a JWT for a super admin user

    When performing a DELETE to the url "/api/v2/users/${UserId}" with the current jwt
    Then the response status code is 400

  Scenario: An admin cannot delete a user
    Given a user
    And a JWT for an admin user

    When performing a DELETE to the url "/api/v2/users/${User.[0].Id}" with the current jwt
    Then the response status code is 403

