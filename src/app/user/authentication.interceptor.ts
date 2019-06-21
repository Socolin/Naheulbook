import {Injectable} from '@angular/core';
import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest
} from '@angular/common/http';

import {Observable} from 'rxjs';
import {LoginService} from './login.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

    public constructor(private loginService: LoginService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler):
        Observable<HttpEvent<any>> {
        if (this.loginService.currentJwt) {
            req = req.clone({
                setHeaders: {
                    'Authorization': `JWT ${this.loginService.currentJwt}`
                },
            });
        }
        return next.handle(req);
    }
}
