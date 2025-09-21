import {Component} from '@angular/core';
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {MatFormField} from '@angular/material/form-field';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatInput} from '@angular/material/input';
import {MatButton} from '@angular/material/button';

export type CreateFightDialogResult = {
    name: string,
}

@Component({
    selector: 'app-create-fight-dialog',
    imports: [
        MatDialogContent,
        MatDialogTitle,
        MatDialogActions,
        MatFormField,
        FormsModule,
        MatInput,
        ReactiveFormsModule,
        MatButton,
        MatDialogClose
    ],
    templateUrl: './create-fight-dialog.component.html',
    styleUrl: './create-fight-dialog.component.scss'
})
export class CreateFightDialogComponent {
    name: string = '';

    constructor(
        private readonly dialogRef: MatDialogRef<CreateFightDialogComponent>,
    ) {
    }

    validate() {
        this.dialogRef.close({
            name: this.name,
        })
    }
}
