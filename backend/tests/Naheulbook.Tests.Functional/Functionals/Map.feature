Feature: Map

  Scenario: Can create a new map
    Given a JWT for an admin user

    When performing a multipart POST to the url "/api/v2/maps/" with the following json content as "request" and an image as "image" and the current jwt
    """json
    {
      "name": "some-map-name",
      "data": {
        "isGm": true,
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
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-map-name",
      "layers": [],
      "imageData": {
        "extraZoomCount": {"__match": {"type": "integer"}},
        "height": {"__match": {"type": "integer"}},
        "width": {"__match": {"type": "integer"}},
        "zoomCount": {"__match": {"type": "integer"}}
      },
      "data": {"__partial": {
          "isGm": true,
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

    When performing a GET to the url "/api/v2/maps/${Map.[-1].Id}"
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Map.[-1].Id}",
      "name": "${Map.[-1].Name}",
      "layers": [
        {
          "id": "!{Map.[-1].Layers.[0].Id}",
          "name": "${Map.[-1].Layers.[0].Name}",
          "source": "${Map.[-1].Layers.[0].Source}",
          "isGm": true,
          "markers": [
            {
              "id": "!{Map.[-1].Layers.[0].Markers.[0].Id}",
              "name": "${Map.[-1].Layers.[0].Markers.[0].Name}",
              "description": "${Map.[-1].Layers.[0].Markers.[0].Description}",
              "type": "${Map.[-1].Layers.[0].Markers.[0].Type}",
              "markerInfo": "!{Map.[-1].Layers.[0].Markers.[0].MarkerInfo}",
              "links": [
                {
                  "id": "!{Map.[-1].Layers.[0].Markers.[0].Links.[0].Id}",
                  "name": "${Map.[-1].Layers.[0].Markers.[0].Links.[0].Name}",
                  "targetMapIsGm": true,
                  "targetMapId": "!{Map.[-1].Layers.[0].Markers.[0].Links.[0].TargetMapId}",
                  "targetMapName": "${Map.[0].Name}"
                }
              ]
            }
          ]
        }
      ],
      "data": {"__partial": {
        "attribution": [
          {
            "name": "some-attribution-name",
            "url": "some-attribution-url"
          }
        ]
      }},
      "imageData": "!{Map.[-1].ImageData}"
    }
    """

  Scenario: Can list maps

    Given a map with all data

    When performing a GET to the url "/api/v2/maps/"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """json
    {
      "id": "!{Map.[-1].Id}",
      "name": "${Map.[-1].Name}",
      "data": {"__partial": {
        "isGm": true,
        "attribution": [
          {
            "name": "some-attribution-name",
            "url": "some-attribution-url"
          }
        ]
      }}
    }
    """

  Scenario: Can edit a map
    Given a JWT for an admin user
    Given a map

    When performing a PUT to the url "/api/v2/maps/${Map.[-1].Id}" with the following json content and the current jwt
    """json
    {
      "name": "some-map-name",
      "data": {
        "unitName": "5m",
        "pixelPerUnit": 42,
        "isGm": true,
        "attribution": [
          {
            "name": "new-name",
            "url": "new-url"
          }
        ]
      }
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Map.Id}",
      "name": "some-map-name",
      "data": {
        "unitName": "5m",
        "pixelPerUnit": 42.0,
        "isGm": true,
        "attribution": [
          {
            "name": "new-name",
            "url": "new-url"
          }
        ]
      }
    }
    """

  Scenario: Can add a layer to a map
    Given a JWT for an admin user
    Given a map

    When performing a POST to the url "/api/v2/maps/${Map.Id}/layers" with the following json content and the current jwt
    """json
    {
      "name": "some-layer-name",
      "source": "official",
      "isGm": true
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-layer-name",
      "source": "official",
      "isGm": true,
      "markers": []
    }
    """

  Scenario: Can edit a map layer
    Given a JWT for an admin user
    Given a map with a layer

    When performing a PUT to the url "/api/v2/mapLayers/${Map.Layers.[0].Id}" with the following json content and the current jwt
    """json
    {
      "name": "some-new-layer-name",
      "source": "private",
      "isGm": false
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Map.Layers.[0].Id}",
      "name": "some-new-layer-name",
      "isGm": false,
      "markers": [],
      "source": "private"
    }
    """

  Scenario: Can delete a map layer
    Given a JWT for an admin user
    Given a map with a layer

    When performing a DELETE to the url "/api/v2/mapLayers/${Map.[-1].Layers.[0].Id}" with the current jwt
    Then the response status code is 204

  Scenario: Can add a marker to a map layer
    Given a JWT for an admin user
    Given a map with a layer

    When performing a POST to the url "/api/v2/mapLayers/${Map.[-1].Layers.[0].Id}/markers" with the following json content and the current jwt
    """json
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
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-marker-name",
      "description": "some-marker-description",
      "links": [],
      "type": "point",
      "markerInfo": {
        "lat": 5,
        "lng": 4
      }
    }
    """

  Scenario: Can delete a map marker
    Given a JWT for an admin user
    Given a map with a marker

    When performing a DELETE to the url "/api/v2/mapMarkers/${Map.[-1].Layers.[0].Markers.[0].Id}" with the current jwt
    Then the response status code is 204

  Scenario: Can edit a map marker
    Given a JWT for an admin user
    Given a map with a marker

    When performing a PUT to the url "/api/v2/mapMarkers/${Map.Layers.[0].Markers.[0].Id}" with the following json content and the current jwt
    """json
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
    """json
    {
      "id": "!{Map.Layers.[0].Markers.[0].Id}",
      "name": "some-new-marker-name",
      "description": "some-new-marker-description",
      "type": "point",
      "links": [],
      "markerInfo": {
        "lat": 8,
        "lng": 10
      }
    }
    """

  Scenario: Can add a link between a marker and a map
    Given a JWT for an admin user
    Given 2 maps with all data

    When performing a POST to the url "/api/v2/mapMarkers/${Map.[1].Layers.[0].Markers.[0].Id}/links" with the following json content and the current jwt
    """json
    {
      "name": "some-link-name",
      "targetMapId": "!{Map.[2].Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": {"__match": {"type": "integer"}},
      "name": "some-link-name",
      "targetMapIsGm": true,
      "targetMapId": "!{Map.[2].Id}",
      "targetMapName": "${Map.[2].Name}"
    }
    """

  Scenario: Can edit a map marker link
    Given a JWT for an admin user
    Given a map with all data

    When performing a PUT to the url "/api/v2/mapMarkerLinks/${Map.[-1].Layers.[0].Markers.[0].Links.[0].Id}" with the following json content and the current jwt
    """json
    {
      "name": "some-link-name",
      "targetMapId": "!{Map.[1].Id}"
    }
    """
    Then the response status code is 200
    And the response should contains the following json
    """json
    {
      "id": "!{Map.[-1].Layers.[0].Markers.[0].Links.[0].Id}",
      "name": "some-link-name",
      "targetMapIsGm": true,
      "targetMapId": "!{Map.[1].Id}",
      "targetMapName": "${Map.[1].Name}"
    }
    """

  Scenario: Can delete a map marker link
    Given a JWT for an admin user
    Given a map with all data

    When performing a DELETE to the url "/api/v2/mapMarkerLinks/${Map.[-1].Layers.[0].Markers.[0].Links.[0].Id}" with the current jwt
    Then the response status code is 204