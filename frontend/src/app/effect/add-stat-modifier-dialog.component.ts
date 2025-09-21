import {Component, OnInit} from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

import {ItemStatModifier, MiscService } from '../shared';
import {StatModificationOperand} from '../api/shared/enums';

@Component({
    selector: 'app-add-stat-modifier-dialog',
    templateUrl: './add-stat-modifier-dialog.component.html',
    styleUrls: ['./add-stat-modifier-dialog.component.scss'],
    standalone: false
})
export class AddStatModifierDialogComponent implements OnInit {
    public selectedStat?: string;
    public selectedType: StatModificationOperand = 'ADD';
    public value?: number;

    public stats: string[];
    public basicStats: string[] = ['AD', 'CHA', 'COU', 'INT', 'FO'];
    public combatStats: string[] = ['AT', 'PRD', 'PR', 'PR_MAGIC', 'ESQ'];
    public lifeStats: string[] = ['EA', 'EV'];
    public magicStats: string[] = ['MPHYS', 'MPSY', 'RESM'];

    constructor(
        private readonly dialogRef: MatDialogRef<AddStatModifierDialogComponent>,
        private readonly miscService: MiscService,
    ) {
    }

    ngOnInit() {
        this.miscService.getStats().subscribe(stats => {
            let filteredStats = stats.map(s => s.name);
            filteredStats = filteredStats.filter(s => {
                return this.basicStats.indexOf(s) === -1
                    && this.combatStats.indexOf(s) === -1
                    && this.lifeStats.indexOf(s) === -1
                    && this.magicStats.indexOf(s) === -1;
            });
            this.stats = filteredStats;
        });
    }

    canAddModifier(): boolean {
        return this.selectedStat !== undefined  && this.value !== undefined;
    }

    addModifier() {
        if (!this.canAddModifier()) {
            return;
        }

        let modifier = new ItemStatModifier();
        // FIXME: Could assert not null with `asserts` typescript 3.7 and remove !
        modifier.stat = this.selectedStat!;
        modifier.value = this.value!;
        modifier.type = this.selectedType;

        this.dialogRef.close(modifier);
    }
}
