import {Component, OnInit, Input, OnChanges, SimpleChanges} from '@angular/core';
import {NhbkAction} from './nhbk-action.model';
import {EffectService} from '../effect/effect.service';
import {Effect} from '../effect/effect.model';
import {duration2text} from '../date/util';
import {CharacterModifier} from '../character/character.model';

@Component({
    selector: 'nhbk-action',
    styleUrls: ['./nhbk-action.component.scss'],
    templateUrl: './nhbk-action.component.html'
})
export class NhbkActionComponent implements OnInit, OnChanges {
    @Input() action: NhbkAction;

    public effectInfo: Effect;
    public effectDuration: string;

    constructor(private _effectService: EffectService) {
    }

    private updateInfos() {
        if (this.action.type === 'addEffect') {
            console.log(this.action);
            if (!this.effectInfo || this.action.data.effectId !== this.effectInfo.id) {
                if (this.action.data.effectId) {
                    this._effectService.getEffect(this.action.data.effectId).subscribe(
                        effect => {
                            this.effectInfo = effect;
                            this.updateEffectInfo();
                        }
                    );
                }
            }
        }
        else if (this.action.type === 'addCustomModifier') {
            this.updateModifierInfo();
        }
    }

    private updateEffectDuration(effect: any) {
        switch (effect.durationType) {
            case 'combat':
                if (effect.combatCount > 1) {
                    this.effectDuration = effect.combatCount + ' combats';
                }
                else {
                    this.effectDuration = effect.combatCount + ' combat';
                }
                break;
            case 'lap':
                if (effect.lap > 1) {
                    this.effectDuration = effect.lapCount + ' tours';
                }
                else {
                    this.effectDuration = effect.lapCount + ' tour';
                }
                break;
            case 'custom':
                this.effectDuration = effect.duration;
                break;
            case 'time':
                this.effectDuration = duration2text(effect.timeDuration);
                break;
            case 'forever':
                this.effectDuration = 'toujours';
                break;
        }
    }
    private updateModifierInfo() {
        let modifier: CharacterModifier = this.action.data.modifier;
        this.updateEffectDuration(modifier);
    }

    private updateEffectInfo() {
        if (this.action.data.effectData.durationType) {
            this.updateEffectDuration(this.action.data.effectData);
        }
        else {
            this.updateEffectDuration(this.effectInfo);
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.updateInfos();
    }

    ngOnInit(): void {
        this.updateInfos();
    }

}
