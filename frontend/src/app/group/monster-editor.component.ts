import {Component, Input, OnInit} from '@angular/core';
import { UntypedFormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/autocomplete';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';

@Component({
    selector: 'app-monster-editor',
    templateUrl: './monster-editor.component.html',
    styleUrls: ['./monster-editor.component.scss'],
    imports: [FormsModule, ReactiveFormsModule, MatFormField, MatLabel, MatInput, MatCheckbox, MatSelect, MatOption, CdkTextareaAutosize]
})
export class MonsterEditorComponent implements OnInit {
    @Input()
    form: UntypedFormGroup;

    constructor() {
    }

    ngOnInit() {
    }

}
