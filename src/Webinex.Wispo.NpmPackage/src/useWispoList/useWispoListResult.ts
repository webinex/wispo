import { Dispatch, ReducerAction, useMemo, useRef } from 'react';
import {
  FetchStatus,
  MarkReadStatus,
  NotificationReducerItemState,
  NotificationListReducerState,
} from './useWispoListReducer';
import { Notification, NotificationListResponse } from '../wispoClient';
import { reduceBy, shallowEqual, useDynamicMemo } from '../util';
import { Result } from './result';

export interface NotificationListItemState extends Notification {
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
}

export interface NotificationListState {
  status: FetchStatus;
  isPending: boolean;
  isFulfilled: boolean;
  isRejected: boolean;
  items: NotificationListItemState[];
  itemsId: string[];
  itemById: Record<string, NotificationListItemState>;
  total?: number;
  totalUnread?: number;
  totalMatch?: number;
  error?: any;
}

export interface UseWispoListResult extends NotificationListState {
  /**
   * Dispatch
   */
  dispatch: Dispatch<ReducerAction<any>>;

  /**
   * Marks specified notifications as read
   */
  markRead: (ids: string[]) => Result<void>;

  /**
   * Marks current shown notifications as read
   */
  markViewRead: () => Result<void>;

  /**
   * Marks all notifications (even not shown) as read
   */
  markAllRead: () => Result<void>;

  /**
   * Refreshes state
   */
  refresh: () => Result<NotificationListResponse>;
}

function mapItem(item: NotificationReducerItemState): NotificationListItemState {
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

export function useItemsMemo(state: NotificationListReducerState) {
  const input = Object.values(state.itemById);
  const prevById = useRef<typeof state.itemById>({});
  const prevByIdMapped = useRef<Record<string, NotificationListItemState>>({});

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

export function useWispoListResultStateMemo(state: NotificationListReducerState): NotificationListState {
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
