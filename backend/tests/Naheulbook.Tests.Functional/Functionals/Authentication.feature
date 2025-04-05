Feature: Authentication

  Scenario: Can create a user with a username and a password and validate his email
    When performing a POST to the url "/api/v2/users/" with the following json content
    """json
    {
      "username": "test@naheulbook.fr",
      "password": "iHE1vAqAKZtoPdWDXW9lgOkI+SWtGV/UB59fPU6Occ602wQs1xsOywVDPLy5z6DS"
    }
    """
    Then the response status code is 201
    And a mail validation mail has been sent to "test@naheulbook.fr"

    When performing a POST to the url "/api/v2/users/test@naheulbook.fr/validate" with the following json content
    """json
    {
      "activationCode": "${ActivationCode}"
    }
    """
    Then the response status code is 204

  Scenario: A user can request a JWT
    Given a user identified by a password

    When performing a POST to the url "/api/v2/users/${Username}/jwt" with the following json content
    """json
    {
      "password": "${Password}"
    }
    """
    Then the response status code is 200
    And the response content contains a valid JWT