import axios, { AxiosInstance } from 'axios';
import * as SignalR from '@microsoft/signalr';
import { guard } from '../util';
import { NotificationBase, Query, QueryResult, SIGNALR_MESSAGE } from './types';

/**
 * Wispo client configuration
 */
export interface WispoClientConfig {
  /**
   * SignalR configuration
   */
  signalR?: WispoSignalRConfig;

  /**
   * Http requests configuration
   */
  http?: WispoHttpConfig;
}

/**
 * Wispo client SignalR configuration
 */
export interface WispoSignalRConfig {
  /**
   * Hub URI
   * @default /api/wispo/hub
   */
  hubUri?: string;

  /**
   * Access token factory
   */
  accessTokenFactory?: () => Promise<string>;

  /**
   * Request headers factory
   */
  headersFactory?: () => Promise<{ [name: string]: string }>;
}

/**
 * Wispo client http requests configuration
 */
export interface WispoHttpConfig {
  /**
   * Axios instance for http requests
   * Useful for authorization and before/post processing
   * Default: empty axios instance with base uri /api/wispo
   */
  axios?: AxiosInstance;
}

const DEFAULT_SIGNALR_CONFIG: WispoSignalRConfig = {
  hubUri: '/api/wispo/hub',
  headersFactory: () => Promise.resolve({}),
};

const DEFAULT_HTTP_CONFIG: WispoHttpConfig = {
  axios: axios.create({ baseURL: '/api/wispo' }),
};

/**
 * Type for SignalR message callback
 */
export type MessageSubscriber = (kind: string, payload: any[]) => any;

/**
 * Wispo client
 */
export class WispoClient<TNotification extends NotificationBase> {
  private _subscribers: MessageSubscriber[] = [];
  private _reconnectedSubscribers: Array<() => any> = [];

  /**
   * SignalR connection. Would be null until `connect` function call
   */
  public connection: SignalR.HubConnection = null!;

  /**
   * Client configuration, merged with default values
   */
  public config: WispoClientConfig;

  /**
   * Creates new instance of wispo client
   * @param config Config override, would be merged with default values
   */
  constructor(config?: WispoClientConfig) {
    config = config ?? {};
    const httpConfig = config.http ?? {};
    const signalRConfig = config?.signalR ?? {};

    this.config = {
      ...config,
      http: { ...DEFAULT_HTTP_CONFIG, ...httpConfig },
      signalR: { ...DEFAULT_SIGNALR_CONFIG, ...signalRConfig },
    };
  }

  /**
   * Initializes new connection with signalR hub and subscribes for messages
   */
  public connect = async () => {
    const headers = await this.config.signalR!.headersFactory!();

    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(this.config.signalR!.hubUri!, {
        accessTokenFactory: this.config.signalR!.accessTokenFactory
          ? this.config.signalR!.accessTokenFactory
          : undefined,
        headers,
      })
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(() => this._reconnectedSubscribers.forEach((s) => s()));

    Object.values(SIGNALR_MESSAGE).forEach((kind) => {
      this.connection.on(kind, (...args) => this.publishMessage(kind, args));
    });

    await this.connection.start();
  };

  private publishMessage = (kind: string, payload: any[]) => {
    this._subscribers.forEach((subscriber) => subscriber(kind, payload));
  };

  /**
   * Subsribes for SignalR messages
   * @param subscriber New message callback
   * @returns Dispose function
   */
  public subscribe = (subscriber: MessageSubscriber) => {
    guard.notNull(subscriber, 'subscriber');

    this._subscribers.push(subscriber);
    return () => this.unsubscribe(subscriber);
  };

  /**
   * Unsubscribes callback
   * @param subscriber Subscribe callback used when you called `subscribe`
   */
  public unsubscribe = (subscriber: MessageSubscriber) => {
    guard.notNull(subscriber, 'subscriber');

    const index = this._subscribers.indexOf(subscriber);
    this._subscribers.splice(index, 1);
  };

  /**
   * Subscribes SignalR reconnection.
   * Useful to fetch data when some of messages can be loosed after disconnection
   * @param subscriber subscribe callback
   * @returns Dispose function
   */
  public subscribeReconnected = (subscriber: () => any) => {
    guard.notNull(subscriber, 'subscriber');

    this._reconnectedSubscribers.push(subscriber);
    return () => this.unsubscribeReconnected(subscriber);
  };

  public unsubscribeReconnected = (subscriber: () => any) => {
    guard.notNull(subscriber, 'subscriber');

    this._reconnectedSubscribers.splice(this._reconnectedSubscribers.indexOf(subscriber), 1);
  };

  /**
   * Fetches unread notifications count
   * @returns total count of unread notifications
   */
  public totalUnreadCount = async (): Promise<number> => {
    const { data } = await this.axios.get('unread/count');
    return data;
  };

  /**
   * Marks notifications as read
   * @param id Required. Notification identifiers which might be marked as read
   */
  public read = async (id: string[]): Promise<void> => {
    guard.notNull(id, 'id');
    if (id.length === 0) {
      return;
    }

    await this.axios.put('read', id);
  };

  /**
   * Marks all unread notificatiosn as read.
   */
  public readAll = async (): Promise<void> => {
    await this.axios.put('read-all');
  };

  /**
   * This method allows you to query exceed data for your needs
   * @param query query
   * @returns data in accordance with `args` includes
   */
  public query = async (query: Query): Promise<QueryResult<TNotification>> => {
    guard.notNull(query, 'query');

    const searchParams = this.convertToSearchParams(query);
    const uri = '?' + searchParams.toString();
    const result = await this.axios.get<QueryResult<TNotification>>(uri);
    return result.data;
  };

  private get axios(): AxiosInstance {
    if (!this.config.http?.axios) throw new Error('Axios unspecified');
    return this.config.http.axios!;
  }

  private convertToSearchParams = (query: Query) => {
    const { filterRule, props, pagingRule, sortRule } = query;

    const searchParams = new URLSearchParams();
    filterRule && searchParams.append('filterRule', JSON.stringify(filterRule));
    sortRule && searchParams.append('sortRule', JSON.stringify(sortRule));
    props && searchParams.append('props', props.join(','));
    pagingRule && searchParams.append('pagingRule', JSON.stringify(pagingRule));

    return searchParams;
  };
}
