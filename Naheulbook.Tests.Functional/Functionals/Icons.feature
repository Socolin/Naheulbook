Feature: Icons

  Scenario: Can load list of icons
    Given an icon

    When performing a GET to the url "/api/v2/icons/"
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      "${Icon.Name}"
    ]
    """
