import { useEffect, useState } from 'react';
import { except, guard, unique } from '../util';
import { WispoClient } from '../wispoClient';
import { WISPO_SIGNALR_MESSAGES } from '../wispoClient/wispoSignalRMessages';

type Subscriber = (value: number) => any;
const STATE_SYMBOL = Symbol('__WISPO_UNREAD_STATE_SYMBOL__');

/**
 * Preloads count of unread messages and tracks SignalR messages
 * to globally update unread count state
 */
export class WispoUnreadState {
  private _client: WispoClient;
  private _id: string[] = [];
  private _subscribers: Subscriber[] = [];

  constructor(client: WispoClient) {
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
    return this._id.length;
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
    this._id = await this._client.unreadIds();
    count !== this.count && this.notify();
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
      case WISPO_SIGNALR_MESSAGES.New:
        this.handleNew(args);
        break;
      case WISPO_SIGNALR_MESSAGES.Read:
        this.handleRead(args);
        break;
    }
  };

  private handleNew = ([_, id]: [unknown, string[]]) => {
    guard.notNull(id, 'id');
    this._id = unique(this._id, id);
  };

  private handleRead = ([_, id]: [unknown, string[]]) => {
    guard.notNull(id, 'id');
    this._id = except(this._id, id);
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
export function useWispoUnreadState(client: WispoClient): WispoUnreadState {
  const instance = client as any;

  if (instance[STATE_SYMBOL]) {
    return instance[STATE_SYMBOL];
  }

  const state = new WispoUnreadState(client);
  instance[STATE_SYMBOL] = state;
  return state;
}

/**
 * Fetchs unread count from server and tracks new signalR messages
 * @param client wispo client
 * @returns count of unread messages
 */
export function useWispoUnreadCount(client: WispoClient) {
  const state = useWispoUnreadState(client);
  const [value, setValue] = useState<number>(state.count);

  useEffect(() => state.subscribe(setValue), [state]);
  return value;
}
