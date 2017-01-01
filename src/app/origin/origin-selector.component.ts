import {Component, Input, Output, EventEmitter, OnInit, OnChanges} from '@angular/core';

import {Origin} from './origin.model';
import {StatRequirement} from '../shared';
import {OriginService} from './origin.service';
import {getRandomInt} from "../shared/random";

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

    @Input() selectedOrigin: Origin = null;
    @Output() originChange: EventEmitter<Origin> = new EventEmitter<Origin>();

    public origins: Origin[] = [];
    public invalidStats: Object;
    public stats: Object;
    public detail: {[originId: number]: boolean} = {};
    public viewNotAvailable: boolean = false;

    constructor(private _originService: OriginService) {
        this.stats = this;
        this.invalidStats = [];
    }

    updateInvalidStats(origin: Origin) {
        let invalids = [];
        if (!origin.requirements) {
            return invalids;
        }
        for (let i = 0; i < origin.requirements.length; i++) {
            let req: StatRequirement;
            req = origin.requirements[i];
            let statName = req.stat.toLowerCase();
            let statValue = this[statName];
            if (statValue) {
                if (req.min && statValue < req.min) {
                    invalids.push({stat: statName, min: req.min});
                }
                if (req.max && statValue > req.max) {
                    invalids.push({stat: statName, max: req.min});
                }
            }
        }
        return invalids;
    }

    isAvailable(origin: Origin) {
        if (this.selectedOrigin) {
            return (this.selectedOrigin.id === origin.id);
        }
        return !(this.invalidStats[origin.id] && this.invalidStats[origin.id].length);
    }

    selectOrigin(origin: Origin) {
        if (this.isAvailable(origin)) {
            this.selectedOrigin = origin;
            this.originChange.emit(origin);
        }
        return false;
    }

    updateInvalids() {
        for (let i = 0; i < this.origins.length; i++) {
            let origin = this.origins[i];
            this.invalidStats[origin.id] = this.updateInvalidStats(origin);
        }
    }

    getOrigins() {
        this._originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
                this.updateInvalids();
            },
            err => {
                console.log(err);
            }
        );
    }

    randomSelect(): void {
        let count = 0;
        for (let i = 0; i < this.origins.length; i++) {
            if (this.isAvailable(this.origins[i])) {
                count++;
            }
        }
        let rnd = getRandomInt(1, count);
        count = 0;
        for (let i = 0; i < this.origins.length; i++) {
            if (this.isAvailable(this.origins[i])) {
                count++;
                if (count == rnd) {
                    this.selectOrigin(this.origins[i]);
                }
            }
        }
    }

    toggleViewAll(): void {
        this.viewNotAvailable = !this.viewNotAvailable;
    }

    toggleDetail(origin: Origin) {
        this.detail[origin.id] = !this.detail[origin.id];
    }

    displayDetail(origin: Origin) {
        return this.detail[origin.id];
    }

    ngOnChanges() {
        this.updateInvalids();
    }

    ngOnInit() {
        this.getOrigins();
    }
}
