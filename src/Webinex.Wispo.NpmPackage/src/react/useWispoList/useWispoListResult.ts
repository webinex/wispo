import { Dispatch, ReducerAction, useMemo, useRef } from 'react';
import {
  FetchStatus,
  MarkReadStatus,
  NotificationReducerState,
  NotificationListReducerState,
} from './useWispoListReducer';
import { reduceBy, shallowEqual, useDynamicMemo } from '../../util';
import { Result } from '../../core/result';
import { NotificationBase, QueryResult } from '../../core';

export type NotificationState<TNotification extends NotificationBase> = TNotification & {
  /**
   * True when markRead (for this item) or markAllRead called
   */
  markRead: {
    status: MarkReadStatus;
    error?: boolean;
    isPending: boolean;
    isReloading: boolean;
    isRejected: boolean;
  };
};

export interface NotificationListState<TNotification extends NotificationBase> {
  status: FetchStatus;
  isPending: boolean;
  isFulfilled: boolean;
  isRejected: boolean;
  items: NotificationState<TNotification>[];
  itemsId: string[];
  itemById: Record<string, NotificationState<TNotification>>;
  total?: number;
  totalUnread?: number;
  totalMatch?: number;
  error?: any;
}

export interface UseWispoListResult<TNotification extends NotificationBase>
  extends NotificationListState<TNotification> {
  /**
   * Dispatch
   */
  dispatch: Dispatch<ReducerAction<any>>;

  /**
   * Marks specified notifications as read
   */
  read: (ids: string[]) => Result<void>;

  /**
   * Marks current shown notifications as read
   */
  readView: () => Result<void>;

  /**
   * Marks all notifications (even not shown) as read
   */
  readAll: () => Result<void>;

  /**
   * Refreshes state
   */
  refresh: () => Result<QueryResult<TNotification>>;
}

function mapItem<TNotification extends NotificationBase>(
  item: NotificationReducerState<TNotification>,
): NotificationState<TNotification> {
  const { markRead } = item;
  const { status } = markRead;

  return {
    ...item,
    markRead: {
      ...markRead,
      isPending: status === MarkReadStatus.pending,
      isRejected: status === MarkReadStatus.rejected,
      isReloading: status === MarkReadStatus.reloading,
    },
  };
}

export function useItemsMemo<TNotification extends NotificationBase>(
  state: NotificationListReducerState<TNotification>,
) {
  const input = Object.values(state.itemById);
  const prevById = useRef<typeof state.itemById>({});
  const prevByIdMapped = useRef<Record<string, NotificationState<TNotification>>>({});

  const items = useDynamicMemo(
    () =>
      input.map((x) => {
        const prev = prevById.current[x.id];
        return shallowEqual(prev, x) ? prevByIdMapped.current[x.id] : mapItem(x);
      }),
    input,
  );

  const itemById = useMemo(() => reduceBy(items, (x) => x.id), [items]);
  const itemsId = useMemo(() => items.map((x) => x.id), [items]);

  prevById.current = state.itemById;
  prevByIdMapped.current = itemById;

  return { items, itemById, itemsId };
}

export function useWispoListResultStateMemo<TNotification extends NotificationBase>(
  state: NotificationListReducerState<TNotification>,
): NotificationListState<TNotification> {
  const { itemById: _1, items: _2, ...rest } = state;
  const itemsState = useItemsMemo(state);

  return useDynamicMemo(
    () => ({
      ...rest,
      ...itemsState,
      isFulfilled: state.status === FetchStatus.fulfilled,
      isPending: state.status === FetchStatus.pending,
      isRejected: state.status === FetchStatus.rejected,
    }),
    Object.values(rest).concat(itemsState),
  );
}
