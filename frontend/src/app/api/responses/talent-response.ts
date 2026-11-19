export type TalentData = {
    /** Who can take the talent (origin, job, skill...). Absent when the talent is open to everyone. */
    restriction?: string;
    effect: string;
    notes?: string;
    requiredLevel?: number;
    /** Talent that must be known before this one can be taken (e.g. "Renforcement majeur" requires "Renforcement inférieur"). */
    prerequisiteTalentId?: string;
}

export type TalentResponse = {
    id: string;
    name: string;
    data: TalentData;
}

export type TalentGroupResponse = {
    id: string;
    name: string;
    description?: string;
    talents: TalentResponse[];
}

export type TalentGroupsResponse = {
    talentGroups: TalentGroupResponse[];
}
