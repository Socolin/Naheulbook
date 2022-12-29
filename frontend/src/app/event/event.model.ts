import {EventResponse} from '../api/responses';

export class NEvent {
    id: number;
    name: string;
    description: string;
    timestamp: number;

    static fromResponse(response: EventResponse): NEvent {
        let event = new NEvent();
        Object.assign(event, response);
        return event;
    }

    static fromResponses(responses: EventResponse[]): NEvent[] {
        let events: NEvent[] = [];
        for (let eventData of responses) {
            events.push(NEvent.fromResponse(eventData));
        }
        return events;
    }
}
