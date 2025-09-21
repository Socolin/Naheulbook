import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';

import {Observable} from 'rxjs';
import {LoginService} from './login.service';

@Injectable({providedIn: 'root'})
export class AuthenticationInterceptor implements HttpInterceptor {

    public constructor(private loginService: LoginService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler):
        Observable<HttpEvent<any>> {
        if (this.loginService.currentJwt && req.url !== '/api/v2/users/me/jwt' && req.url !== '/api/v2/users/me') {
            req = req.clone({
                setHeaders: {
                    'Authorization': `JWT ${this.loginService.currentJwt}`
                },
            });
        }
        return next.handle(req);
    }
}
