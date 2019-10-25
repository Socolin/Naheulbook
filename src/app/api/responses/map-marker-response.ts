export interface MapMarkerResponse {
    id: number;
    name: string;
    description?: string;
    type: 'point' | 'area' | 'rectangle' | 'circle';
    markerInfo: any;
}
