import {Component, Input, OnInit} from '@angular/core';
import {MdOptionSelectionChange} from '@angular/material';

import {EffectService} from './effect.service';
import {Effect, EffectCategory} from './effect.model';

@Component({
    selector: 'effect-editor',
    styleUrls: ['./effect-editor.component.scss'],
    templateUrl: './effect-editor.component.html',
})
export class EffectEditorComponent implements OnInit {
    @Input() effect: Effect;
    public categories: EffectCategory[];

    constructor(private _effectService: EffectService) {
        this.effect = new Effect();
    }

    selectCategory(event: MdOptionSelectionChange) {
        let i = this.categories.findIndex(c => c.id === event.source.value);
        if (i !== -1) {
            this.effect.category = this.categories[i];
        }
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
        });
    }
}
