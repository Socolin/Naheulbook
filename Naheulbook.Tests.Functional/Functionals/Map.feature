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
          "source": "${Map.Layers.[0].Source}",
          "markers": [
            {
              "id": ${Map.Layers.[0].Markers.[0].Id},
              "name": "${Map.Layers.[0].Markers.[0].Name}",
              "description": "${Map.Layers.[0].Markers.[0].Description}",
              "type": "${Map.Layers.[0].Markers.[0].Type}",
              "markerInfo": ${Map.Layers.[0].Markers.[0].MarkerInfo}
            }
          ]
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
      "source": "official",
      "markers": []
    }
    """

  Scenario: Can add a marker to a map layer
    Given a JWT for an admin user
    Given a map with a layer

    When performing a POST to the url "/api/v2/maps/${Map.Id}/layers/${Map.Layers.[0].Id}/markers" with the following json content and the current jwt
    """
    {
      "name": "some-marker-name",
      "description": "some-marker-description",
      "type": "point",
      "markerInfo": {
        "lat": 5,
        "lng": 4
      }
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-marker-name",
      "description": "some-marker-description",
      "type": "point",
      "markerInfo": {
        "lat": 5,
        "lng": 4
      }
    }
    """

  Scenario: Can delete a map marker
    Given a JWT for an admin user
    Given a map with all data

    When performing a DELETE to the url "/api/v2/mapMarkers/${Map.Layers.[0].Markers.[0].Id}" with the current jwt
    Then the response status code is 204

  Scenario: Can edit a map marker
    Given a JWT for an admin user
    Given a map with all data

    When performing a PUT to the url "/api/v2/mapMarkers/${Map.Layers.[0].Markers.[0].Id}" with the following json content and the current jwt
    """
    {
      "name": "some-new-marker-name",
      "description": "some-new-marker-description",
      "type": "point",
      "markerInfo": {
        "lat": 8,
        "lng": 10
      }
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """
    {
      "id": ${Map.Layers.[0].Markers.[0].Id},
      "name": "some-new-marker-name",
      "description": "some-new-marker-description",
      "type": "point",
      "markerInfo": {
        "lat": 8,
        "lng": 10
      }
    }
    """