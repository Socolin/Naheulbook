import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {ItemTemplate, ItemSlot} from './item-template.model';

@Component({
    selector: 'item-template-editor-module',
    styleUrls: ['./item-template-editor-module.component.scss'],
    templateUrl: './item-template-editor-module.component.html'
})
export class ItemTemplateEditorModuleComponent implements OnInit {
    @Input() itemTemplate: ItemTemplate;
    @Input() moduleName: string;

    @Input() slots: ItemSlot[];

    @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

    isInSlot(slot) {
        if (!this.itemTemplate.slots) {
            return false;
        }
        for (let i = 0; i < this.itemTemplate.slots.length; i++) {
            if (this.itemTemplate.slots[i].id === slot.id) {
                return true;
            }
        }
        return false;
    }

    toggleSlot(slot) {
        if (!this.itemTemplate.slots) {
            this.itemTemplate.slots = [];
        }
        if (this.isInSlot(slot)) {
            for (let i = 0; i < this.itemTemplate.slots.length; i++) {
                if (this.itemTemplate.slots[i].id === slot.id) {
                    this.itemTemplate.slots.splice(i, 1);
                    break;
                }
            }
        } else {
            this.itemTemplate.slots.push(slot);
        }
    }

    deleteModule() {
        this.onDelete.emit(true);
    }

    ngOnInit(): void {

    }
}
