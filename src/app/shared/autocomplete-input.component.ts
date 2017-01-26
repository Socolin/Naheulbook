import {Component, Input, Output, EventEmitter, ViewChild, ElementRef} from '@angular/core';

export class AutocompleteValue {
    public value: any;
    public text: string;

    constructor(value: any, text: string) {
        this.value = value;
        this.text = text;
    }
}

@Component({
    selector: 'autocomplete-input',
    templateUrl: './autocomplete-input.component.html',
    styleUrls: ['./autocomplete-input.component.scss']
})
export class AutocompleteInputComponent {
    @Input() callback: Function;
    @Input() value: string;
    @Input() placeholder: string;
    @Input() disabled: boolean = false;
    @Input() clearOnSelect: boolean = false;
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('inputField')
    public inputField: ElementRef;

    public matchingValues: AutocompleteValue[];
    public preSelectedValueIndex: number;

    focus() {
        this.inputField.nativeElement.focus();
    }

    clear() {
        this.value = '';
    }

    selectValue(value: any) {
        this.onSelect.emit(value.value);
        if (this.clearOnSelect) {
            this.value = '';
        }
        else {
            this.value = value.text;
        }
        this.matchingValues = null;
        return false;
    }

    close() {
        this.matchingValues = null;
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
            },
            err => {
                console.log(err);
            }
        );
    }
}
