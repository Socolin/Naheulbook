import {Component, Input, OnInit, OnChanges, SimpleChanges} from '@angular/core';

import {EffectService} from "./effect.service";
import {Effect, EffectCategory} from './effect.model';
import {NhbkDateOffset} from "../date/date.model";
import {dateOffset2TimeDuration} from "../date/util";

@Component({
    selector: 'effect-editor',
    templateUrl: 'effect-editor.component.html',
})
export class EffectEditorComponent implements OnInit, OnChanges {
    @Input() effect: Effect;
    public categories: EffectCategory[];
    public effectDateOffset: NhbkDateOffset = new NhbkDateOffset();
    private effectDurationType: string = 'custom';

    constructor(private _effectService: EffectService) {
        this.effect = new Effect();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('effect' in changes) {
            if (this.effect.combatCount) {
                this.effectDurationType = 'combat';
            }
            else if (this.effect.timeDuration) {
                this.effectDurationType = 'time';
            }
            else {
                this.effectDurationType = 'custom';
            }
        }
    }
    setEffectDurationType(type: string) {
        this.effectDurationType = type;
        if (type == 'combat') {
            this.effect.timeDuration = null;
            this.effect.duration = null;
        }
        else if (type === 'time') {
            this.effect.combatCount = null;
            this.effect.duration = null;
        }
        else if (type === 'custom') {
            this.effect.combatCount = null;
            this.effect.timeDuration = null;
        }
        else {
            throw new Error("Invalid type: " + type);
        }
    }

    setEffectTimeDuration(dateOffset: NhbkDateOffset) {
        this.effect.timeDuration= dateOffset2TimeDuration(dateOffset);
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
        });
    }
}
