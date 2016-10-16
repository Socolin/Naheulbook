import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

import {SharedModule} from "../shared/shared.module";

import {EditLocationComponent, LocationComponent, LocationEditorComponent, LocationListComponent} from "./";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule
    ],
    declarations: [
        EditLocationComponent,
        LocationComponent,
        LocationEditorComponent,
        LocationListComponent,
    ],
})
export class LocationModule {
}
