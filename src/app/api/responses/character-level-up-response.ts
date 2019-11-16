import {ActiveStatsModifier} from '../../shared';
import {Speciality} from '../../job';
import {Guid} from '../shared/util';

export interface CharacterLevelUpResponse {
    newModifiers: ActiveStatsModifier[];
    newSkillIds: Guid[];
    newSpecialities: Speciality[];
    newLevel: number;
}
