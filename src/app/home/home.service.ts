import {Injectable} from '@angular/core';
import {Http} from '@angular/http';

import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

@Injectable()
export class HomeService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

}
