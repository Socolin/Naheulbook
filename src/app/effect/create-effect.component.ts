import {Component} from '@angular/core';

import {NotificationsService} from '../notifications';
import {EffectService} from './effect.service';
import {Effect} from './effect.model';

@Component({
    selector: 'create-effect',
    templateUrl: './create-effect.component.html',
    providers: [EffectService],
})
export class CreateEffectComponent {
    public effect: Effect = new Effect();

    constructor(private _effectService: EffectService
        , private _notification: NotificationsService) {
    }

    create() {
        if (!this.effect.category) {
            return false;
        }
        if (!this.effect.name) {
            return false;
        }

        this._effectService.createEffect(this.effect).subscribe(
            newEffect => {
                this._notification.success('Effet créé', newEffect.name);
                this.effect = new Effect();
                if (newEffect.dice) {
                    this.effect.dice = newEffect.dice + 1;
                }
            },
            error => {
                console.log(error);
                this._notification.error('Erreur', 'Erreur serveur');
            }
        );
    }
}
