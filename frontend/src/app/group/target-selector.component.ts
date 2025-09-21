import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ConnectionPositionPair} from '@angular/cdk/overlay';

import {Fighter} from './group.model';

@Component({
    selector: 'target-selector',
    templateUrl: './target-selector.component.html',
    styleUrls: ['./target-selector.component.scss'],
    standalone: false
})
export class TargetSelectorComponent implements OnInit {
    @Input() fighter: Fighter;
    @Input() targets: Fighter[];
    @Output() targetChange: EventEmitter<Fighter> = new EventEmitter<Fighter>();
    public showSelector = false;
    public selectedTabIndex: number;

    public positions = [
        new ConnectionPositionPair(
            {originX: 'end', originY: 'top'},
            {overlayX: 'end', overlayY: 'bottom'}),
        new ConnectionPositionPair(
            {originX: 'start', originY: 'bottom'},
            {overlayX: 'start', overlayY: 'top'}),
    ];

    public selectorHeight = 0;
    public selectorMinHeight = 0;

    hideSelector() {
        this.showSelector = false;
    }

    displaySelector(event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.showSelector = true;
    }

    selectTarget(target: Fighter) {
        this.hideSelector();
        this.targetChange.emit(target);
    }

    ngOnInit(): void {
        if (!this.fighter.isMonster) {
            this.selectedTabIndex = 0;
        }
        else {
            this.selectedTabIndex = 1;
        }
        let monsterCount = this.targets.filter(a => a.isMonster).length;
        let playerCount = this.targets.filter(a => a.isMonster).length;
        let maxPerTab = Math.max(monsterCount, playerCount);
        let minPerTab = Math.min(monsterCount, playerCount);
        this.selectorMinHeight = 48 + 66 * Math.ceil(minPerTab / 6);
        this.selectorHeight = 48 + 66 * Math.ceil(maxPerTab / 6);
    }
}
