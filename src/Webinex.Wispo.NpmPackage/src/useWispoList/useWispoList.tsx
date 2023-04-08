import { useCallback, useEffect, useRef } from 'react';
import { useDynamicMemo } from '../util';
import { Result } from './result';
import { UseWispoListArgs } from './useWispoListArgs';
import { useWispoListReducer, WispoListReducer } from './useWispoListReducer';
import { UseWispoListResult, useWispoListResultStateMemo } from './useWispoListResult';

function useFetch(args: UseWispoListArgs, reducer: WispoListReducer) {
  const [, dispatch] = reducer;
  const { client, skip, take, filter, include, sort } = args;
  const params = { skip, take, filter, include, sort };

  function fetch() {
    dispatch({ type: 'fetch/pending' });
    const promise = client.get(params);
    promise
      .then((payload) => dispatch({ type: 'fetch/fulfilled', payload }))
      .catch((error) => dispatch({ type: 'fetch/rejected', payload: { error } }));
    return new Result(promise);
  }

  return useDynamicMemo(() => fetch, [...Object.values(params), client]);
}

function useSignalRSubscription(args: UseWispoListArgs, reducer: WispoListReducer) {
  const { client } = args;

  const fetchRef = useRef<ReturnType<typeof useFetch>>();
  fetchRef.current = useFetch(args, reducer);

  useEffect(() => {
    const messageDispose = client.subscribe(() => fetchRef.current!());
    const reconnectedDispose = client.subscribeReconnected(() => fetchRef.current!());

    return () => {
      messageDispose();
      reconnectedDispose();
    };
  }, [client]);
}

function useMarkRead(args: UseWispoListArgs, reducer: WispoListReducer) {
  const [, dispatch] = reducer;
  const { client } = args;

  function markRead(id: string[]) {
    dispatch({ type: 'mark/pending', payload: { id } });
    const promise = client.markRead(id);
    promise
      .then(() => dispatch({ type: 'mark/fulfilled', payload: { id } }))
      .catch((error) => dispatch({ type: 'mark/rejected', payload: { id, error } }));

    return new Result(promise);
  }

  return useCallback(markRead, [client, dispatch]);
}

function useMarkViewRead(args: UseWispoListArgs, reducer: WispoListReducer) {
  const items = reducer[0].items;
  const markRead = useMarkRead(args, reducer);
  return useCallback(() => markRead(items), [markRead, items]);
}

function useMarkAllRead(args: UseWispoListArgs, reducer: WispoListReducer) {
  const [state, dispatch] = reducer;
  const { items } = state;
  const { client } = args;

  function markAllRead() {
    dispatch({ type: 'mark/pending', payload: { id: items } });
    const promise = client.markAllRead();
    promise
      .then(() => dispatch({ type: 'mark/fulfilled', payload: { id: items } }))
      .catch((error) =>
        dispatch({
          type: 'mark/fulfilled',
          payload: { id: items, error },
        }),
      );

    return new Result(promise);
  }

  return useCallback(markAllRead, [client, dispatch, items]);
}

/**
 * On mount: Fetches data from the server.
 * On SignalR message: Refreshes data in accordance with params.
 * On input change: Refreshes data.
 *
 * @example
 *
 * function useAppListState(config: UseWispoListParams) {
 *   return useWispoList({ ...config, client: yourWispoClient });
 * }
 *
 * const FILTER: FilterRule = {
 *   operator: '=',
 *   fieldId: NotificationField.IS_READ,
 *   value: false,
 * }
 *
 * const SORT: SortRule = {
 *   fieldId: NotificationField.CREATED_AT,
 *   dir: 'desc'
 * }
 *
 * const INCLUDE: NotificationListFieldValue[] = [
 *   NotificationListField.Items,
 *   NotificationListField.TotalUnread,
 * ];
 *
 * function NotificationList() { *
 *  const { isPending, items, markRead, markViewRead } = useAppListState({
 *    take: 10,
 *    filter: FILTER,
 *    sort: SORT,
 *    include: INCLUDE,
 *  });
 * }
 *
 * @param {UseWispoListArgs} args is {@link UseWispoListParams} and {@link WispoClient}
 * @returns {UseWispoListResult} as result
 */
export function useWispoList(args: UseWispoListArgs): UseWispoListResult {
  const reducer = useWispoListReducer();
  useSignalRSubscription(args, reducer);
  const fetch = useFetch(args, reducer);
  const markRead = useMarkRead(args, reducer);
  const markViewRead = useMarkViewRead(args, reducer);
  const markAllRead = useMarkAllRead(args, reducer);
  const [reducerState, dispatch] = reducer;
  const state = useWispoListResultStateMemo(reducerState);

  const actions = {
    dispatch,
    markRead,
    markViewRead,
    markAllRead,
    refresh: fetch,
  };

  useEffect(() => {
    fetch();
  }, [fetch]);

  return useDynamicMemo(
    () => ({ ...actions, ...state }),
    Object.values(actions).concat(Object.values(state)),
  );
}
