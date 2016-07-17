import {Component, Input, OnInit} from '@angular/core';

import {PlusMinusPipe, ModifiersEditorComponent} from "../shared";
import {EffectService} from "./effect.service";
import {Effect, EffectCategory} from './effect.model';

@Component({
    moduleId: module.id,
    selector: 'effect-editor',
    templateUrl: 'effect-editor.component.html',
    pipes: [PlusMinusPipe],
    directives: [ModifiersEditorComponent]
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
