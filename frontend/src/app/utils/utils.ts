export function assertNever(x: never): never {
    throw new Error('Unexpected object: ' + x);
}

export function toDictionary<T extends { id: number }>(array: T[]): { [id: number]: T } {
    return array.reduce((dic: { [id: number]: T }, element) => {
        dic[element.id] = element;
        return dic;
    }, {});
}

export function toDictionaryByKey<T extends {}>(
    array: T[],
    keySelector: (element: T) => string,
)
    : { [key: string]: T } {
    return array.reduce((dic: { [k: string]: T }, element) => {
        dic[keySelector(element)] = element;
        return dic;
    }, {});
}
