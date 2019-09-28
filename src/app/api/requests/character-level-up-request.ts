export interface CharacterLevelUpRequest {
    evOrEa: string;
    evOrEaValue: number;
    targetLevelUp: number;
    statToUp: string;
    skillId?: number;
    specialityIds: number[];
}
