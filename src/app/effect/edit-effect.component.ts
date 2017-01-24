import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';

import {EffectService} from './effect.service';

@Component({
    templateUrl: './edit-effect.component.html',
})
export class EditEffectComponent implements OnInit {
    public effect: Object;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _effectService: EffectService) {
        this.effect = {};
    }

    edit() {
        this._effectService.editEffect(this.effect).subscribe(
            effect => {
                this._effectService.clearCacheCategory(effect.category);
                this._router.navigate(['/database/effects'], {queryParams: {id: effect.category}});
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
