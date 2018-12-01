Feature: Authentication

  Scenario: Can create a user with a username and a password and validate his email
    When performing a POST to the url "/api/v2/users/" with the following "application/json" content
    """
    {
      "username": "test@naheulbook.fr",
      "password": "iHE1vAqAKZtoPdWDXW9lgOkI+SWtGV/UB59fPU6Occ602wQs1xsOywVDPLy5z6DS"
    }
    """
    Then the response status code is 201
    And a mail validation mail has been sent to "test@naheulbook.fr"

    When performing a POST to the url "/api/v2/users/test@naheulbook.fr/validate" with the following "application/json" content
    """
    {
      "activationCode": "${ActivationCode}"
    }
    """
    Then the response status code is 204
