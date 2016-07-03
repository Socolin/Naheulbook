import {Component} from '@angular/core';

import {PlusMinusPipe, ModifiersEditorComponent} from "../shared";
import {EffectService} from "./effect.service";

@Component({
    selector: 'effect-editor',
    templateUrl: 'app/effect/effect-editor.component.html',
    inputs: ["effect"],
    pipes: [PlusMinusPipe],
    directives: [ModifiersEditorComponent]
})
export class EffectEditorComponent {
    public categories: any[];
    public effect: Object;

    constructor(private _effectService: EffectService) {
        this.effect = {};
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
        });
    }
}
