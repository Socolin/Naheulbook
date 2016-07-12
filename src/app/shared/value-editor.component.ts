import {Component, Input, Output, EventEmitter, Renderer, ElementRef} from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'value-editor',
    template: `<span style="position: relative"(click)="showEditor()">
            {{value || 0}}
            <div [hidden]="!displayEditor" style="position: absolute; left:10px; bottom: 15px">
                <input class="form-control" 
                style="width: 40px;
                    padding: 0px; 
                    font-weight: bold;
                    font-size: large;
                    " 
                type="text" 
                [(ngModel)]="newValue" 
                (keyup.enter)="commitValue()">
            </div>
        </span>`,
})
export class ValueEditorComponent {
    @Input() value: string[] = [];
    @Output() onChange: EventEmitter<string> = new EventEmitter<string>();

    private newValue: string;
    private displayEditor: boolean = false;

    constructor(private _renderer: Renderer
        , private _elementRef: ElementRef) {
    }

    showEditor() {
        const element = this._elementRef.nativeElement.querySelector('input');
        setTimeout(
            () => this._renderer.invokeElementMethod(element, 'focus', [])
            , 0);
        this.displayEditor = !this.displayEditor;
    }

    commitValue() {
        this.displayEditor = false;
        this.onChange.emit(this.newValue);
        this.newValue = null;
    }
}
