import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';

import {ItemStatModifier, MiscService} from '../shared';
import {StatModificationOperand} from '../api/shared/enums';
import {MatFormField, MatHint, MatLabel} from '@angular/material/form-field';
import {MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {MatOptgroup, MatOption} from '@angular/material/core';
import {MatInput} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {MatTooltip} from '@angular/material/tooltip';

@Component({
    selector: 'app-add-stat-modifier-dialog',
    templateUrl: './add-stat-modifier-dialog.component.html',
    styleUrls: ['./add-stat-modifier-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatFormField, MatLabel, MatSelect, FormsModule, MatOptgroup, MatOption, MatInput, MatButton, MatButtonToggleGroup, MatButtonToggle, MatTooltip, MatHint]
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

    public readonly operations: { type: StatModificationOperand, symbol: string, label: string }[] = [
        {type: 'ADD', symbol: '+', label: 'Ajouter / Soustraire'},
        {type: 'MUL', symbol: '×', label: 'Multiplier'},
        {type: 'DIV', symbol: '÷', label: 'Diviser'},
        {type: 'SET', symbol: '=', label: 'Remplacer par'},
    ];

    get statGroups(): { label: string, stats: string[] }[] {
        return [
            {label: 'Caractéristiques', stats: this.basicStats},
            {label: 'Énergie', stats: this.lifeStats},
            {label: 'Combat', stats: this.combatStats},
            {label: 'Magie', stats: this.magicStats},
            {label: 'Autres', stats: this.stats ?? []},
        ];
    }

    get operationSymbol(): string {
        return this.operations.find(o => o.type === this.selectedType)?.symbol ?? '';
    }

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
        return this.selectedStat !== undefined && this.value !== undefined && this.value !== null;
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
