import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {NotificationsService} from '../notifications';

import {EffectService} from "./effect.service";

@Component({
    templateUrl: 'edit-effect.component.html',
    providers: [EffectService],
})
export class EditEffectComponent implements OnInit {
    public effect: Object;
    public errorMessage: string;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _notification: NotificationsService
        , private _effectService: EffectService) {
        this.effect = {};
    }

    edit() {
        this._effectService.editEffect(this.effect).subscribe(
            effect => {
                this._router.navigate(['/database/effects'], {queryParams: {id: effect.category}});
            },
            error => {
                console.log(error);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            let effectId = +params['id'];
            this._effectService.getEffect(effectId).subscribe(
                effect => {
                    this.effect = effect;
                },
                error => {
                    console.log(error);
                    this.errorMessage = "Erreur lors du chargement de l'effet";
                }
            );
        });
    }
}
