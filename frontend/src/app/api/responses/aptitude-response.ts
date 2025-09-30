export type AptitudeResponse = {
    id: string;
    roll: number;
    type: string;
    name: string;
    description: string;
    effect: string;
}

export type AptitudeGroupResponse = {
    id: string;
    name: string;
    aptitudes: AptitudeResponse[];
}


export type SummaryAptitudeGroupResponse = {
    id: string;
    name: string;
}

export type AptitudeGroupsResponse = {
    aptitudeGroups: SummaryAptitudeGroupResponse[];
}
