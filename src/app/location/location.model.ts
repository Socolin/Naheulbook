import {LocationMapResponse} from '../api/responses';

export class Map {
    id: number;
    name = '';
    file = '';
    gm: boolean;
    data: any;

    static fromResponse(responses: LocationMapResponse): Map {
        const map = new Map();
        Object.assign(map, responses);
        return map;
    }

    static fromResponses(responses: LocationMapResponse[]): Map[] {
        return responses.map(response => Map.fromResponse(response));
    }
}

export class Location {
    id: number;
    name: string;
    parent: number;
    sons: Location[];
    data: any;
}
