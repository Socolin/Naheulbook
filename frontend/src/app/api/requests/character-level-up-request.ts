import {Guid} from '../shared/util';

export interface CharacterLevelUpRequest {
    evOrEa: string;
    evOrEaValue: number;
    targetLevelUp: number;
    statToUp: string;
    skillId?: Guid;
    specialityIds: Guid[];
    /** Talent learned with the talent or mastery point of odd levels (3, 5, 7...). Exclusive with masteryId. */
    talentId?: Guid;
    /** Mastery learned with the talent or mastery point of odd levels (3, 5, 7...). Exclusive with talentId. */
    masteryId?: Guid;
}
