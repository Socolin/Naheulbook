import {LocationResponse} from './location-response';
import {GroupGroupInviteResponse} from './group-group-invite-response';

export interface GroupResponse {
    id: number;
    name: string;
    data?: any;
    location: LocationResponse;
    characterIds: number[];
    invites: GroupGroupInviteResponse[];
}
