import { useEffect, useState } from 'react';
import { guard } from '../../util';
import { WispoClient, SIGNALR_MESSAGE, NotificationBase } from '../../core';

type Subscriber = (value: number) => any;
const STATE_SYMBOL = Symbol('__WISPO_UNREAD_STATE_SYMBOL__');

/**
 * Preloads count of unread messages and tracks SignalR messages
 * to globally update unread count state
 */
export class WispoUnreadState<TNotification extends NotificationBase> {
  private _client: WispoClient<TNotification>;
  private _count: number = -1;
  private _subscribers: Subscriber[] = [];

  constructor(client: WispoClient<TNotification>) {
    this._client = guard.notNull(client, 'client');
  }

  /**
   * Adds subscriber which would be invoked when unread count changed
   * @param subscriber Subscriber callback
   * @returns Dispose function, to unsubscribe from changes
   */
  public subscribe = (subscriber: Subscriber) => {
    guard.notNull(subscriber, 'subscriber');
    this._subscribers.push(subscriber);
    this._subscribers.length === 1 && this.initialize();
    return () => this.unsubscribe(subscriber);
  };

  public get count() {
    return this._count;
  }

  public unsubscribe = (subscriber: Subscriber) => {
    guard.notNull(subscriber, 'subscriber');
    this._subscribers.splice(this._subscribers.indexOf(subscriber), 1);
    this._subscribers.length === 0 && this.dispose();
  };

  private initialize = () => {
    this.fetch();
    this._client.subscribe(this.handleMessage);
    this._client.subscribeReconnected(this.fetch);
  };

  private dispose = () => {
    this._client.unsubscribe(this.handleMessage);
    this._client.unsubscribeReconnected(this.fetch);
  };

  private fetch = async () => {
    const count = this.count;
    this._count = await this._client.totalUnreadCount();
    count !== this._count && this.notify();
  };

  private handleMessage = (kind: string, args: any) => {
    guard.notNull(kind, 'kind');
    guard.notNull(args, 'args');

    const count = this.count;
    this.processMessage(kind, args);
    this.count !== count && this.notify();
  };

  private processMessage = (kind: string, args: any) => {
    switch (kind) {
      case SIGNALR_MESSAGE.New:
        this.handleNew(args);
        break;
      case SIGNALR_MESSAGE.Read:
        this.handleRead(args);
        break;
    }
  };

  private handleNew = ([notifications]: [TNotification[]]) => {
    this._count += notifications.length;
  };

  private handleRead = ([ids]: [string[]]) => {
    this._count -= ids.length;
  };

  private notify = () => {
    for (const subscriber of this._subscribers) {
      subscriber(this.count);
    }
  };
}

/**
 * Associates wispo unread state with client
 * @param client wispo client
 * @returns Wispo unread state associated with client
 */
function useWispoUnreadState<TNotification extends NotificationBase>(
  client: WispoClient<TNotification>,
): WispoUnreadState<TNotification> {
  const instance = client as any;

  if (instance[STATE_SYMBOL]) {
    return instance[STATE_SYMBOL];
  }

  const state = new WispoUnreadState<TNotification>(client);
  instance[STATE_SYMBOL] = state;
  return state;
}

/**
 * Fetchs unread count from server and tracks new signalR messages
 * @param client wispo client
 * @returns count of unread messages
 */
export function useWispoUnreadCount<TNotification extends NotificationBase>(
  client: WispoClient<TNotification>,
) {
  const state = useWispoUnreadState(client);
  const [value, setValue] = useState<number>(state.count);

  useEffect(() => state.subscribe(setValue), [state]);
  return value;
}
