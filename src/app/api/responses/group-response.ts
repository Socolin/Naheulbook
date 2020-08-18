import {GroupGroupInviteResponse} from './group-group-invite-response';
import {IGroupConfig} from '../shared';

export interface GroupResponse {
    id: number;
    name: string;
    data?: any;
    config: IGroupConfig;
    characterIds: number[];
    invites: GroupGroupInviteResponse[];
}
