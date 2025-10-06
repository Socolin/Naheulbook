import {Guid} from '../shared/util';
import {SpecialityResponse} from './speciality-response';

export interface CharacterLevelUpResponse {
    newModifiers: ActiveStatsModifierResponse[];
    newSkillIds: Guid[];
    newSpecialities: SpecialityResponse[];
    newLevel: number;
}

export type ActiveStatsModifierResponse = {
    // FIXME: when needed
}

