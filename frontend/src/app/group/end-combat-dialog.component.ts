import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';

export class EndCombatDialogResult {
    decreaseCombatTimer: boolean;
}

@Component({
    selector: 'app-end-combat-dialog',
    templateUrl: './end-combat-dialog.component.html',
    styleUrls: ['./end-combat-dialog.component.scss']
})
export class EndCombatDialogComponent implements OnInit {
    public decreaseCombatTimer = true;

    constructor(
        private readonly dialogRef: MatDialogRef<EndCombatDialogComponent, EndCombatDialogResult>
    ) {
    }

    ngOnInit(): void {
    }

    confirm() {
        this.dialogRef.close({
            decreaseCombatTimer: this.decreaseCombatTimer
        })
    }
}
