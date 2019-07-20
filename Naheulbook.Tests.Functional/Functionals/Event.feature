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

  Scenario: Can create a group event
    Given a JWT for a user
    Given a group

    When performing a POST to the url "/api/v2/groups/${Group.Id}/events" with the following json content and the current jwt
    """
    {
      "timestamp": 42,
      "name": "some-event-name",
      "description": "some-event-description"
    }
    """
    Then the response status code is 201
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "timestamp": 42,
      "name": "some-event-name",
      "description": "some-event-description"
    }
    """


  Scenario: Can delete a group event
    Given a JWT for a user
    Given a group
    And an event

    When performing a DELETE to the url "/api/v2/groups/${Group.Id}/events/${Event.Id}" with the current jwt
    Then the response status code is 204