import { useReducer } from 'react';
import { Notification } from '../wispoClient';
import { reduceBy } from '../util';

export const NotificationListReducerAction = {
  fetchPending: 'fetch/pending',
  fetchFulfilled: 'fetch/fulfilled',
  fetchRejected: 'fetch/rejected',
  markPending: 'mark/pending',
  markFulfilled: 'mark/fulfilled',
  markRejected: 'mark/rejected',
  setState: 'common/setState',
} as const;

const Action = NotificationListReducerAction;

export type NotificationListReducerActionValue =
  (typeof NotificationListReducerAction)[keyof typeof NotificationListReducerAction];
export type WispoListReducer = ReturnType<typeof useWispoListReducer>;

export interface NotificationReducerItemState extends Notification {
  /**
   * True when markRead (for this item) or markAllRead called
   */
  markRead: {
    status: MarkReadStatus;
    error?: boolean;
  };
}

export enum FetchStatus {
  uninitialized = 'uninitialized',
  pending = 'pending',
  fulfilled = 'fulfilled',
  rejected = 'rejected',
}

export enum MarkReadStatus {
  uninitialized = 'uninitialized',
  pending = 'pending',
  reloading = 'reloading',
  rejected = 'rejected',
}

export interface NotificationListReducerState {
  status: FetchStatus;
  items: string[];
  itemById: { [key: string]: NotificationReducerItemState };
  total?: number;
  totalUnread?: number;
  totalMatch?: number;
  error?: any;
}

const INITIAL_STATE: NotificationListReducerState = {
  itemById: {},
  items: [],
  status: FetchStatus.uninitialized,
};

function updateItems(
  state: NotificationListReducerState,
  args: {
    id: string[];
    mutation: (item: NotificationReducerItemState) => NotificationReducerItemState;
  },
): NotificationListReducerState {
  const { id, mutation } = args;
  const markItems = id.map((x) => state.itemById[x]).map(mutation);
  const markItemsById = reduceBy(markItems, (x) => x.id);

  return {
    ...state,
    itemById: {
      ...state.itemById,
      ...markItemsById,
    },
  };
}

function reducer(state: NotificationListReducerState, action: any): NotificationListReducerState {
  const type: string = action.type;

  switch (type) {
    case Action.fetchPending:
      return { ...state, status: FetchStatus.pending };

    case Action.fetchFulfilled: {
      const { payload } = action;
      const items: Notification[] = payload.items;
      const { items: _, ...rest } = payload;
      const itemById: Record<string, NotificationReducerItemState> = reduceBy(
        items,
        (x) => x.id,
        (x) => ({ ...x, markRead: { status: MarkReadStatus.uninitialized } }),
      );

      return {
        ...state,
        status: FetchStatus.fulfilled,
        items: Object.keys(itemById),
        itemById,
        error: undefined,
        ...rest,
      };
    }

    case Action.fetchRejected: {
      const { payload } = action;
      return { ...state, status: FetchStatus.rejected, error: payload.error };
    }

    case Action.markPending: {
      const id: string[] = action.payload.id;
      return updateItems(state, {
        id,
        mutation: (item) => ({
          ...item,
          markRead: {
            ...item.markRead,
            status: MarkReadStatus.pending,
          },
        }),
      });
    }

    case Action.markFulfilled: {
      const id: string[] = action.payload.id;
      return updateItems(state, {
        id,
        mutation: (item) => ({
          ...item,
          markRead: {
            ...item.markRead,
            status: MarkReadStatus.reloading,
            error: undefined,
          },
        }),
      });
    }

    case Action.markRejected: {
      const id: string[] = action.payload.id;
      return updateItems(state, {
        id,
        mutation: (item) => ({
          ...item,
          markRead: {
            ...item.markRead,
            status: MarkReadStatus.rejected,
            error: action.payload.error,
          },
        }),
      });
    }

    case Action.setState:
      return action.payload(state);

    default:
      throw new Error(`Unknown type ${type}`);
  }
}

export function useWispoListReducer() {
  return useReducer(reducer, INITIAL_STATE);
}
