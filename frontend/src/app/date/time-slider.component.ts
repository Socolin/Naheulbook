import {ChangeDetectionStrategy, Component, computed, effect, ElementRef, input, output, signal, viewChild} from '@angular/core';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';

import {NhbkDate, NhbkDateOffset} from './date.model';
import {date2Timestamp, duration2text, NhbkDateUtil} from './util';

/** Position of the "current time" on the track, in percent */
const START_PERCENT = 10;
const STEP_MINUTES = 15;

@Component({
    selector: 'time-slider',
    templateUrl: './time-slider.component.html',
    styleUrls: ['./time-slider.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [MatButton, MatIconButton, MatIcon],
})
export class TimeSliderComponent {
    readonly date = input<NhbkDate | undefined>();
    /** Amount of time covered by the slider between the start position and the right edge */
    readonly maxHours = input(25);
    readonly timeAdded = output<NhbkDateOffset>();

    private readonly track = viewChild.required<ElementRef<HTMLElement>>('track');

    readonly steps = signal(0);
    readonly dragging = signal(false);
    /**
     * Hour of day displayed at the start position. Kept in [24, 48) when idle (so there is always
     * a full cycle on the left), and allowed to go above while animating forward.
     */
    readonly backgroundHour = signal<number | undefined>(undefined);
    readonly animateBackground = signal(false);

    readonly startPercent = START_PERCENT;
    readonly hourTicks = Array.from({length: 24}, (_, i) => i);

    readonly maxSteps = computed(() => Math.round(this.maxHours() * 60 / STEP_MINUTES));
    readonly thumbPercent = computed(() => START_PERCENT + this.steps() / this.maxSteps() * (100 - START_PERCENT));
    /** Width of one 24h cycle relative to the track width */
    readonly cycleWidthPercent = computed(() => 24 * (100 - START_PERCENT) / this.maxHours());
    /**
     * Number of day/night cycles rendered in the background strip, enough to cover the track while
     * the background hour goes up to 48 + maxHours during an animation.
     */
    readonly cycles = computed(() => {
        const cycleWidth = this.cycleWidthPercent();
        const maxBackgroundHour = 48 + this.maxHours();
        const count = Math.ceil((100 - START_PERCENT + maxBackgroundHour / 24 * cycleWidth) / cycleWidth);
        return Array.from({length: count}, (_, i) => i);
    });
    readonly stripLeftPercent = computed(() => {
        const hour = this.backgroundHour() ?? 24;
        return START_PERCENT - hour / 24 * this.cycleWidthPercent();
    });

    readonly durationText = computed(() => duration2text(this.steps() * STEP_MINUTES * NhbkDateUtil.minuteDuration));
    readonly targetTimeText = computed(() => {
        const date = this.date();
        if (!date) {
            return '';
        }
        const totalMinutes = date.hour * 60 + date.minute + this.steps() * STEP_MINUTES;
        const dayOffset = Math.floor(totalMinutes / (24 * 60));
        const minutesOfDay = totalMinutes % (24 * 60);
        const hh = Math.floor(minutesOfDay / 60).toString().padStart(2, '0');
        const mm = (minutesOfDay % 60).toString().padStart(2, '0');
        let text = `${hh}:${mm}`;
        if (dayOffset === 1) {
            text += ' (lendemain)';
        } else if (dayOffset > 1) {
            text += ` (+${dayOffset} jours)`;
        }
        return text;
    });

    private previousTimestamp?: number;

    constructor() {
        effect(() => {
            const date = this.date();
            if (!date) {
                return;
            }
            this.syncBackground(date);
        });
    }

    private syncBackground(date: NhbkDate) {
        const hourOfDay = date.hour + date.minute / 60;
        const timestamp = date2Timestamp(date);
        const previousTimestamp = this.previousTimestamp;
        this.previousTimestamp = timestamp;

        const current = this.backgroundHour();
        if (current === undefined || previousTimestamp === undefined) {
            this.animateBackground.set(false);
            this.backgroundHour.set(24 + hourOfDay);
            return;
        }
        const diff = (timestamp - previousTimestamp) / NhbkDateUtil.hourDuration;
        if (diff === 0) {
            return;
        }
        if (diff > 0 && diff <= this.maxHours()) {
            // Moving forward: animate the background scrolling to the new time
            this.animateBackground.set(true);
            this.backgroundHour.set(current + diff);
        } else {
            this.animateBackground.set(false);
            this.backgroundHour.set(24 + hourOfDay);
        }
    }

    onBackgroundTransitionEnd(event: TransitionEvent) {
        if (event.target !== event.currentTarget) {
            return;
        }
        const hour = this.backgroundHour();
        if (hour !== undefined && hour >= 48) {
            // Snap back whole cycles (visually identical) so the strip never runs out
            this.animateBackground.set(false);
            this.backgroundHour.set(24 + hour % 24);
        }
    }

    onPointerDown(event: PointerEvent) {
        if (event.button !== 0) {
            return;
        }
        (event.currentTarget as HTMLElement).setPointerCapture(event.pointerId);
        this.dragging.set(true);
        this.updateFromPointer(event);
    }

    onPointerMove(event: PointerEvent) {
        if (!this.dragging()) {
            return;
        }
        this.updateFromPointer(event);
    }

    onPointerUp(event: PointerEvent) {
        if (!this.dragging()) {
            return;
        }
        this.dragging.set(false);
        (event.currentTarget as HTMLElement).releasePointerCapture(event.pointerId);
    }

    onKeyDown(event: KeyboardEvent) {
        let steps = this.steps();
        switch (event.key) {
            case 'ArrowRight':
            case 'ArrowUp':
                steps++;
                break;
            case 'ArrowLeft':
            case 'ArrowDown':
                steps--;
                break;
            case 'PageUp':
                steps += 4;
                break;
            case 'PageDown':
                steps -= 4;
                break;
            case 'Home':
                steps = 0;
                break;
            case 'End':
                steps = this.maxSteps();
                break;
            case 'Enter':
                this.validate();
                event.preventDefault();
                return;
            default:
                return;
        }
        event.preventDefault();
        this.steps.set(Math.max(0, Math.min(this.maxSteps(), steps)));
    }

    private updateFromPointer(event: PointerEvent) {
        const rect = this.track().nativeElement.getBoundingClientRect();
        const percent = (event.clientX - rect.left) / rect.width * 100;
        const ratio = (percent - START_PERCENT) / (100 - START_PERCENT);
        const steps = Math.round(ratio * this.maxSteps());
        this.steps.set(Math.max(0, Math.min(this.maxSteps(), steps)));
    }

    reset() {
        this.steps.set(0);
    }

    validate() {
        const steps = this.steps();
        if (steps <= 0) {
            return;
        }
        const totalMinutes = steps * STEP_MINUTES;
        const offset = new NhbkDateOffset();
        offset.hour = Math.floor(totalMinutes / 60);
        offset.minute = totalMinutes % 60;
        this.steps.set(0);
        this.timeAdded.emit(offset);
    }
}
