export type MasteryData = {
    /** Who can take the mastery (e.g. armor natural PR range). Absent when there is no restriction. */
    restriction?: string;
    effect: string;
    notes?: string;
    requiredLevel?: number;
    /** Mastery that must be known before this one can be taken (the base mastery of a "+1" mastery). */
    prerequisiteMasteryId?: string;
}

export type MasteryResponse = {
    id: string;
    name: string;
    data: MasteryData;
}

export type MasteryGroupResponse = {
    id: string;
    name: string;
    description?: string;
    masteries: MasteryResponse[];
}

export type MasteryGroupsResponse = {
    masteryGroups: MasteryGroupResponse[];
}
