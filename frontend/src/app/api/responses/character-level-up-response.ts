import {Guid} from '../shared/util';
import {SpecialityResponse} from './speciality-response';
import {TalentResponse} from './talent-response';
import {MasteryResponse} from './mastery-response';

export interface CharacterLevelUpResponse {
    newModifiers: ActiveStatsModifierResponse[];
    newSkillIds: Guid[];
    newSpecialities: SpecialityResponse[];
    newLevel: number;
    newTalents: TalentResponse[];
    newMasteries: MasteryResponse[];
}

export type ActiveStatsModifierResponse = {
    // FIXME: when needed
}

