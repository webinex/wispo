import { useCallback, useEffect, useRef } from 'react';
import { useDynamicMemo } from '../../util';
import { UseWispoListArgs } from './useWispoListArgs';
import { useWispoListReducer, WispoListReducer } from './useWispoListReducer';
import { UseWispoListResult, useWispoListResultStateMemo } from './useWispoListResult';
import { NotificationBase, Result } from '../../core';

function useFetch<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
  reducer: WispoListReducer<TNotification>,
) {
  const [, dispatch] = reducer;
  const { client, ...params } = args;

  function fetch() {
    dispatch({ type: 'fetch/pending' });
    const promise = client.query(params);
    promise
      .then((payload) => dispatch({ type: 'fetch/fulfilled', payload }))
      .catch((error) => dispatch({ type: 'fetch/rejected', payload: { error } }));
    return new Result(promise);
  }

  return useDynamicMemo(() => fetch, [...Object.values(params), client]);
}

function useSignalRSubscription<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
  reducer: WispoListReducer<TNotification>,
) {
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

function useRead<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
  reducer: WispoListReducer<TNotification>,
) {
  const [, dispatch] = reducer;
  const { client } = args;

  function markRead(id: string[]) {
    dispatch({ type: 'mark/pending', payload: { id } });
    const promise = client.read(id);
    promise
      .then(() => dispatch({ type: 'mark/fulfilled', payload: { id } }))
      .catch((error) => dispatch({ type: 'mark/rejected', payload: { id, error } }));

    return new Result(promise);
  }

  return useCallback(markRead, [client, dispatch]);
}

function useMarkViewRead<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
  reducer: WispoListReducer<TNotification>,
) {
  const items = reducer[0].items;
  const markRead = useRead(args, reducer);
  return useCallback(() => markRead(items), [markRead, items]);
}

function useMarkAllRead<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
  reducer: WispoListReducer<TNotification>,
) {
  const [state, dispatch] = reducer;
  const { items } = state;
  const { client } = args;

  function markAllRead() {
    dispatch({ type: 'mark/pending', payload: { id: items } });
    const promise = client.readAll();
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
export function useWispoList<TNotification extends NotificationBase>(
  args: UseWispoListArgs<TNotification>,
): UseWispoListResult<TNotification> {
  const reducer = useWispoListReducer<TNotification>();
  useSignalRSubscription(args, reducer);
  const fetch = useFetch(args, reducer);
  const read = useRead(args, reducer);
  const readView = useMarkViewRead(args, reducer);
  const readAll = useMarkAllRead(args, reducer);
  const [reducerState, dispatch] = reducer;
  const state = useWispoListResultStateMemo(reducerState);

  const actions = {
    dispatch,
    read,
    readView,
    readAll,
    refresh: fetch,
  };

  useEffect(() => {
    fetch();
  }, [fetch]);

  return useDynamicMemo<UseWispoListResult<TNotification>>(
    () => ({ ...actions, ...state }),
    Object.values(actions).concat(Object.values(state)),
  );
}
