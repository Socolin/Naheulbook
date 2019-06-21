import {map} from 'rxjs/operators';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot} from '@angular/router';
import {Injectable} from '@angular/core';
import {LoginService} from './login.service';
import {Observable} from 'rxjs';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild {
    constructor(private _loginService: LoginService, private _router: Router) {
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<boolean> | Promise<boolean> | boolean {
        if (this._loginService.currentLoggedUser) {
            return true;
        }

        return this._loginService.isLogged().pipe(map((logged: boolean) => {
            if (!logged) {
                let redirect = route.data['authGuardRedirect'];
                this._router.navigateByUrl(redirect);
            }
            return logged;
        }));
    }

    canActivateChild(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<boolean> | Promise<boolean> | boolean {
        return this.canActivate(route, state);
    }
}
