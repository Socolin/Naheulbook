import {Component, Input, OnInit} from '@angular/core';
import {UntypedFormGroup} from '@angular/forms';

@Component({
    selector: 'app-monster-editor',
    templateUrl: './monster-editor.component.html',
    styleUrls: ['./monster-editor.component.scss'],
    standalone: false
})
export class MonsterEditorComponent implements OnInit {
    @Input()
    form: UntypedFormGroup;

    constructor() {
    }

    ngOnInit() {
    }

}
