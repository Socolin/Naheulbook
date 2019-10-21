Feature: Map

  Scenario: Can create a new map
    Given a JWT for an admin user

    When performing a multipart POST to the url "/api/v2/maps/" with the following json content as "request" and an image as "image" and the current jwt
    """
    {
      "name": "some-map-name",
      "data": {
        "attribution": [
          {
            "name": "Naheulbeuk",
            "url": "http://naheulbeuk.com/"
          }
        ]
      }
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-map-name",
      "layers": [],
      "data": {"__partial": {
          "attribution": [
            {
              "name": "Naheulbeuk",
              "url": "http://naheulbeuk.com/"
            }
          ]
        }
      }
    }
    """

  Scenario: Can get a map info
    Given a map with all data

    When performing a GET to the url "/api/v2/maps/${Map.Id}"
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Map.Id},
      "name": "${Map.Name}",
      "layers": [
        {
          "id": ${Map.Layers.[0].Id},
          "name": "${Map.Layers.[0].Name}",
          "source": "${Map.Layers.[0].Source}"
        }
      ],
      "data": {"__partial": {
        "width": {"__match": {"type": "integer"}},
        "height":  {"__match": {"type": "integer"}},
        "attribution": [
          {
            "name": "some-attribution-name",
            "url": "some-attribution-url"
          }
        ]
      }}
    }
    """

  Scenario: Can add a layer to a map
    Given a JWT for an admin user
    Given a map

    When performing a POST to the url "/api/v2/maps/${Map.Id}/layers" with the following json content and the current jwt
    """
    {
      "name": "some-layer-name",
      "source": "official"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-layer-name",
      "source": "official"
    }
    """
