import { useRef } from 'react';

export function reduceBy<TItem, TKey extends keyof any, TValue = TItem>(
  items: TItem[],
  key: (item: TItem) => TKey,
  value?: (item: TItem) => TValue,
): Record<TKey, TValue> {
  return items.reduce((prev, current) => {
    prev[key(current)] = value ? value(current) : (current as any as TValue);
    return prev;
  }, {} as Record<TKey, TValue>);
}

export function shallowEqual<T>(x: T, y: T) {
  if (Object.is(x, y)) {
    return true;
  }

  if (x == null || y == null) {
    return false;
  }

  const xEntries = Object.entries(x);
  const yEntries = Object.entries(y);

  if (xEntries.length !== yEntries.length) {
    return false;
  }

  for (const key in x) {
    if (!y.hasOwnProperty(key)) {
      return false;
    }

    if (x[key] !== y[key]) {
      return false;
    }
  }

  return true;
}

export const guard = {
  notNull<T>(value: T, name: string): T {
    if (value == null) {
      throw new Error(`\`${name}\` might not be null.`);
    }

    return value;
  },
};

/**
 * Same as useMemo, but it allows dynamic range of dependencies
 * @param factory Factory method
 * @param deps Factory method dependencies
 */
export function useDynamicMemo<T>(factory: () => T, deps: any[]): T {
  guard.notNull(deps, 'deps');
  guard.notNull(factory, 'factory');

  const prevDeps = useRef<any[]>();
  const prevResult = useRef<T>();

  if (prevDeps.current !== undefined && shallowEqual(prevDeps.current, deps)) {
    return prevResult.current!;
  }

  prevDeps.current = deps;
  return (prevResult.current = factory());
}
