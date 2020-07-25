import {HammerGestureConfig} from '@angular/platform-browser';
import { Injectable } from "@angular/core";

@Injectable()
export class CustomHammerConfig extends HammerGestureConfig {
    overrides = <any> {
        'pinch': {enable: false},
        'rotate': {enable: false}
    }
}
