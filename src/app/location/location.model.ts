export class Map {
    id: number;
    name: string = '';
    file: string = '';
    gm: boolean;
    data: any;
}

export class Location {
    id: number;
    name: string;
    parent: number;
    sons: Location[];
    data: any;
}
