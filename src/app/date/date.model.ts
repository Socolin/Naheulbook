export interface CalendarDate {
    id: number;
    name: string;
    startDay: number;
    endDay: number;
    note: string;
}

export class NhbkDate {
    public minute = 0;
    public hour = 0;
    public day = 0;
    public year = 1498;

    constructor(minute?: number, hour?: number, day?: number, year?: number) {
        if (minute) {
            this.minute = minute;
        }
        if (hour) {
            this.hour = hour;
        }
        if (day) {
            this.day = day;
        }
        if (year) {
            this.year = year;
        }
    }
}

export class NhbkDateOffset {
    public minute = 0;
    public hour = 0;
    public day = 0;
    public week = 0;
    public year = 0;
    public second = 0;
}

