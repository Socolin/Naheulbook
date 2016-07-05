import {Component, Input, Output, EventEmitter, OnInit, OnChanges} from '@angular/core';

import {Origin} from "./origin.model";
import {StatRequirement} from "../shared/stat-requirement.model";
import {OriginService} from "./origin.service";

@Component({
    moduleId: module.id,
    selector: 'origin-selector',
    templateUrl: 'origin-selector.component.html'
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

    constructor(private _originService: OriginService) {
        this.stats = this;
        this.invalidStats = [];
    }

    isSelected(origin: Origin) {
        return this.selectedOrigin && this.selectedOrigin.id === origin.id;
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

    ngOnChanges() {
        this.updateInvalids();
    }

    ngOnInit() {
        this.getOrigins();
    }
}
