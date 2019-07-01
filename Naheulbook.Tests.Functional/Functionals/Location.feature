Feature: Location

  Scenario: Can list locations
    Given a location with a map and a parent

    When performing a GET to the url "/api/v2/locations"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
      "id": ${Location.[1].Id},
      "name": "${Location.[1].Name}",
      "data": ${Location.[1].Data},
      "parent": ${Location.[0].Id}
    }
    """

    When performing a GET to the url "/api/v2/locations/${Location.[1].Id}/maps"
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Location.[1].Maps.[0].Id},
        "gm": ${Location.[1].Maps.[0].IsGm},
        "data": ${Location.[1].Maps.[0].Data},
        "name": "${Location.[1].Maps.[0].Name}",
        "file": "${Location.[1].Maps.[0].File}"
      }
    ]
    """

  Scenario: Can search location
    Given a location

    When performing a GET to the url "/api/v2/locations/search?filter=${Location.Name}"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
      "id": ${Location.Id},
      "name": "${Location.Name}",
      "data": ${Location.Data},
    }
    """
