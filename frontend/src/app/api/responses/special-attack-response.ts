import {IDurable} from '../shared';

export type SpecialAttackPrerequisite = {
    specialAttackId: string;
    successCount: number;
}

export type SpecialAttackData = {
    requiredLevel?: number;
    usableBy?: string;
    learning?: string;
    usage?: string;
    effects?: string;
    test?: string;
    conditions?: string;
    damage?: string;
    criticalFailure?: string;
    criticalSuccess?: string;
    criticalNotes?: string;
    range?: string;
    xp?: number;
    /** Delay before the special attack can be used again after a use. Absent when the usage is not limited. */
    cooldown?: IDurable;
    /** Delay before a new attempt after a failure. Absent when the special attack cannot fail. */
    failureCooldown?: IDurable;
    /** Number of uses allowed during one cooldown period (e.g. "twice per day"). Absent means 1. */
    usesPerCooldown?: number;
    prerequisite?: SpecialAttackPrerequisite;
}

export type SpecialAttackResponse = {
    id: string;
    name: string;
    data: SpecialAttackData;
}

export type SpecialAttackGroupResponse = {
    id: string;
    name: string;
    description?: string;
    specialAttacks: SpecialAttackResponse[];
}

export type SpecialAttackGroupsResponse = {
    specialAttackGroups: SpecialAttackGroupResponse[];
}

/**
 * A running cooldown: same fields as the modifiers durations so the group duration updates can decrement it.
 */
export type SpecialAttackCooldown = IDurable & {
    currentCombatCount?: number;
    currentLapCount?: number;
    currentTimeDuration?: number;
    failure: boolean;
}

export type CharacterSpecialAttackData = {
    successCount: number;
    cooldowns: SpecialAttackCooldown[];
}

export type CharacterSpecialAttackResponse = {
    specialAttack: SpecialAttackResponse;
    data: CharacterSpecialAttackData;
}

export type UseCharacterSpecialAttackResponse = CharacterSpecialAttackResponse & {
    /** Experience of the character after the use (a success gives the XP of the special attack) */
    experience: number;
}
