import {Injectable} from '@angular/core';
import {Http} from '@angular/http';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';
import {JsonService} from './json-service';

@Injectable()
export class MiscService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }
}
