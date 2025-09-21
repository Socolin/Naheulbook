import {map} from 'rxjs/operators';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from '@angular/router';
import {Injectable} from '@angular/core';
import {LoginService} from './login.service';
import {Observable} from 'rxjs';

@Injectable({ providedIn: 'root'})
export class AuthGuard  {
    constructor(
        private readonly loginService: LoginService,
        private readonly router: Router,
    ) {
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<boolean> | Promise<boolean> | boolean {
        if (this.loginService.currentLoggedUser) {
            return true;
        }

        return this.loginService.isLogged().pipe(map((logged: boolean) => {
            if (!logged) {
                let redirect = route.data['authGuardRedirect'];
                this.router.navigateByUrl(redirect);
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
