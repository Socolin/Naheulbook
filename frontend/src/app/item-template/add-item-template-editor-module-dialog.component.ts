import {Component, Inject, OnInit} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {ItemTemplateModuleDefinition, itemTemplateModulesDefinitions} from './item-template-modules-definitions';

export interface AddItemTemplateEditorModuleDialogData {
    selectedModules: string[]
}

@Component({
    templateUrl: './add-item-template-editor-module-dialog.component.html',
    styleUrls: ['./add-item-template-editor-module-dialog.component.scss']
})
export class AddItemTemplateEditorModuleDialogComponent implements OnInit {
    public availableModules: ItemTemplateModuleDefinition[] = [];

    constructor(
        public dialogRef: MatDialogRef<AddItemTemplateEditorModuleDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: AddItemTemplateEditorModuleDialogData,
    ) {
    }

    ngOnInit() {
        this.availableModules = itemTemplateModulesDefinitions.filter(m => this.data.selectedModules.indexOf(m.name) === -1)
    }

    selectModule(module: ItemTemplateModuleDefinition) {
        this.dialogRef.close(module.name);
    }
}
