import {Directive, ElementRef, Input} from '@angular/core';
import {ItemTemplateCategory} from './item-template.model';

@Directive({
    selector: '[itemCategory]'
})
export class ItemCategoryDirective {
    @Input() category: ItemTemplateCategory;
    constructor(public elementRef: ElementRef) {
    }
}
