import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {
    NhbkAction,
    NhbkActionType, NhbkAddCustomModifierAction, NhbkAddEaAction,
    NhbkAddEffectAction, NhbkAddEvAction,
    NhbkAddItemAction, NhbkCustomAction,
    NhbkRemoveItemAction
} from './nhbk-action.model';
import {EffectService} from '../effect';
import {Effect} from '../effect';
import {duration2text} from '../date/util';
import {ActiveStatsModifier} from '../shared';

@Component({
    selector: 'nhbk-action',
    styleUrls: ['./nhbk-action.component.scss'],
    templateUrl: './nhbk-action.component.html'
})
export class NhbkActionComponent implements OnInit, OnChanges {
    @Input() action: NhbkAction;

    public effectInfo: Effect;
    public effectDuration: string;

    constructor(
        private readonly effectService: EffectService,
    ) {
    }

    public asNhbkAddItemAction(action: NhbkAction): NhbkAddItemAction { return action as NhbkAddItemAction; }
    public asNhbkRemoveItemAction(action: NhbkAction): NhbkRemoveItemAction { return action as NhbkRemoveItemAction; }
    public asNhbkAddEffectAction(action: NhbkAction): NhbkAddEffectAction { return action as NhbkAddEffectAction; }
    public asNhbkAddCustomModifierAction(action: NhbkAction): NhbkAddCustomModifierAction { return action as NhbkAddCustomModifierAction; }
    public asNhbkAddEvAction(action: NhbkAction): NhbkAddEvAction { return action as NhbkAddEvAction; }
    public asNhbkAddEaAction(action: NhbkAction): NhbkAddEaAction { return action as NhbkAddEaAction; }
    public asNhbkCustomAction(action: NhbkAction): NhbkCustomAction { return action as NhbkCustomAction; }

    private updateInfos() {
        if (this.action.type === NhbkActionType.addEffect) {
            if (!this.effectInfo || this.action.data.effectId !== this.effectInfo.id) {
                if (this.action.data.effectId) {
                    this.effectService.getEffect(this.action.data.effectId).subscribe(
                        effect => {
                            this.effectInfo = effect;
                            this.updateEffectInfo();
                        }
                    );
                }
            }
        } else if (this.action.type === NhbkActionType.addCustomModifier) {
            this.updateModifierInfo();
        }
    }

    private updateEffectDuration(effect: any) {
        switch (effect.durationType) {
            case 'combat':
                if (effect.combatCount > 1) {
                    this.effectDuration = effect.combatCount + ' combats';
                } else {
                    this.effectDuration = effect.combatCount + ' combat';
                }
                break;
            case 'lap':
                if (effect.lap > 1) {
                    this.effectDuration = effect.lapCount + ' tours';
                } else {
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
        if (this.action.type !== NhbkActionType.addCustomModifier) {
            throw `Invalid action type: ${this.action.type}`;
        }

        let modifier: ActiveStatsModifier = this.action.data.modifier;
        this.updateEffectDuration(modifier);
    }

    private updateEffectInfo() {
        if (this.action.type !== NhbkActionType.addEffect) {
            throw `Invalid action type: ${this.action.type}`;
        }

        if (this.action.data.effectData.durationType) {
            this.updateEffectDuration(this.action.data.effectData);
        } else {
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
