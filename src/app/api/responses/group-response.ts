import {GroupGroupInviteResponse} from './group-group-invite-response';

export interface GroupResponse {
    id: number;
    name: string;
    data?: any;
    characterIds: number[];
    invites: GroupGroupInviteResponse[];
}
