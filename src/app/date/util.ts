import {NhbkDateOffset} from "./date.model";

export class NhbkDateUtil {
    public static yearDuration: number = 365 * 24 * 3600;
    public static weekDuration: number = 7 * 24 * 3600;
    public static dayDuration: number = 24 * 3600;
    public static hourDuration: number = 3600;
    public static minuteDuration: number = 60;
}


export function dateOffset2TimeDuration (dateOffset: NhbkDateOffset): number {
    let duration: number = 0;

    duration += dateOffset.year * NhbkDateUtil.yearDuration;
    duration += dateOffset.week * NhbkDateUtil.weekDuration;
    duration += dateOffset.day * NhbkDateUtil.dayDuration;
    duration += dateOffset.hour * NhbkDateUtil.hourDuration;
    duration += dateOffset.minute * NhbkDateUtil.minuteDuration;

    return duration;
}

export function timeDuration2DateOffset2 (duration: number): NhbkDateOffset {
    let dateOffset: NhbkDateOffset = new NhbkDateOffset();

    dateOffset.year = Math.floor(duration / NhbkDateUtil.yearDuration);
    duration -= NhbkDateUtil.yearDuration * dateOffset.year;

    dateOffset.week = Math.floor(duration / NhbkDateUtil.weekDuration);
    duration -= NhbkDateUtil.weekDuration * dateOffset.week;
    dateOffset.day = Math.floor(duration / NhbkDateUtil.dayDuration);
    duration -= NhbkDateUtil.dayDuration * dateOffset.day;
    dateOffset.hour = Math.floor(duration / NhbkDateUtil.hourDuration);
    duration -= NhbkDateUtil.hourDuration * dateOffset.hour;
    dateOffset.minute = Math.floor(duration / NhbkDateUtil.minuteDuration);
    duration -= NhbkDateUtil.minuteDuration * dateOffset.minute;
    dateOffset.second = duration;

    return dateOffset;
}
