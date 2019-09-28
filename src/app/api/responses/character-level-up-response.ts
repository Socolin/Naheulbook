import {ActiveStatsModifier} from '../../shared';
import {Speciality} from '../../job';

export interface CharacterLevelUpResponse {
    newModifiers: ActiveStatsModifier[];
    newSkillIds: number[];
    newSpecialities: Speciality[];
    newLevel: number;
}
