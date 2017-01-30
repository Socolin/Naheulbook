import {NhbkDateOffset, NhbkDate} from './date.model';

export class NhbkDateUtil {
    public static yearDuration: number = 365 * 24 * 3600;
    public static weekDuration: number = 7 * 24 * 3600;
    public static dayDuration: number = 24 * 3600;
    public static hourDuration: number = 3600;
    public static minuteDuration: number = 60;
}


export function dateOffset2TimeDuration (dateOffset: NhbkDateOffset): number {
    let duration = 0;

    duration += dateOffset.year * NhbkDateUtil.yearDuration;
    duration += dateOffset.week * NhbkDateUtil.weekDuration;
    duration += dateOffset.day * NhbkDateUtil.dayDuration;
    duration += dateOffset.hour * NhbkDateUtil.hourDuration;
    duration += dateOffset.minute * NhbkDateUtil.minuteDuration;

    return duration;
}


export function date2Timestamp (date: NhbkDate): number {
    let timestamp = 0;

    timestamp += date.year * NhbkDateUtil.yearDuration;
    timestamp += date.day * NhbkDateUtil.dayDuration;
    timestamp += date.hour * NhbkDateUtil.hourDuration;
    timestamp += date.minute * NhbkDateUtil.minuteDuration;

    return timestamp;
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


export function timestamp2Date (duration: number): NhbkDate {
    let date: NhbkDate = new NhbkDate();

    date.year = Math.floor(duration / NhbkDateUtil.yearDuration);
    duration -= NhbkDateUtil.yearDuration * date.year;

    date.day = Math.floor(duration / NhbkDateUtil.dayDuration);
    duration -= NhbkDateUtil.dayDuration * date.day;
    date.hour = Math.floor(duration / NhbkDateUtil.hourDuration);
    duration -= NhbkDateUtil.hourDuration * date.hour;
    date.minute = Math.floor(duration / NhbkDateUtil.minuteDuration);

    return date;
}

export function duration2text(duration: number|NhbkDateOffset) {
    let dateOffset;

    if (!duration) {
        return '';
    }

    if (typeof duration === 'number') {
        dateOffset = timeDuration2DateOffset2(duration);
    } else {
        dateOffset = duration;
    }

    let result = '';
    if (dateOffset.year > 1) {
        result += dateOffset.year + ' ans ';
    }
    else if (dateOffset.year === 1) {
        result += dateOffset.year + ' an ';
    }

    if (dateOffset.week > 1) {
        result += dateOffset.week + ' semaines ';
    }
    else if (dateOffset.week === 1) {
        result += dateOffset.week + ' semaine ';
    }

    if (dateOffset.day > 1) {
        result += dateOffset.day + ' jours ';
    }
    else if (dateOffset.day === 1) {
        result += dateOffset.day + ' jour ';
    }

    if (dateOffset.hour > 1) {
        result += dateOffset.hour + ' heures ';
    }
    else if (dateOffset.hour === 1) {
        result += dateOffset.hour + ' heure ';
    }

    if (dateOffset.minute > 1) {
        result += dateOffset.minute + ' minutes ';
    }
    else if (dateOffset.minute === 1) {
        result += dateOffset.minute + ' minute ';
    }

    if (dateOffset.second > 1) {
        result += dateOffset.second + ' secondes ';
    }
    else if (dateOffset.second === 1) {
        result += dateOffset.second + ' seconde ';
    }

    result = result.trim();
    return result;
}
