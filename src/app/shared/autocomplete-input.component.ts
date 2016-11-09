import {Component, Input, Output, EventEmitter} from '@angular/core';

export class AutocompleteValue {
    constructor(value: any, text: string) {
        this.value = value;
        this.text = text;
    }

    value: any;
    text: string;
}

@Component({
    selector: 'autocomplete-input',
    templateUrl: 'autocomplete-input.component.html'
})
export class AutocompleteInputComponent {
    @Input() callback: Function;
    @Input() value: string;
    @Input() clearOnSelect: boolean = false;
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();
    private values: AutocompleteValue[];
    private preSelectedValueIndex: number;

    selectValue(value: any) {
        this.onSelect.emit(value.value);
        if (this.clearOnSelect) {
            this.value = "";
        }
        else {
            this.value = value.text;
        }
        this.values = null;
        return false;
    }

    close() {
        this.values = null;
    }

    onKey(event: KeyboardEvent) {
        if (event.keyCode === 27) {
            this.close();
        }
        else if (event.keyCode === 38) {
            if (this.preSelectedValueIndex > 0)
                this.preSelectedValueIndex--;
        }
        else if (event.keyCode === 40) {
            if (this.values && this.preSelectedValueIndex < this.values.length) {
                this.preSelectedValueIndex++;
            }
        }
        else if (event.keyCode === 13) {
            if (this.values && this.preSelectedValueIndex >= 0 && this.preSelectedValueIndex < this.values.length) {
                this.selectValue(this.values[this.preSelectedValueIndex]);
            }
        }
        else if (event.keyCode === 9) {
            if (this.values && this.preSelectedValueIndex >= 0 && this.preSelectedValueIndex < this.values.length) {
                this.selectValue(this.values[this.preSelectedValueIndex]);
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
            values => {
                this.values = values;
                this.preSelectedValueIndex = 0;
            },
            err => {
                console.log(err);
            }
        )
        ;
    }
}
