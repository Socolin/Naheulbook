import {Component} from '@angular/core';

@Component({
    selector: 'usefull-data',
    styleUrls: ['./usefull-data.component.css'],
    templateUrl: './usefull-data.component.html',
})
export class UsefullDataComponent {

    private currentPanel: string = null;
    private currentSubPanel: string = null;
    private selectedSubPanels = {};

    toggleDisplayPanel(name: string) {
        if (this.currentPanel === name) {
            this.currentPanel = null;
        } else {
            if (name) {
                this.currentSubPanel = this.selectedSubPanels[name];
            }
            this.currentPanel = name;
        }
        return false;
    }

    selectSubPanel(name) {
        this.currentSubPanel = name;
        this.selectedSubPanels[this.currentPanel] = name;
        return false;
    }

    private effectsCategoryId = 1;

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        this.currentPanel = 'effects';
        return false;
    }
}
