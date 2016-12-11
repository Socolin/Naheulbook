import {Component, EventEmitter, Input, Output, OnInit} from '@angular/core';
import {Fighter} from './group.model';

@Component({
    selector: 'target-selector',
    templateUrl: 'target-selector.component.html',
    styleUrls: ['target-selector.component.css']
})
export class TargetSelectorComponent implements OnInit {
    @Input() fighter: Fighter;
    @Input() targets: Fighter[];
    @Output() onTargetChange: EventEmitter<Fighter> = new EventEmitter<Fighter>();
    public showSelector: boolean = false;
    public targetType: string = 'monster';

    hideSelector() {
        this.showSelector = false;
    }

    displaySelector() {
        this.showSelector = true;
    }

    setTargetType(targetType: string): void {
        this.targetType = targetType;
    }

    selectTarget(target: Fighter) {
        this.hideSelector();
        this.onTargetChange.emit(target);
    }

    isTargetShow(target: Fighter): boolean {
        if (target.isMonster && this.targetType === 'monster') {
            return true;
        }
        if (!target.isMonster && this.targetType === 'character') {
            return true;
        }
        return false;
    }

    ngOnInit(): void {
        if (!this.fighter.isMonster) {
            this.setTargetType('monster');
        }
        else {
            this.setTargetType('character');
        }
    }
}
