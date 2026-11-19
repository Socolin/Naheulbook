import {EntryGroupView, EntrySource, toEntryView} from './entry-group-tabs.model';

/**
 * Reference entry that can be learned (talent, mastery): it may need a level and another entry known before
 */
export interface LearnableEntry extends EntrySource {
    data: EntrySource['data'] & { requiredLevel?: number };
}

export interface LearnableGroup<T extends LearnableEntry> {
    id: string;
    name: string;
    description?: string;
    entries: T[];
}

export interface Candidates {
    groups: EntryGroupView[];
    /** Unmet conditions by entry id, shown as warnings (they do not block the choice: the GM can make exceptions) */
    warnings: Record<string, string[]>;
}

/**
 * Entries the character can still learn, grouped, with the conditions they do not meet yet
 */
export function buildCandidates<T extends LearnableEntry>(
    groups: LearnableGroup<T>[],
    knownIds: Set<string>,
    level: number,
    getPrerequisiteId: (entry: T) => string | undefined,
): Candidates {
    const namesById = getNamesById(groups);
    const all = groups.flatMap(g => g.entries);
    return {
        groups: groups.map(group => ({
            id: group.id,
            name: group.name,
            description: group.description,
            entries: group.entries
                .filter(e => !knownIds.has(e.id))
                .map(e => toEntryView(e, getPrerequisiteId(e), namesById)),
        })),
        warnings: Object.fromEntries(all.map(e => [e.id, getWarnings(e, getPrerequisiteId(e), knownIds, level, namesById)])),
    };
}

export function getNamesById<T extends LearnableEntry>(groups: LearnableGroup<T>[]): Map<string, string> {
    return new Map(groups.flatMap(g => g.entries).map(e => [e.id, e.name]));
}

export function getWarnings(
    entry: LearnableEntry,
    prerequisiteId: string | undefined,
    knownIds: Set<string>,
    level: number,
    namesById: Map<string, string>,
): string[] {
    const warnings: string[] = [];
    if (entry.data.requiredLevel && entry.data.requiredLevel > level) {
        warnings.push(`Niveau ${entry.data.requiredLevel} requis`);
    }
    if (prerequisiteId && !knownIds.has(prerequisiteId)) {
        warnings.push(`Requiert « ${namesById.get(prerequisiteId) ?? '?'} »`);
    }
    return warnings;
}
