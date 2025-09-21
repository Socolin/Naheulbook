import {Component, ElementRef, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {IconDescription} from './icon.model';
import { MatFormField } from '@angular/material/form-field';
import { CdkOverlayOrigin, CdkConnectedOverlay } from '@angular/cdk/overlay';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatNavList, MatListItem } from '@angular/material/list';
import { IconComponent } from './icon.component';
import { MatLine } from '@angular/material/grid-list';
import { MatIcon } from '@angular/material/icon';

export class AutocompleteValue {
    public value: any;
    public text: string;
    public secondaryText?: string;
    public icon?: IconDescription;
    public mdIcon?: string;

    constructor(value: any, text: string, secondaryText?: string, icon?: IconDescription, mdIcon?: string) {
        this.value = value;
        this.text = text;
        this.secondaryText = secondaryText;
        this.icon = icon;
        this.mdIcon = mdIcon;
    }
}

@Component({
    selector: 'autocomplete-input',
    templateUrl: './autocomplete-input.component.html',
    styleUrls: ['./autocomplete-input.component.scss'],
    imports: [MatFormField, CdkOverlayOrigin, MatInput, FormsModule, CdkConnectedOverlay, MatNavList, MatListItem, IconComponent, MatLine, MatIcon]
})
export class AutocompleteInputComponent {
    @Input() callback: Function;
    @Input() value: string;
    @Input() placeholder: string;
    @Input() disabled = false;
    @Input() clearOnSelect = false;
    @Output() selected: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('inputField', {static: true})
    public inputField: ElementRef;

    public matchingValues?: AutocompleteValue[];
    public preSelectedValueIndex: number;

    selectValue(value: any) {
        this.selected.emit(value.value);
        if (this.clearOnSelect) {
            this.value = '';
        }
        else {
            this.value = value.text;
        }
        this.matchingValues = undefined;
        return false;
    }

    close() {
        this.matchingValues = undefined;
    }

    onKey(event: KeyboardEvent) {
        if (event.keyCode === 27) {
            this.close();
            this.inputField.nativeElement.blur();
            return;
        }
        else if (event.keyCode === 38) {
            if (this.preSelectedValueIndex > 0) {
                this.preSelectedValueIndex--;
            }
        }
        else if (event.keyCode === 40) {
            if (this.matchingValues && this.preSelectedValueIndex < this.matchingValues.length) {
                this.preSelectedValueIndex++;
            }
        }
        else if (event.keyCode === 13) {
            if (this.matchingValues && this.preSelectedValueIndex >= 0 && this.preSelectedValueIndex < this.matchingValues.length) {
                this.selectValue(this.matchingValues[this.preSelectedValueIndex]);
            }
        }
        else if (event.keyCode === 9) {
            if (this.matchingValues && this.preSelectedValueIndex >= 0 && this.preSelectedValueIndex < this.matchingValues.length) {
                this.selectValue(this.matchingValues[this.preSelectedValueIndex]);
            }
        }
        else {
            return;
        }
        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
    }

    updateList() {
        this.callback(this.value).subscribe(
            matchingValues => {
                this.matchingValues = matchingValues;
                this.preSelectedValueIndex = 0;
            }
        );
    }
}
