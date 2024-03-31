import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {MatIconModule} from '@angular/material/icon';
import {MatRippleModule} from '@angular/material/core';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbarModule} from '@angular/material/toolbar';

import {SharedModule} from '../shared/shared.module';
import {MarkdownModule} from '../markdown/markdown.module';

import {routes} from './map.routes';

import 'leaflet';
import 'leaflet.path.drag';
import 'leaflet-editable';
import 'leaflet-geometryutil';
import 'leaflet-almostover';

import {
    CreateMapComponent,
    EditMapComponent,
    MapComponent,
    MapHomeComponent,
    MapLayerDialogComponent,
    MapListComponent,
    MapMarkerLinkDialogComponent,
    MapService,
    SelectMarkerTypeDialogComponent,
} from '.';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatDialogModule} from '@angular/material/dialog';
import {MatSelectModule} from '@angular/material/select';
import {MatMenuModule} from '@angular/material/menu';
import {MatRadioModule} from '@angular/material/radio';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatListModule} from '@angular/material/list';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatAutocompleteModule} from '@angular/material/autocomplete';

@NgModule({
    declarations: [
        MapComponent,
        CreateMapComponent,
        MapLayerDialogComponent,
        SelectMarkerTypeDialogComponent,
        MapHomeComponent,
        MapListComponent,
        EditMapComponent,
        MapMarkerLinkDialogComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),

        MatSidenavModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        MatCheckboxModule,
        MatProgressBarModule,
        MatDialogModule,
        MatSelectModule,
        MatMenuModule,
        MatRadioModule,
        SharedModule,
        MatTooltipModule,
        MatToolbarModule,
        MatListModule,
        MatProgressSpinnerModule,
        MatAutocompleteModule,
        MatRippleModule,

        MarkdownModule
    ],
    providers: [
        MapService,
    ],
})
export class MapModule {
}
