Feature: Group Events

  Scenario: Can list group events
    Given a JWT for a user
    Given a group
    And an event

    When performing a GET to the url "/api/v2/groups/${Group.Id}/events" with the current jwt
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Event.Id},
        "timestamp": ${Event.Timestamp},
        "name": "${Event.Name}",
        "description": "${Event.Description}"
      }
    ]
    """