import {Component, Input, Output, EventEmitter, OnInit, OnChanges} from '@angular/core';

import {Origin} from './origin.model';
import {OriginService} from './origin.service';
import {generateAllStatsPair, getRandomInt} from '../shared';

@Component({
    selector: 'origin-selector',
    templateUrl: './origin-selector.component.html',
    styleUrls: ['./origin-selector.component.scss'],
})
export class OriginSelectorComponent implements OnInit, OnChanges {
    @Input('cou') cou: number;
    @Input('cha') cha: number;
    @Input('int') int: number;
    @Input('ad') ad: number;
    @Input('fo') fo: number;

    @Input() selectedOrigin?: Origin;
    @Output() originChange: EventEmitter<Origin> = new EventEmitter<Origin>();
    @Output() swapStats: EventEmitter<string[]> = new EventEmitter<string[]>();

    public stats: { [statName: string]: number } = {};
    public origins: Origin[] = [];
    public originsStates: { [originId: number]: { state: string, changes?: any[] } };
    public swapList: string[][];

    static isOriginValid(origin: Origin, stats: { [statName: string]: number }): boolean {
        for (let req of origin.requirements) {
            let statName = req.stat.toLowerCase();
            let statValue = stats[statName];
            if (statValue) {
                if (req.min && statValue < req.min) {
                    return false;
                }
                if (req.max && statValue > req.max) {
                    return false;
                }
            }
        }
        return true;
    }

    constructor(private _originService: OriginService) {
        this.swapList = generateAllStatsPair();
    }

    updateOriginStates() {
        this.updateStats();
        let originsStates = {};
        for (let origin of this.origins) {
            if (OriginSelectorComponent.isOriginValid(origin, this.stats)) {
                originsStates[origin.id] = {state: 'ok'};
            }
            else {
                let validSwap: string[][] = [];
                for (let swap of this.swapList) {
                    let testStats = Object.assign({}, this.stats);
                    let tmp = testStats[swap[0]];
                    testStats[swap[0]] = testStats[swap[1]];
                    testStats[swap[1]] = tmp;
                    if (OriginSelectorComponent.isOriginValid(origin, testStats)) {
                        validSwap.push(swap);
                    }
                }
                if (validSwap.length) {
                    originsStates[origin.id] = {state: 'swap', changes: validSwap};
                } else {
                    originsStates[origin.id] = {state: 'ko'};
                }
            }
        }
        this.originsStates = originsStates;
    }

    doSwapStats(change: string[]) {
        this.swapStats.emit(change);
    }

    selectOrigin(origin: Origin) {
        if (this.originsStates[origin.id].state === 'ok') {
            this.selectedOrigin = origin;
            this.originChange.emit(origin);
        }
        return false;
    }


    getOrigins() {
        this._originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
                this.updateOriginStates();
            },
            err => {
                console.log(err);
            }
        );
    }

    randomSelect(): void {
        let count = 0;
        for (let i = 0; i < this.origins.length; i++) {
            if (this.originsStates[this.origins[i].id].state === 'ok') {
                count++;
            }
        }
        let rnd = getRandomInt(1, count);
        count = 0;
        for (let i = 0; i < this.origins.length; i++) {
            if (this.originsStates[this.origins[i].id].state === 'ok') {
                count++;
                if (count === rnd) {
                    this.selectOrigin(this.origins[i]);
                }
            }
        }
    }

    private updateStats() {
        this.stats = {
            cou: this.cou,
            cha: this.cha,
            fo: this.fo,
            ad: this.ad,
            int: this.int
        };
    }

    ngOnChanges() {
        this.updateOriginStates();
    }

    ngOnInit() {
        this.getOrigins();
    }
}
