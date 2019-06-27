Feature: Stats
  Scenario: Can list stats
    Given a stat

    When performing a GET to the url "/api/v2/stats"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by name
    """
    {
      "name": "${Stat.Name}",
      "displayName": "${Stat.DisplayName}",
      "description": "${Stat.Description}"
    }
    """