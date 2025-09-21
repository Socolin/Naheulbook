import {Component, Input, Output, EventEmitter, OnInit, OnChanges} from '@angular/core';

import {Origin} from './origin.model';
import {OriginService} from './origin.service';
import {generateAllStatsPair, getRandomInt} from '../shared';

type OriginAvailability = {
    title: string,
    state: 'ok' | 'ko' | 'swap',
    icon: string,
    origins: Origin[]
};

@Component({
    selector: 'origin-selector',
    templateUrl: './origin-selector.component.html',
    styleUrls: ['./origin-selector.component.scss'],
    standalone: false
})
export class OriginSelectorComponent implements OnInit, OnChanges {
    @Input() cou: number;
    @Input() cha: number;
    @Input() int: number;
    @Input() ad: number;
    @Input() fo: number;

    @Input() selectedOrigin?: Origin;
    @Output() originChange: EventEmitter<Origin> = new EventEmitter<Origin>();
    @Output() swapStats: EventEmitter<string[]> = new EventEmitter<string[]>();

    public stats: { [statName: string]: number } = {};
    public origins: Origin[] = [];
    public originsStates: { [originId: string]: { changes?: any[] } };
    public swapList: string[][];
    public availabilityOk: OriginAvailability;
    public availabilities: OriginAvailability[] = [];

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

    constructor(
        private readonly originService: OriginService
    ) {
        this.swapList = generateAllStatsPair();
    }

    updateOriginStates() {
        this.updateStats();
        const {availabilityOk, availabilitySwap, availabilityKo} = this.createAvailabilities();

        const originsStates = {};
        for (let origin of this.origins) {
            if (OriginSelectorComponent.isOriginValid(origin, this.stats)) {
                availabilityOk.origins.push(origin);
            } else {
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
                    availabilitySwap.origins.push(origin);
                    originsStates[origin.id] = {changes: validSwap};
                } else {
                    availabilityKo.origins.push(origin);
                }
            }
        }
        this.originsStates = originsStates;
        this.availabilities = [
            availabilityOk,
            availabilitySwap,
            availabilityKo
        ];
        this.availabilityOk = availabilityOk;
    }

    private createAvailabilities() {
        const availabilityOk = {
            title: 'Origines disponibles',
            state: 'ok',
            icon: 'check',
            origins: []
        } as OriginAvailability;
        const availabilitySwap = {
            title: 'Origines disponible en inversant deux caractéristiques',
            state: 'swap',
            icon: 'sync',
            origins: []
        } as OriginAvailability;
        const availabilityKo = {
            title: 'Origines non disponibles',
            state: 'ko',
            icon: 'close',
            origins: []
        } as OriginAvailability;
        return {availabilityOk, availabilitySwap, availabilityKo};
    }

    doSwapStats(change: string[]) {
        this.swapStats.emit(change);
    }

    selectOrigin(origin: Origin): void {
        this.selectedOrigin = origin;
        this.originChange.emit(origin);
    }

    getOrigins() {
        this.originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
                this.updateOriginStates();
            }
        );
    }

    randomSelect(): void {
        const count = this.availabilityOk.origins.length;
        const rnd = getRandomInt(1, count);
        this.selectOrigin(this.availabilityOk.origins[rnd - 1]);
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
