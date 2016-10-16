import {Component, Input, OnInit} from '@angular/core';

import {EffectService} from "./effect.service";
import {Effect, EffectCategory} from './effect.model';

@Component({
    selector: 'effect-editor',
    templateUrl: 'effect-editor.component.html',
})
export class EffectEditorComponent implements OnInit {
    @Input() effect: Effect;
    public categories: EffectCategory[];

    constructor(private _effectService: EffectService) {
        this.effect = new Effect();
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
        });
    }
}
