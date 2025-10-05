export type AptitudeResponse = {
    id: string;
    name: string;
    roll: number;
    type: string;
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
