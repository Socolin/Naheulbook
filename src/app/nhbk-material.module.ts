import {NgModule} from '@angular/core';

import {OverlayModule} from '@angular/cdk/overlay';
import {A11yModule} from '@angular/cdk/a11y';
import {BidiModule} from '@angular/cdk/bidi';
import {ObserversModule} from '@angular/cdk/observers';
import {PortalModule} from '@angular/cdk/portal';
import {
    MatAutocompleteModule, MatButtonModule, MatButtonToggleModule, MatCardModule, MatCheckboxModule,
    MatChipsModule, MatCommonModule, MatDatepickerModule, MatDialogModule, MatExpansionModule, MatFormFieldModule,
    MatGridListModule,
    MatIconModule, MatInputModule, MatListModule, MatMenuModule, MatPaginatorModule, MatProgressBarModule,
    MatProgressSpinnerModule, MatRadioModule, MatRippleModule, MatSelectModule, MatSidenavModule, MatSliderModule,
    MatSlideToggleModule, MatSnackBarModule, MatSortModule,
    MatTableModule, MatTabsModule, MatToolbarModule, MatTooltipModule
} from '@angular/material';
import {PlatformModule} from '@angular/cdk/platform';

const MATERIAL_MODULES = [
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatChipsModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatTableModule,
    MatDialogModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    OverlayModule,
    PortalModule,
    BidiModule,
    A11yModule,
    PlatformModule,
    MatCommonModule,
    ObserversModule,
];

@NgModule({
    imports: MATERIAL_MODULES,
    exports: MATERIAL_MODULES,
})
export class NhbkMaterialModule {}
