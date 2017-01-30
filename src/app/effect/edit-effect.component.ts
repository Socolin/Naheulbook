import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';

import {EffectService} from './effect.service';
import {Effect} from './effect.model';

@Component({
    templateUrl: './edit-effect.component.html',
})
export class EditEffectComponent implements OnInit {
    public effect: Effect;
    public saving: boolean;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _effectService: EffectService) {
    }

    edit() {
        this.saving = true;
        this._effectService.editEffect(this.effect).subscribe(
            effect => {
                this._effectService.clearCacheEffect(effect);
                this._router.navigate(['/database/effects'], {queryParams: {id: effect.category}});
            },
            () => {
                this.saving = false;
            }
        );
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            let effectId = +params['id'];
            this._effectService.getEffect(effectId).subscribe(
                effect => {
                    this.effect = effect;
                }
            );
        });
    }
}
