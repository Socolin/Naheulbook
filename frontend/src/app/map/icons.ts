import * as L from 'leaflet';

export const defaultMarkerIcon = new L.Icon({
    iconUrl: '/assets/icons/position-marker.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const measureMarkerIcon = new L.Icon({
    iconUrl: '/assets/icons/measure-marker.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const woodenSignIcon = new L.Icon({
    iconUrl: '/assets/icons/wooden-sign.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const towerIcon = new L.Icon({
    iconUrl: '/assets/icons/tower.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const directionSignsIcon = new L.Icon({
    iconUrl: '/assets/icons/direction-signs.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const campingTentIcon = new L.Icon({
    iconUrl: '/assets/icons/camping-tent.svg',
    className: 'marker-blue',
    iconSize: [48, 48],
    iconAnchor: [24, 48],
    attribution: 'https://game-icons.net'
});

export const markerIcons = {
    default: defaultMarkerIcon,
    woodenSign: woodenSignIcon,
    tower: towerIcon,
    directionSigns: directionSignsIcon,
    campingTent: campingTentIcon,
};
