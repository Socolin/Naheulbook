import {HammerGestureConfig} from '@angular/platform-browser';

export class CustomHammerConfig extends HammerGestureConfig {
    overrides = <any> {
        'pinch': {enable: false},
        'rotate': {enable: false}
    }
}
