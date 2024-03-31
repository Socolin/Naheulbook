import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {MatLegacyAutocompleteModule as MatAutocompleteModule} from '@angular/material/legacy-autocomplete';
import {MatLegacyButtonModule as MatButtonModule} from '@angular/material/legacy-button';
import {MatLegacyCheckboxModule as MatCheckboxModule} from '@angular/material/legacy-checkbox';
import {MatLegacyDialogModule as MatDialogModule} from '@angular/material/legacy-dialog';
import {MatLegacyFormFieldModule as MatFormFieldModule} from '@angular/material/legacy-form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatLegacyInputModule as MatInputModule} from '@angular/material/legacy-input';
import {MatLegacyListModule as MatListModule} from '@angular/material/legacy-list';
import {MatLegacyMenuModule as MatMenuModule} from '@angular/material/legacy-menu';
import {MatLegacyProgressBarModule as MatProgressBarModule} from '@angular/material/legacy-progress-bar';
import {MatLegacyProgressSpinnerModule as MatProgressSpinnerModule} from '@angular/material/legacy-progress-spinner';
import {MatLegacyRadioModule as MatRadioModule} from '@angular/material/legacy-radio';
import {MatRippleModule} from '@angular/material/core';
import {MatLegacySelectModule as MatSelectModule} from '@angular/material/legacy-select';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatLegacyTooltipModule as MatTooltipModule} from '@angular/material/legacy-tooltip';

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
