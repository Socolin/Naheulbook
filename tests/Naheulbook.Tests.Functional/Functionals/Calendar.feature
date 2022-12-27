Feature: Calendar

  Scenario: Can load the calendar
    Given a calendar entry

    When performing a GET to the url "/api/v2/calendar/"
    Then the response status code is 200
    And the response should contains the following json
    """
    [
      {
        "id": ${Calendar.Id},
        "name":"${Calendar.Name}",
        "startDay": ${Calendar.StartDay},
        "endDay": ${Calendar.EndDay},
        "note": "${Calendar.Note}"
      }
    ]
    """
