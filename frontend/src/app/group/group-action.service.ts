import {Injectable} from '@angular/core';
import {Group} from './group.model';
import {ActionService} from '../shared/action.service';

@Injectable()
export class GroupActionService extends ActionService<Group> {

}
