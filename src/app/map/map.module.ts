import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {MatSidenavModule} from '@angular/material/sidenav';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatIconModule} from '@angular/material/icon';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDialogModule} from '@angular/material/dialog';
import {MatSelectModule} from '@angular/material/select';
import {MatMenuModule} from '@angular/material/menu';

import {routes} from './map.routes';

import 'leaflet';
import 'leaflet.path.drag';
import 'leaflet-editable';
import 'leaflet-geometryutil';
import 'leaflet-almostover';

import {CreateMapComponent, MapComponent, MapService} from '.';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MapLayerDialogComponent} from './map-layer-dialog.component';
import {SelectMarkerTypeDialogComponent} from './select-marker-type-dialog.component';
import {MatRadioModule} from '@angular/material/radio';
import {SharedModule} from '../shared/shared.module';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MapHomeComponent} from './map-home.component';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatListModule} from '@angular/material/list';
import {MapListComponent} from './map-list.component';
import {EditMapComponent} from './edit-map.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MapMarkerLinkDialogComponent} from './map-marker-link-dialog.component';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatRippleModule} from '@angular/material/core';

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
    ],
    providers: [
        MapService,
    ],
    entryComponents: [
        MapLayerDialogComponent,
        MapMarkerLinkDialogComponent,
        SelectMarkerTypeDialogComponent,
    ]
})
export class MapModule {
}
